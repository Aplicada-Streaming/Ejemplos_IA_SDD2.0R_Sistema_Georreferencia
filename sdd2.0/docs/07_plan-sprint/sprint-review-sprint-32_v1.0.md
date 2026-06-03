# Sprint Review — Sprint 32

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-32_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-32_v1.0.md`:

> Llevar el mapa interactivo de la revisión a la app móvil (`GeoVial.Mobile`, MAUI): mostrar los marcadores georreferenciados sobre un mapa **Leaflet con teselas de OpenStreetMap** dentro de un **WebView**, **sin clave de proveedor**.

Veredicto: Cumplido.

Explicación corta: el núcleo testeable `MapaRevisionHtml.Construir(VistaMapa)` (en `GeoVial.Revision`, dentro del gate) arma el documento HTML autocontenido del mapa —Leaflet + teselas OSM con atribución, y los marcadores de la vista como JSON, con popup por marcador y encuadre a un punto o a la caja contenedora—. La app móvil suma `MapaRevisionPage` (un `WebView` que carga ese HTML, con toolbar "Cerrar") y un botón "Ver en mapa" en `RevisionPage` que lo abre con los marcadores del relevamiento cargado. Se eligió **WebView + Leaflet** en lugar de un control de mapa nativo (Google, que exige clave) o de una librería nativa (Mapsui, que agregaría una dependencia NuGet con riesgo de warnings de vulnerabilidad bajo `TreatWarningsAsErrors`): así se reusa el mismo enfoque OSM del front web, sin clave y sin dependencia nueva. El núcleo entra al gate (100 % cubierto); la pantalla MAUI queda fuera de CI y compila para android-arm64.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-21 | Producto | La app móvil abre el mapa OSM con los marcadores del relevamiento (zoom/arrastre, popups) | Visualización geográfica también en el móvil |
| US-21 | Producto | "Ver en mapa" desde la revisión y "Cerrar" para volver | Flujo simple e intuitivo |
| US-21 | Calidad | `MapaRevisionHtml` arma el HTML (Leaflet+OSM+atribución+pines), 100 % cubierto | Lógica del mapa verificada en el gate |

## 3. Feedback recibido

- Unificar la visualización geográfica entre web y móvil con el mismo enfoque OSM/Leaflet deja una experiencia coherente y sin credenciales.
- WebView + Leaflet evitó sumar una librería de mapa nativa (y su riesgo de warnings de vulnerabilidad), manteniendo el build móvil limpio.
- Mantener el armado del HTML en un núcleo testeable (`MapaRevisionHtml`) deja la pantalla MAUI como cáscara fina, coherente con los sprints móviles previos.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 325 verdes (288 unitarias + 37 de integración), +5 unitarias (`MapaRevisionHtml`). Cobertura del gate: Domain líneas 89,7 % / branches 79,8 %; Application 90,0 % / 82,0 %; Revision 99,2 % / 100 % (`MapaRevisionHtml` 100 % / 100 %). La pantalla MAUI queda fuera del gate y compila para `net10.0-android` (android-arm64) sin warnings.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-21-MAPA-MOVIL | Tarea | Aceptada (mapa OSM en el móvil vía WebView+Leaflet; `MapaRevisionHtml` en el gate; sin clave ni dependencia nueva) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 32 se traslada. |

Con el mapa OSM en web (S30/S31) y móvil (S32), la visualización geográfica queda unificada. Backlog restante: caché/Service Worker de teselas para offline real, y mantenimiento post-release (Dependabot/CVE por SLA, incluir libs vendorizadas en el seguimiento de versiones).

## 7. Decisiones tomadas durante el review

- WebView + Leaflet en el móvil (no control nativo ni Mapsui): sin clave, sin dependencia NuGet nueva, reusa el enfoque OSM del web.
- Mantener `MapaRevisionHtml` (armado del HTML) en `GeoVial.Revision` (gate) y la pantalla en `GeoVial.Mobile` (fuera de CI).
- Leaflet por CDN dentro del WebView: la app necesita red para el mapa de todos modos; bundlear Leaflet en la app queda como opción futura junto con la caché de teselas.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Sprint review del Sprint 32 (mapa interactivo de la revisión sobre OSM en el móvil, WebView+Leaflet, sin clave). Veredicto Cumplido, velocity 8, 0 carry-over, 325 pruebas verdes. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
