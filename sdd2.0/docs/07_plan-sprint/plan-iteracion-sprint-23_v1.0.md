# Plan de Iteración — Sprint 23

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-23_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-04-20
**Fecha fin:** 2027-05-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S20–S22); capacidad sugerida estricta 9 SP. Se compromete el endurecimiento E2E del ciclo de conflictos (8 SP). Todo el valor es testeable y vive en el gate (pruebas de integración); cierra la deuda de E2E de las retros S18/S20.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Verificar de punta a punta por HTTP el ciclo de conflictos por radio (CU-11/CU-12): detectar marcadores en un mismo radio y resolverlos —unificar o mantener separados— sobre la API real, cerrando la cobertura E2E del flujo de conflictos que faltaba. Se extrae además un helper de escenario compartido que siembra su propia área, desacoplándose del seed (acción de la retro del Sprint 20).

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-E2E-CONF | Tarea | Helper de escenario compartido + pruebas E2E del ciclo de conflictos por radio (detectar → resolver) | Alta | 8 | QA / Dev backend | Pendiente |

Total de puntos comprometidos: 8 SP. No agrega funcionalidad: consolida el flujo de conflictos (entregado en backend en S05 y en la web en S18) con verificación E2E por HTTP.

## 4. Alcance técnico

1. **Helper de escenario compartido** (`EscenarioE2E`): siembra su **propia** área, un agente de campo con credencial y un relevamiento asignado, y autentica por HTTP; reemplaza la dependencia del área del seed de Development (retro S20).
2. **Pruebas E2E del ciclo de conflictos por radio**:
   - **Unificar**: el agente captura dos observaciones a ~33 m (con radio 15 m quedan en marcadores distintos) → se amplía el radio a 50 m → se detectan los conflictos → la lista muestra el conflicto de radio con sus dos marcadores → se unifica en uno → la lista de pendientes queda vacía.
   - **Mantener separados**: mismo escenario → se mantienen separados → ambos marcadores se conservan y el conflicto queda resuelto.
3. **Reuso de endpoints existentes**: captura, ajuste de radio (`PUT /radio`), detección (`POST /conflictos/deteccion`), listado (`GET /conflictos`) y resolución (`POST /conflictos/{id}/resolucion`); el agente está autorizado en su área (RN-01).

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- El escenario se siembra con su propia área (desacoplado del seed) y permite autenticarse por HTTP.
- Dos capturas a ~33 m con radio 15 m producen dos marcadores distintos; al ampliar el radio a 50 m, la detección registra el conflicto.
- Unificar resuelve el conflicto y vacía la lista de pendientes; mantener separados conserva ambos marcadores y resuelve el conflicto.
- Las pruebas son deterministas (identificadores únicos por escenario); el gate de cobertura se mantiene.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Dos capturas cercanas se agrupan en un marcador en lugar de dos | Media | Medio | Las capturas se hacen a ~33 m con radio 15 m (sobre el umbral), garantizando dos marcadores; el conflicto surge tras ampliar el radio |
| La autorización de detección/resolución requiere un jefe | Baja | Bajo | `PuedeAccederArea` autoriza también al agente en su área (RN-01); el ciclo lo ejecuta el agente del escenario |
| El proveedor en memoria se comparte entre pruebas | Media | Bajo | Cada escenario siembra su propia área con identificadores únicos; las pruebas consultan por sus propios ids |

## 7. Criterios de hecho del sprint

El Sprint 23 se considera completo cuando el helper de escenario compartido y las pruebas E2E del ciclo de conflictos por radio están verdes: detectar y resolver (unificar / mantener separados) se verifican de punta a punta por HTTP; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US que consolidan | US-25 (detección), US-26 (resolución), por HTTP |
| CU que consolidan | CU-11 (detección), CU-12 (resolución) |
| EP | EP-06 (Resolución de conflictos) |
| RN aplicadas | RN-02 (radio), RN-04 (consolidación), RN-01 (autorización), RN-07 (auditoría) |
| Calidad | definition-of-done §1 (pruebas E2E); retro S18/S20 (deuda de E2E de conflictos) |
| Tests previstos | integration/AT-11, AT-12 (ciclo de conflictos por radio) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 23 (endurecimiento E2E del ciclo de conflictos por radio): helper de escenario compartido (propia área) + pruebas de integración por HTTP de detectar → unificar / mantener separados. Compromete 8 SP. Cierra la deuda de E2E de conflictos de las retros S18/S20. Generado por AG-07 |
