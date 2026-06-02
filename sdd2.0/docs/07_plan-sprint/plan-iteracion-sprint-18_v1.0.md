# Plan de Iteración — Sprint 18

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-18_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-02-09
**Fecha fin:** 2027-02-20
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 9,7 SP (S15–S17); capacidad sugerida estricta 11 SP. Se compromete US-26 (8 SP), cerrando EP-06. Es trabajo de backend (dominio/aplicación, dentro del gate de cobertura) + web (Blazor, fuera del gate de cobertura como el resto de la UI).

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Completar la resolución de conflictos de sincronización desde la web (US-26, CU-12): además de unificar o mantener separados los marcadores en un mismo radio (entregado en el Sprint 05), permitir **dirimir las ediciones en conflicto** (conflictos de tipo `EdicionEnConflicto` que el last-write-wins de la sincronización deja marcados, RN-04), distinguiendo el tipo de conflicto en la web y resolviendo cada uno con la acción adecuada.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-26 | Historia | Resolver conflictos de sincronización desde la web (radio + ediciones) | Should | 8 | Dev fullstack / Dev backend | Pendiente |

Total de puntos comprometidos: 8 SP. El Sprint 05 entregó la detección y la resolución por radio; este sprint cierra US-26 añadiendo la resolución de las ediciones en conflicto y la distinción de tipos, y corrige que el listado de pendientes asumía que todo conflicto era de radio.

## 4. Alcance técnico

1. **Dominio**: `ConflictoSync.Recurso()` devuelve el recurso de un conflicto de edición (paralelo a `Marcadores()` para los de radio). Nuevo código de error `DECISION_CONFLICTO_INAPLICABLE` para una decisión que no corresponde al tipo de conflicto.
2. **Aplicación**:
   - `DecisionConflicto.ConfirmarEdicion`: la decisión humana de dar por dirimida una edición en conflicto (el valor consolidado por última escritura prevalece; RN-04).
   - `ConflictosPendientesHandler`: se corrige para no asumir que todo conflicto es de radio (antes llamaba `Marcadores()` para todos, lo que rompía con un conflicto de edición) y expone el recurso de las ediciones en conflicto.
   - `ResolverConflictoHandler`: valida que la decisión corresponda al tipo (unificar/mantener separados solo para radio; confirmar edición solo para edición), resuelve la edición levantando la marca y auditando (RN-07).
3. **Contrato y API**: `ConflictoPendienteDto` lleva el recurso y marcadores opcionales según el tipo; el endpoint de listado mapea ambos tipos.
4. **Web (Blazor)**: la pantalla de conflictos distingue el tipo: para los de radio mantiene unificar/mantener separados; para las ediciones en conflicto muestra el recurso y permite confirmar la resolución.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- El listado de conflictos pendientes incluye los de edición en conflicto sin romper (antes lanzaba excepción).
- Una edición en conflicto se resuelve con `ConfirmarEdicion`: queda resuelta y auditada; el valor consolidado por última escritura se mantiene (RN-04).
- Aplicar una decisión que no corresponde al tipo (unificar a una edición, o confirmar a un conflicto de radio) responde `DECISION_CONFLICTO_INAPLICABLE`.
- Resolver sobre un relevamiento cerrado responde `RELEVAMIENTO_SOLO_LECTURA` (US-26 CA-03).
- El backend respeta el gate de cobertura (dominio/aplicación líneas ≥ 80 %, branches ≥ 70 %); la pantalla web queda fuera del gate.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El cambio de forma de `ConflictoPendienteDto` rompe consumidores | Baja | Bajo | Campos nuevos opcionales (marcadores nullable + recurso); las pruebas existentes siguen verdes |
| Generar un conflicto de edición real para probar requiere un escenario de sync completo | Media | Bajo | Las pruebas construyen el `ConflictoSync.EdicionEnConflicto` directamente y verifican el listado y la resolución a nivel handler |
| La pantalla web no entra al gate de cobertura | — | — | Es UI (Blazor); el valor verificable vive en dominio/aplicación, como en el resto del proyecto |

## 7. Criterios de hecho del sprint

El Sprint 18 se considera completo cuando US-26 está terminada según la DoD con sus pruebas verdes: el listado de pendientes maneja ambos tipos de conflicto, las ediciones en conflicto se resuelven con `ConfirmarEdicion` y la decisión inaplicable se rechaza; la pantalla web distingue el tipo y ofrece la acción adecuada; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US que avanzan | US-26 (resolver conflictos desde la web: radio + ediciones) |
| CU que avanzan | CU-12 (resolución de conflictos) |
| EP | EP-06 (Resolución de conflictos) |
| NB que avanzan | NB-05 (consistencia por resolución web) |
| RN aplicadas | RN-04 (last-write-wins), RN-02 (radio), RN-05 (solo lectura), RN-07 (auditoría), RN-01 (autorización) |
| BT derivadas | BT-07, BT-09, BT-17, BT-18 |
| Tests previstos | acceptance/AT-12-resolucion-conflictos |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 18 (cierre de US-26: resolución de ediciones en conflicto desde la web + distinción de tipos + corrección del listado). Compromete US-26 (8 SP), cerrando EP-06. Generado por AG-07 |
