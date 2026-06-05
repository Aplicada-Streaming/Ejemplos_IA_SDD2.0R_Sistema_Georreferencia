# Plan de Iteración — Sprint 50

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-50_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-05-01
**Fecha fin:** 2028-05-12
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S47–S49); capacidad sugerida estricta 9 SP. Se compromete la **bandeja sin georreferenciar visible y navegable en la app** (8 SP), gap de la revisión funcional. Núcleo (enriquecimiento del DTO + presenter) en el gate; la **colocación en el mapa desde la app** queda explícitamente para un sprint siguiente.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Las fotos sin GPS van a la **bandeja sin georreferenciar** (RN-03) a la espera de ubicación manual (CU-05), pero la app no las mostraba: el agente no tenía forma de saber qué quedó sin ubicar. Además, el backend sólo exponía los **IDs** de esas observaciones (sin momento ni foto), insuficiente para mostrarlas. El objetivo es **enriquecer** la bandeja en la revisión (momento + referencia de foto) y **mostrarla** en una solapa de la app, ordenada y legible. La colocación efectiva en el mapa desde la app se planifica aparte.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-BANDEJA-VER | Historia | Solapa "Bandeja": el agente ve las observaciones del relevamiento activo que esperan ubicación, con momento y foto | Alta | 4 | Dev móvil (AG-08) | Cerrada |
| BT-BANDEJA-DTO | Tarea | Enriquecer la revisión con una bandeja (id + momento + referencia de foto), de forma aditiva | Alta | 2 | Dev backend (AG-06) | Cerrada |
| BT-BANDEJA-NUCLEO | Tarea | `PresentadorBandeja` (ordena por momento, etiqueta legible) en `GeoVial.Revision`, cubierto en el gate | Alta | 2 | Dev fullstack (AG-09) | Cerrada |

Total de puntos comprometidos: 8 SP. Núcleo en `GeoVial.Application`/`GeoVial.Revision` (en el gate); UI en `GeoVial.Mobile`.

## 4. Alcance técnico

1. **Backend (en el gate), aditivo:** `RevisionRelevamiento` (y el DTO `RevisionRelevamientoDto`) ganan un campo `Bandeja: IReadOnlyList<ObservacionSinGeo(Dto)>` con `ObservacionId + MomentoCaptura + ReferenciaArchivo`. Se **conserva** `ObservacionesSinGeorreferenciar` (sólo IDs) por compatibilidad; el campo nuevo es opcional al final del DTO para no romper consumidores/constructores previos. El handler de revisión puebla la bandeja resolviendo la foto de cada observación sin georreferenciar; el filtro por etiquetas la vacía (consistente con el listado de IDs).
2. **Núcleo (en el gate):** `PresentadorBandeja.Presentar(bandeja)` ordena las entradas de la más reciente a la más antigua y arma una etiqueta legible (`"Sin ubicar · dd/MM HH:mm · nombreArchivo"`, "(sin foto)" si no hay). Función estática sin dependencias de plataforma (en `GeoVial.Revision`).
3. **Móvil:** solapa "Bandeja" (`BandejaPage`) que trae la revisión del relevamiento activo, presenta las filas y muestra el conteo; si está vacía, lo indica. La **ubicación manual desde la app** (tap en el mapa) queda fuera de alcance: hoy la resolución sigue siendo por la web/revisión.

## 5. Definition of Done aplicada

- La revisión expone la bandeja enriquecida (id + momento + foto); la app la muestra ordenada y legible para el relevamiento activo.
- Núcleo (enriquecimiento del handler + presenter) cubierto por pruebas en el gate; el cambio es aditivo y no rompe consumidores ni tests previos.
- Cobertura DoD respetada (Domain/Application líneas ≥80 %, ramas ≥70 %); `GeoVial.Revision` bien cubierto.
- El MAUI compila para `net10.0-android` con la nueva solapa.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Cambiar el DTO de revisión rompe consumidores (web/tests) | Media | Medio | Campo `Bandeja` **aditivo y opcional** (default null); se conserva `ObservacionesSinGeorreferenciar` |
| El agente espera resolver desde la app y sólo puede ver | Media | Medio | Alcance explícito: ver ahora, ubicar-en-mapa en sprint siguiente; la UI lo aclara ("se ubican en la web/revisión") |
| Una observación sin foto rompe el armado de la etiqueta | Baja | Bajo | `ReferenciaArchivo` nullable; el presenter muestra "(sin foto)" |

## 7. Criterios de hecho del sprint

El Sprint 50 se considera completo cuando: la revisión enriquece la bandeja, la app la muestra ordenada para el relevamiento activo, el núcleo está cubierto en el gate sin romper lo previo, la cobertura DoD se mantiene, el MAUI compila, y se facilitan el sprint review y la retrospectiva.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | `revision-funcional-app_v1.0.md` (backlog: bandeja sin georreferenciar navegable) |
| CU | CU-08 (revisión), CU-05 (ubicación manual); RN-03 (fuente de ubicación) |
| Componentes | `GeoVial.Application` (`Revision`); `GeoVial.Shared`; `GeoVial.Api`; `GeoVial.Revision` (`PresentadorBandeja`); `GeoVial.Mobile` (`BandejaPage`) |
| Calidad | definition-of-done §1.4 (cobertura) |
| Tests previstos | `PresentadorBandejaTests` (orden, etiqueta con/sin foto, vacía) + handler (bandeja enriquecida) + E2E (bandeja poblada antes de ubicar, vacía después) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Plan del Sprint 50 (bandeja sin georreferenciar visible y navegable): enriquecimiento aditivo de la revisión (id + momento + foto) + `PresentadorBandeja` en el gate + solapa `BandejaPage`. La colocación en mapa desde la app queda para S51. Compromete 8 SP. Generado por AG-07 |
