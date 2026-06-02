# Plan de Iteración — Sprint 24

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-24_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-05-04
**Fecha fin:** 2027-05-15
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S21–S23); capacidad sugerida estricta 9 SP. Se compromete el endurecimiento E2E del ciclo de edición en conflicto (8 SP). Todo el valor es testeable y vive en el gate; cierra la última deuda de E2E (retros S18/S20/S23) del flujo de sincronización con colisión.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Verificar de punta a punta por HTTP el ciclo de edición en conflicto (CU-07/CU-12): sincronizar un comentario y luego ediciones del mismo recurso que colisionan (el last-write-wins consolida la última escritura y marca `EdicionEnConflicto`, RN-04), listar el conflicto y confirmarlo (`ConfirmarEdicion`). Completa la cobertura E2E del flujo de conflictos, que ya cubría el conflicto por radio (Sprint 23).

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-E2E-EDIT | Tarea | E2E del ciclo de edición en conflicto por HTTP (sync con colisión → listar → confirmar) | Alta | 8 | QA / Dev backend | Pendiente |

Total de puntos comprometidos: 8 SP. No agrega funcionalidad: consolida el flujo de edición en conflicto (sincronización del Sprint 09, resolución del Sprint 18) con verificación E2E por HTTP, reusando el helper de escenario compartido.

## 4. Alcance técnico

1. **Helper de escenario** (`EscenarioE2E`): se extiende para devolver el identificador del agente, necesario como autor de los comentarios sincronizados.
2. **Prueba E2E del ciclo de edición en conflicto**:
   - El agente captura una observación → obtiene un marcador.
   - Sincroniza (`POST /relevamientos/{id}/sync`) una **creación** de comentario (marca T0) → confirmado, sin conflictos.
   - Sincroniza una **edición** del mismo comentario (T1 > T0) → confirmada, sin conflictos (aún no había edición previa que compita).
   - Sincroniza una **segunda edición** (T2) → el comentario ya estaba editado: el last-write-wins consolida y marca `EdicionEnConflicto` (RN-04); la respuesta de sync incluye el conflicto.
   - Lista los conflictos pendientes (`GET /conflictos`) → aparece el conflicto de edición (tipo edición, con su recurso).
   - Confirma la edición (`POST /conflictos/{id}/resolucion`, decisión confirmar) → resuelto; la lista queda sin ese conflicto.
3. **Reuso de endpoints existentes**: captura, sincronización, listado y resolución de conflictos; el agente está autorizado en su área (RN-01).

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- Una creación seguida de dos ediciones del mismo comentario por sincronización produce un `EdicionEnConflicto`.
- La sincronización es idempotente: reintentar un `CambioId` ya aplicado se confirma sin duplicar (verificado en la secuencia).
- El conflicto de edición se lista y se confirma; tras confirmarlo no queda pendiente.
- Las pruebas son deterministas (identificadores y marcas temporales fijas); el gate de cobertura se mantiene.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El conflicto de edición requiere un orden preciso de operaciones | Media | Medio | La marca de conflicto se dispara en la segunda edición (cuando el comentario ya estaba editado); la prueba envía las tres operaciones con marcas temporales crecientes |
| La detección por radio podría interferir en el listado | Baja | Bajo | El escenario tiene un único marcador; el listado solo muestra el conflicto de edición |
| El proveedor en memoria se comparte entre pruebas | Media | Bajo | El escenario siembra su propia área con identificadores únicos |

## 7. Criterios de hecho del sprint

El Sprint 24 se considera completo cuando la prueba E2E del ciclo de edición en conflicto está verde: sincronizar con colisión produce el `EdicionEnConflicto`, que se lista y se confirma por HTTP; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US que consolidan | US-18 (sincronización), US-26 (resolución de ediciones), por HTTP |
| CU que consolidan | CU-07 (sincronización), CU-12 (resolución) |
| EP | EP-04 (sincronización) / EP-06 (resolución) |
| RN aplicadas | RN-04 (last-write-wins + conflicto), RN-01 (autorización), RN-07 (auditoría) |
| Calidad | definition-of-done §1 (pruebas E2E); retro S18/S20/S23 (deuda de E2E de edición en conflicto) |
| Tests previstos | integration/AT-07, AT-12 (ciclo de edición en conflicto) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 24 (endurecimiento E2E del ciclo de edición en conflicto): sincronizar con colisión → listar → confirmar, por HTTP, reusando el helper de escenario. Compromete 8 SP. Cierra la última deuda de E2E de conflictos. Generado por AG-07 |
