# Sprint Review — Sprint 31

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-31_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-31_v1.0.md`:

> Quitar la dependencia de CDN del mapa de la revisión: servir **Leaflet desde `wwwroot`** (vendorizado) en lugar de unpkg, de modo que la librería del mapa no requiera Internet en runtime; y documentar la estrategia de **teselas/offline**.

Veredicto: Cumplido.

Explicación corta: se vendorizó **Leaflet 1.9.4** en `GeoVial.Web/wwwroot/lib/leaflet/` (`leaflet.js`, `leaflet.css` y `images/` con los íconos), `App.razor` ahora lo referencia localmente (sin `unpkg`/integrity) y `mapaRevision.js` fija `L.Icon.Default.imagePath` a la carpeta vendorizada para que los íconos de marcador resuelvan sin autodetección. Se verificó que `dotnet publish` bundlea y comprime (`.br`/`.gz`) los assets en `publish/wwwroot/lib/leaflet/`. Además se documentó la estrategia de teselas/offline (`09_devops/mapa-offline-teselas`), separando explícitamente lo que queda offline —la **librería** Leaflet— de lo que sigue requiriendo red —las **teselas** de OSM—, con opciones (caché HTTP, Service Worker/PWA, proxy de teselas, bundle de un área, o proveedor gestionado). Con esto, la librería del mapa no depende de un CDN; las teselas siguen por red, con degradación elegante si no hay conexión (Leaflet carga y los marcadores se ubican por coordenada).

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-21 | Frontend | El mapa carga Leaflet desde el propio front (sin CDN); `publish` incluye los assets | Sin dependencia de red para la librería |
| US-21 | Frontend | Los íconos de marcador resuelven desde la carpeta vendorizada | Marcadores correctos al vendorizar |
| US-21 | DevOps | Documento que separa librería (offline) de teselas (red) con opciones de caché/proxy | Estrategia de offline clara |

## 3. Feedback recibido

- Vendorizar la librería elimina un punto de fallo (CDN caído/bloqueado) y deja el front autocontenido para la parte de Leaflet.
- Quedó explícito que "Leaflet offline" no es "mapa offline": las teselas son el componente que requiere red, y su offline real es un esfuerzo aparte (caché/bundle).
- Para el MVP, teselas directas de OSM con atribución es suficiente; las opciones de producción quedan documentadas para cuando haga falta.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 320 verdes (283 unitarias + 37 de integración), sin cambio respecto del Sprint 30: trabajo de assets/frontend sin lógica de dominio nueva, por lo que el gate de cobertura se mantiene. Verificación específica: `GeoVial.Web` compila en Release sin warnings y el `dotnet publish` bundlea Leaflet (`leaflet.js`/`.css` + 5 íconos) en `wwwroot/lib/leaflet/`.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-MAPA-OFFLINE | Tarea | Aceptada (Leaflet vendorizado en `wwwroot`, `imagePath` fijado; doc de teselas/offline) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 31 se traslada. |

Backlog restante: mejoras opcionales (mapa OSM también en el móvil con Mapsui/WebView; caché/Service Worker de teselas si se requiere offline real) y mantenimiento post-release (Dependabot/CVE por SLA).

## 7. Decisiones tomadas durante el review

- Fijar la versión de Leaflet (1.9.4) en `wwwroot` y actualizar por PR cuando corresponda, en lugar de seguir un CDN móvil.
- Fijar `L.Icon.Default.imagePath` explícitamente para evitar la fragilidad de la autodetección de la ruta de íconos al vendorizar.
- Mantener las teselas directas de OSM (con atribución) para el MVP y documentar las opciones de offline/alto volumen sin implementarlas aún.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Sprint review del Sprint 31 (vendorizado de Leaflet + estrategia de teselas/offline). Veredicto Cumplido, velocity 8, 0 carry-over, 320 pruebas verdes. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
