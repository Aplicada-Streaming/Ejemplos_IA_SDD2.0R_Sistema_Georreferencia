# Plan de Iteración — Sprint 30

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-30_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-07-27
**Fecha fin:** 2027-08-07
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S27–S29); capacidad sugerida estricta 9 SP. Se compromete el mapa interactivo de la revisión en el front web (8 SP). El núcleo testeable (cálculo de la vista del mapa) entra al gate; el render Leaflet vive en la página Blazor.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Entregar el mapa interactivo de la revisión en el front web (`GeoVial.Web`, Blazor): mostrar los marcadores georreferenciados del relevamiento sobre un mapa **Leaflet con teselas de OpenStreetMap**, con popups por marcador (coordenada, en conflicto, conteo de fotos y comentarios). **Sin clave de proveedor**: OSM es libre (solo requiere atribución). Esto desbloquea el ítem "mapa interactivo" que se venía postergando: el bloqueo real era únicamente el control de mapa **nativo** de la plataforma (Google Maps en Android), no los mapas en general.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-21-MAPA | Historia | Mapa interactivo de la revisión sobre OSM en el front web (CU-08) | Media | 8 | Dev fullstack | Pendiente |

Total de puntos comprometidos: 8 SP. Completa la visualización geográfica de la revisión (US-21), que hasta ahora se mostraba sólo como lista de marcadores por coordenada. No requiere cambios de backend ni de dominio.

## 4. Alcance técnico

1. **`GeoVial.Revision`** (en la solución y el gate):
   - `PinMapa`: proyección de un marcador para el mapa (id, lat, lon, en conflicto, nº de fotos, nº de comentarios).
   - `VistaMapa`: a partir de los marcadores de la revisión calcula la lista de pins, el centro y la caja contenedora (bounds) para encuadrar el mapa; con cero marcadores usa un centro por defecto. Lógica pura, testeable.
2. **`GeoVial.Web`** (compila en CI; el render JS no entra al gate):
   - Referencia a `GeoVial.Revision` para usar `VistaMapa`.
   - Leaflet (CSS/JS) y la hoja de OSM en `App.razor`; módulo JS `wwwroot/js/mapaRevision.js` con `render(elementId, vista)` y `destruir()` que crea el mapa Leaflet con teselas OSM (atribución incluida) y dibuja los marcadores con popup.
   - `Revision.razor`: contenedor del mapa; tras cargar la revisión, proyecta `VistaMapa` y lo envía al módulo JS por interop; lo redibuja al recargar y lo libera al disponer.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- `VistaMapa` proyecta correctamente los marcadores y calcula centro/bounds (1 marcador → centrado en él; varios → caja que los contiene; ninguno → centro por defecto).
- El front muestra el mapa OSM con un marcador por coordenada y su popup; sin ninguna clave de proveedor (atribución OSM presente).
- El núcleo `GeoVial.Revision` respeta el gate de cobertura (líneas ≥ 80 %, branches ≥ 70 %); la página Blazor queda fuera del gate.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Dependencia de CDN de Leaflet/OSM en runtime | Media | Bajo | Para demo es aceptable; si se requiere offline, se vendoriza Leaflet en `wwwroot` y se cachean teselas |
| Política de uso de teselas de OSM (volumen) | Baja | Bajo | Uso de demo/educativo dentro de la política; producción usaría un proveedor de teselas o caché (documentado) |
| Interop JS sólo tras render (InteractiveServer) | Baja | Bajo | El render del mapa se dispara en `OnAfterRenderAsync` con un flag al recargar la revisión |

## 7. Criterios de hecho del sprint

El Sprint 30 se considera completo cuando `VistaMapa` está terminado según la DoD con sus pruebas verdes; el front web muestra el mapa interactivo OSM con los marcadores de la revisión y sus popups, sin clave de proveedor; se corrige en los documentos la nota de "bloqueado por clave de proveedor de mapas" (el bloqueo era sólo el control nativo de Google); y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US | US-21 (revisión sobre mapa) — completa la visualización geográfica |
| CU | CU-08 (revisión) |
| Núcleo | `GeoVial.Revision` (`VistaMapa`, `PinMapa`) |
| Calidad | definition-of-done §1; retros S25/S27/S28/S29 (mapa interactivo, reencauzado a OSM keyless) |
| Tests previstos | unit: `VistaMapa` (centro/bounds/proyección con 0/1/N marcadores) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Plan inicial del Sprint 30 (mapa interactivo de la revisión sobre OpenStreetMap en el front web, sin clave de proveedor): núcleo `VistaMapa` en el gate + render Leaflet/OSM en Blazor. Reencauza el ítem antes marcado como "bloqueado por clave de mapas" (el bloqueo era sólo el control nativo de Google). Compromete 8 SP. Generado por AG-07 |
