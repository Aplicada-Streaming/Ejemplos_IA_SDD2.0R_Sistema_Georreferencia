# Plan de Iteración — Sprint 32

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-32_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-08-24
**Fecha fin:** 2027-09-04
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S29–S31); capacidad sugerida estricta 9 SP. Se compromete llevar el mapa interactivo OSM al móvil (8 SP). El núcleo testeable (HTML del mapa) entra al gate; la pantalla MAUI queda fuera de CI.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Llevar el mapa interactivo de la revisión a la app móvil (`GeoVial.Mobile`, MAUI): mostrar los marcadores georreferenciados sobre un mapa **Leaflet con teselas de OpenStreetMap** dentro de un **WebView**, **sin clave de proveedor** (se evita el control de mapa nativo de Google y, con WebView+Leaflet, no se agrega una dependencia NuGet que arriesgue warnings de vulnerabilidad bajo `TreatWarningsAsErrors`). Reusa `VistaMapa` (ya en el gate) y unifica la visualización geográfica entre web y móvil. Cierra la acción reiterada de las retros S30/S31.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-21-MAPA-MOVIL | Tarea | Mapa interactivo de la revisión sobre OSM en la app móvil (WebView + Leaflet) | Media | 8 | Dev móvil | Pendiente |

Total de puntos comprometidos: 8 SP. Extiende US-21 al móvil; no requiere backend ni dominio nuevos.

## 4. Alcance técnico

1. **`GeoVial.Revision`** (en la solución y el gate):
   - `MapaRevisionHtml.Construir(VistaMapa)`: arma el documento HTML autocontenido del mapa (Leaflet + teselas OSM con atribución + los marcadores de la vista como JSON, con popup por marcador y encuadre a punto o a bounds). Lógica pura, testeable; reusa `VistaMapa`.
2. **`GeoVial.Mobile`** (fuera de CI; compila android-arm64):
   - `MapaRevisionPage`: `ContentPage` con un `WebView` que carga el HTML; toolbar "Cerrar".
   - `RevisionPage`: guarda la revisión cargada y agrega un botón "Ver en mapa" que abre `MapaRevisionPage` (modal) con `MapaRevisionHtml.Construir(new VistaMapa(revision.Marcadores))`.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- `MapaRevisionHtml` produce un HTML con Leaflet + teselas OSM (con atribución) y los marcadores de la vista; sin clave de proveedor; sin tokens sin reemplazar.
- La pantalla móvil abre el mapa con los marcadores del relevamiento cargado y permite cerrarlo.
- El núcleo `GeoVial.Revision` respeta el gate de cobertura (líneas ≥ 80 %, branches ≥ 70 %); la pantalla MAUI queda fuera del gate y compila para `net10.0-android` (android-arm64).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Una lib de mapa nativa (Mapsui) agrega warnings de vulnerabilidad bajo `TreatWarningsAsErrors` | Media | Medio | Se usa WebView + Leaflet (sin dependencia NuGet nueva), reusando el enfoque del front web |
| El WebView necesita red para Leaflet/teselas | Media | Bajo | La app necesita red para el mapa de todas formas; degradación elegante si no hay red |
| Escapado de llaves al armar el HTML en C# | Baja | Bajo | Plantilla con tokens (`__PIN__`) reemplazados por `String.Replace`, sin interpolación |

## 7. Criterios de hecho del sprint

El Sprint 32 se considera completo cuando `MapaRevisionHtml` está terminado según la DoD con sus pruebas verdes; la app móvil abre el mapa interactivo OSM con los marcadores de la revisión y permite cerrarlo; la pantalla MAUI compila para android-arm64; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US | US-21 (revisión sobre mapa) — extiende la visualización geográfica al móvil |
| CU | CU-08 (revisión) |
| Núcleo | `GeoVial.Revision` (`MapaRevisionHtml`, reusa `VistaMapa`) |
| Calidad | definition-of-done §1; retros S30/S31 (mapa OSM al móvil) |
| Tests previstos | unit: `MapaRevisionHtml` (vacío/1/varios, atribución, pines, vista nula) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Plan inicial del Sprint 32 (mapa interactivo de la revisión sobre OpenStreetMap en el móvil, WebView + Leaflet, sin clave): núcleo `MapaRevisionHtml` en el gate + pantalla MAUI fuera de CI. Cierra la acción de S30/S31. Compromete 8 SP. Generado por AG-07 |
