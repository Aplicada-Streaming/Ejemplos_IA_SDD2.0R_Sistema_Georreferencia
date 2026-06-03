# Mapa de la revisión: vendorizado de Leaflet y estrategia de teselas/offline — GeoVial

**Proyecto:** GeoVial
**Documento:** mapa-offline-teselas_v1.0.md
**Versión:** 1.1
**Estado:** Aceptado (Leaflet vendorizado en S31; caché de teselas por Service Worker en S34)
**Fecha:** 2026-06-03
**Autor:** Ingeniero DevOps Senior (AG-09), Equipo SDD 2.0
**Trazabilidad upstream:** US-21 (revisión sobre mapa); plan/review Sprint 30 (mapa OSM) y Sprint 31 (vendorizado)
**Trazabilidad downstream:** `GeoVial.Web/wwwroot/lib/leaflet`, `Components/App.razor`, `wwwroot/js/mapaRevision.js`

El mapa interactivo de la revisión (US-21) usa **Leaflet** con teselas de **OpenStreetMap (OSM)**. Este documento distingue dos cosas que suelen confundirse —la **librería** del mapa y las **teselas**— y fija qué queda offline y qué no.

## 1. Qué está offline y qué no

| Componente | Origen | ¿Offline? |
| --- | --- | --- |
| **Librería Leaflet** (`leaflet.js`, `leaflet.css`, íconos) | Vendorizada en `wwwroot/lib/leaflet/` (Sprint 31) | **Sí**: se sirve desde el propio front, sin Internet |
| **Teselas del mapa** (imágenes de cada zoom/tile) | `https://tile.openstreetmap.org/{z}/{x}/{y}.png` por red, **cacheadas** por el Service Worker (S34) | **Parcial (web)**: las **ya visitadas** se ven sin red; las nuevas requieren red |
| **Marcadores y popups** | Coordenadas de la revisión (API propia) | **Sí**: se calculan en el front (`VistaMapa`) sin depender de las teselas |

Conclusión: tras el Sprint 31, **la librería no depende de un CDN**; las **teselas sí requieren red** (es inherente a un mapa de teselas). Si no hay red de teselas, Leaflet igualmente carga y los marcadores se ubican por coordenada sobre un fondo gris (degradación elegante).

## 2. Vendorizado de Leaflet (implementado)

- Archivos de Leaflet **1.9.4** en `GeoVial.Web/wwwroot/lib/leaflet/`: `leaflet.js`, `leaflet.css` e `images/` (`marker-icon.png`, `marker-icon-2x.png`, `marker-shadow.png`, `layers.png`, `layers-2x.png`).
- `Components/App.razor` referencia los archivos locales (`lib/leaflet/leaflet.css` y `lib/leaflet/leaflet.js`); se quitó la referencia a `unpkg` y los `integrity/crossorigin`.
- `wwwroot/js/mapaRevision.js` fija `L.Icon.Default.imagePath = "lib/leaflet/images/"` para que los íconos de marcador resuelvan desde la carpeta vendorizada (sin esto, Leaflet autodetecta la ruta del CSS y puede fallar al vendorizar).
- El pipeline de **static web assets** de Blazor incluye y comprime (`.br`/`.gz`) estos archivos en el `dotnet publish` (verificado: aparecen en `publish/wwwroot/lib/leaflet/`). La versión queda fijada; su actualización se hace por PR.

## 3. Estrategia de teselas (opciones para offline real / alto volumen)

Para el MVP/demo, las teselas se piden directamente a OSM con la **atribución** obligatoria (la agrega el `tileLayer`) y dentro de la [política de uso de teselas de OSM](https://operations.osmfoundation.org/policies/tiles/) (volumen razonable, sin scraping masivo). Para uso desconectado o de alto volumen, opciones de menor a mayor esfuerzo:

1. **Caché HTTP del navegador**: las teselas ya se cachean por `Cache-Control`; mejora la repetición de vistas, no el primer uso offline.
2. **Service Worker (web)** — **implementado en S34** (ver §3.1): cachea las teselas visitadas para volver a verlas sin red. Cubre las zonas ya navegadas, no zonas nuevas.
3. **Proxy/caché de teselas propio**: un servicio que cachea teselas de OSM (o de un proveedor) y las sirve desde la infraestructura; reduce dependencia de la red pública y respeta cuotas. Recomendado para producción de volumen.
4. **Teselas pre-bundleadas de un área acotada**: para un relevamiento de zona conocida, generar/empaquetar las teselas del área y servirlas localmente (offline total en esa zona). Mayor esfuerzo y tamaño.
5. **Proveedor de teselas gestionado** (p. ej. con plan/clave): sólo si se necesita SLA/estilos propios; reintroduce una credencial y queda fuera del enfoque keyless actual.

La elección depende del requisito real de offline; el MVP usa la opción directa con atribución y, además, el Service Worker de caché (§3.1) para que las zonas ya vistas funcionen sin red.

### 3.1 Service Worker de caché de teselas (implementado, S34)

- Archivo: `GeoVial.Web/wwwroot/sw-teselas.js`; se registra en `Components/App.razor` (`navigator.serviceWorker.register('sw-teselas.js')`).
- Estrategia: **cache-first** sólo para las teselas de OpenStreetMap (host `tile.openstreetmap.org`, extensión `.png`); el resto de las peticiones no se intercepta. Una tesela ya vista se sirve desde la caché aunque no haya red.
- Respuestas opacas: las teselas son peticiones no-cors (respuesta `opaque`, status 0); se cachean igualmente.
- Límite: caché acotada a 500 entradas con recorte **FIFO** (se borran las más viejas) para no crecer sin control.
- Contrato compartido: qué URL es una tesela cacheable está centralizado y testeado en `GeoVial.Revision.MapaTeselas.EsUrlDeTesela` (mismo criterio host+`.png` que el Service Worker); el nombre de la caché (`geovial-teselas-v1`) versiona el contenido.
- Alcance: **web** (Blazor). El WebView del móvil no usa este Service Worker; su offline de teselas queda como mejora futura (caché nativa o proxy).

> Verificación manual: cargar el mapa con red para poblar la caché, luego cortar la red y volver a la zona ya vista (las teselas se siguen mostrando); las zonas nuevas quedan en blanco. La caché se inspecciona/limpia desde las DevTools del navegador (Application → Cache Storage → `geovial-teselas-v1`).

## 4. Verificación

```bash
# 1. La librería se sirve localmente (sin red): build/publish bundlea Leaflet
dotnet publish src/GeoVial.Web -c Release -o ./pub
ls ./pub/wwwroot/lib/leaflet ./pub/wwwroot/lib/leaflet/images   # leaflet.js/.css + 5 png

# 2. En la página /revision: con red, el mapa muestra teselas OSM y marcadores;
#    sin red de teselas, Leaflet carga igual y los marcadores se ubican por coordenada.
```

## 5. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Documento inicial (Sprint 31): vendorizado de Leaflet 1.9.4 en `wwwroot` (sin CDN), `imagePath` fijado para los íconos, y estrategia de teselas/offline (qué es offline —la librería— vs. qué requiere red —las teselas— con opciones de caché/proxy/bundle). Por AG-09 |
| 1.1 | 2026-06-03 | Caché de teselas por Service Worker implementada (Sprint 34): `sw-teselas.js` cachea (cache-first, FIFO, 500 entradas) las teselas OSM ya visitadas en la web → offline parcial de zonas navegadas. Contrato de "qué es tesela" centralizado y testeado en `MapaTeselas`. §1 y §3 actualizados (nuevo §3.1). Alcance web; móvil pendiente. Por AG-09 |
