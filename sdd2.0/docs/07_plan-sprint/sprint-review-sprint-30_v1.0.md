# Sprint Review — Sprint 30

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-30_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-30_v1.0.md`:

> Entregar el mapa interactivo de la revisión en el front web (`GeoVial.Web`, Blazor): mostrar los marcadores georreferenciados del relevamiento sobre un mapa **Leaflet con teselas de OpenStreetMap**, con popups por marcador (coordenada, en conflicto, conteo de fotos y comentarios). **Sin clave de proveedor**.

Veredicto: Cumplido.

Explicación corta: se reencauzó el ítem "mapa interactivo" que se venía postergando como "bloqueado por la clave de proveedor de mapas". El bloqueo real era únicamente el control de mapa **nativo** de la plataforma (Google Maps en Android, que sí exige API key); **OpenStreetMap es libre y no usa clave** (sólo atribución). El núcleo testeable `VistaMapa` (en `GeoVial.Revision`, dentro del gate) proyecta los marcadores a pines y calcula el centro y la caja contenedora (bounds) para encuadrar el mapa; con cero marcadores usa un centro por defecto. El front `GeoVial.Web` (Blazor InteractiveServer) carga Leaflet + teselas OSM (atribución incluida) en `App.razor`, y la página `Revision.razor` —que ya listaba los marcadores— ahora dibuja el mapa con un módulo JS (`mapaRevision.js`): un marcador por coordenada con popup (coordenada, conflicto, nº de fotos y comentarios), encuadrado a un punto o a la caja de todos. Se redibuja al recargar/filtrar y se libera al disponer. El render JS queda fuera del gate; el cálculo de la vista entra al gate.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-21 | Producto | Revisión muestra los marcadores sobre un mapa OSM interactivo (zoom/arrastre), sin clave | Visualización geográfica al fin disponible |
| US-21 | Producto | Popup por marcador con coordenada, estado de conflicto y conteo de fotos/comentarios | Contexto del marcador a la vista |
| US-21 | Calidad | `VistaMapa` calcula centro/bounds y proyecta pines (núcleo en el gate, 100 % cubierto) | Lógica de encuadre verificada |

## 3. Feedback recibido

- El mapa cierra la última pieza de producto pendiente del MVP; quedó claro que "mapa" no implicaba "Google Maps" y que OSM resolvía el caso sin credenciales.
- Mantener el cálculo del encuadre en un núcleo testeable (`VistaMapa`) deja la lógica verificada y la página Blazor como cáscara fina de render.
- Para producción con alto volumen se documentó la alternativa de proveedor de teselas o caché; para el MVP, OSM directo con atribución es suficiente.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 320 verdes (283 unitarias + 37 de integración), +5 unitarias (`VistaMapa`). Cobertura del gate: Domain líneas 89,7 % / branches 79,8 %; Application 90,0 % / 82,0 %; Revision 99,1 % / 100 % (`VistaMapa` 100 % / 100 %). `GeoVial.Web` compila en Release sin warnings tratados como error; el render Leaflet/OSM queda fuera del gate.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-21-MAPA | Historia | Aceptada (mapa interactivo OSM en la web con `VistaMapa` en el gate; sin clave de proveedor) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 30 se traslada. |

Con el mapa interactivo entregado, el MVP de producto queda completo. El backlog restante es mantenimiento post-release (Dependabot/CVE por SLA, PATCH si surge) y mejoras opcionales (mapa OSM también en el móvil con Mapsui/WebView; teselas cacheadas para offline).

## 7. Decisiones tomadas durante el review

- Usar OpenStreetMap (Leaflet) en lugar del control nativo: elimina la dependencia de una API key y desbloquea el ítem.
- Mantener el cálculo del encuadre (`VistaMapa`) en `GeoVial.Revision` (gate) y el render en la página Blazor (fuera de CI), coherente con el patrón núcleo-testeable de sprints previos.
- Cargar Leaflet por CDN para el MVP; vendorizar/cachear queda documentado como opción para offline/producción.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Sprint review del Sprint 30 (mapa interactivo de la revisión sobre OpenStreetMap en la web, sin clave). Veredicto Cumplido, velocity 8, 0 carry-over, 320 pruebas verdes. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
