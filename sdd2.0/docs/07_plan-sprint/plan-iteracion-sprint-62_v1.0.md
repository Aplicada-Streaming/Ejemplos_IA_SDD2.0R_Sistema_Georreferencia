# Plan de Iteración — Sprint 62

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-62_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-10-16
**Fecha fin:** 2028-10-27
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas. Equipo: 4 (`equipo_n: 4`). Estimación en story points.
- Capacidad: promedio móvil 5,0 SP (S59–S61, tres UX acotados de la auditoría). Se compromete una **mejora de UX (5 SP)** que reusa el HTML de mapa y los núcleos de GPS/teselas ya en el gate; sin backend.

## 2. Objetivo del sprint

Cerrar el **núcleo** del hallazgo **H-01/H-03** de la auditoría UX móvil (el último P1 estructural): la **captura no ocurría sobre el mapa** (era un formulario en una pestaña separada del mapa) y el mapa no centraba en los datos ni tenía Centrar-GPS al alcance. wireframes-captura-movil §2 muestra la captura **sobre el mapa**, con la posición del agente y los pines. El objetivo es **embeber el mapa en la pantalla de captura** (pines del relevamiento + posición del agente + "Centrar por GPS"), encima de los controles de captura, **reusando** lo ya construido.

> Alcance: este sprint trae el mapa a la superficie de captura (contexto: pines + posición + centrar-GPS). La integración más estrecha —fijar la coordenada de la captura arrastrando el pin sobre **este** mapa— se apoya en el map-pick ya existente (S56, botón "Elegí el punto en el mapa") y queda como evolución; no se rehace el flujo de captura (EXIF/offline/encolado) ya probado.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| H-01-MAPA-CAPTURA | Historia | Embeber el mapa (pines + posición + Centrar-GPS) en la pantalla de captura | Alta | 5 | Dev móvil (AG-08) | Cerrada |

Total: 5 SP (UX; reusa `MapaRevisionHtml`, `ScriptUbicacionDispositivo` y el caché de teselas del gate, sin backend).

## 4. Alcance técnico

- **Reuso (sin cambios, en el gate):** `MapaRevisionHtml.Construir(VistaMapa)` (Leaflet + OSM + pines, centra/encarga bounds en los marcadores), `ScriptUbicacionDispositivo.Centrar` + `UbicacionDispositivo` (S60), `MapaWebViewClient` + `CacheTeselasDisco` (S38, teselas offline).
- **`CapturaPage`:** se reestructura el `Grid` a `Auto`(cinta, S59) / `Auto`(mapa, alto fijo) / `*`(scroll con los controles). En `OnAppearing` se carga el mapa una vez con los marcadores del relevamiento activo (`ClienteRevisionHttp`). Acción de toolbar **"📍 Mi ubicación"** que obtiene el GPS y recentra el mapa de la captura. El mapa es **contexto**: si no carga (sin conexión la 1ª vez), la captura sigue funcionando.
- **Sin cambios de backend/Application/Domain.** Sin registro nuevo en DI (`ClienteRevisionHttp` ya está; `CapturaPage` es transitoria).

## 5. Definition of Done aplicada

- La pantalla de captura muestra el mapa con los pines del relevamiento y permite "Centrar por GPS" en la posición del agente, encima de los controles de captura.
- El flujo de captura (tomar/elegir foto, EXIF/RN-03, map-pick S56, manual, encolado offline) sigue intacto.
- Suite del gate verde (reusa núcleos ya cubiertos; sin núcleo nuevo, como S56); cobertura DoD sin regresión.
- El MAUI compila y se verifica on-device (mapa con pines en la captura + centrado por GPS).

## 6. Riesgos y mitigaciones

| Riesgo | Prob. | Impacto | Mitigación |
| --- | --- | --- | --- |
| El mapa no carga sin conexión la 1ª vez | Media | Bajo | Es contexto: el `catch` no rompe la captura; las teselas visitadas quedan cacheadas (S38) |
| El mapa ocupa demasiado del viewport de captura | Media | Bajo | Alto fijo acotado (240); los controles quedan en el scroll debajo |
| Reentrar a la solapa recarga el mapa | Baja | Bajo | Se carga una sola vez (`_mapaCargado`) |

## 7. Criterios de hecho del sprint

Completo cuando: la captura se realiza sobre el mapa (pines del relevamiento + posición + centrar-GPS) sin romper el flujo de captura existente; la suite del gate verde; el MAUI compila y se verifica on-device; y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Auditoría UX móvil `evaluacion-ux-mobile_v1.0.md`, hallazgos **H-01/H-03** (P1, último estructural) |
| CU/UX | wireframes-captura-movil §2-§4 (captura sobre el mapa, posición y pines), experiencia §3.2 |
| Componentes | `GeoVial.Mobile` (`CapturaPage`); reuso de `GeoVial.Revision` (`MapaRevisionHtml`, `VistaMapa`, `ScriptUbicacionDispositivo`) y `MapaWebViewClient`/`CacheTeselasDisco` |
| Calidad | definition-of-done §1.4; gate Domain/Application líneas ≥80 % / ramas ≥70 % |
| Tests | Núcleos ya cubiertos (`MapaRevisionHtmlTests`, `ScriptUbicacionDispositivoTests`); este sprint es glue de UI |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-07 | Plan del Sprint 62 (H-01/H-03: mapa en la pantalla de captura). Embebe `MapaRevisionHtml` (pines + posición + centrar-GPS) en `CapturaPage`, reusando núcleos del gate (mapa, GPS de S60, teselas de S38). Sin backend. 5 SP. Generado por AG-07 |
