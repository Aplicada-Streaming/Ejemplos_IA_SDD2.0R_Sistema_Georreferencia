# Plan de Iteración — Sprint 27

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-27_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-06-15
**Fecha fin:** 2027-06-26
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S24–S26); capacidad sugerida estricta 9 SP. Se compromete la consolidación de las pruebas E2E de captura sobre el helper compartido `EscenarioE2E` (8 SP). Es trabajo de calidad/refactor: todo entra al gate.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Eliminar la duplicación de andamiaje en las pruebas E2E: migrar `CapturaE2ETests` al helper compartido `EscenarioE2E` (sembrado de escenario + autenticación HTTP), de modo que toda la suite E2E (captura, conflictos por radio, edición en conflicto) use el mismo sembrado autocontenido que no depende del seed de Development. Cierra la acción reiterada en las retros S24/S25/S26.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-E2E-HELPER | Tarea | Migrar `CapturaE2ETests` al helper `EscenarioE2E`; eliminar el andamiaje duplicado | Media | 8 | QA / Dev backend | Pendiente |

Total de puntos comprometidos: 8 SP. No agrega historias nuevas: consolida el andamiaje de pruebas E2E ya entregadas (Sprint 20). No toca lógica de dominio ni de aplicación.

## 4. Alcance técnico

1. **`CapturaE2ETests`** se reescribe sobre `EscenarioE2E`:
   - Reemplaza su `SembrarEscenarioAsync`/`SembrarAgenteAsync`/`Escenario`/`Clave`/`ClienteAutenticadoAsync` privados por `EscenarioE2E.SembrarAsync` y `EscenarioE2E.ClienteAutenticadoAsync`.
   - El escenario pasa a sembrar su propia área (no `db.Areas.FirstAsync()` del seed de Development), desacoplando la prueba del seed (acción de la retro del Sprint 20).
   - La prueba de autorización (`Agente_de_otra_area_no_captura`) usa un segundo escenario sembrado: su agente pertenece a otra área, por lo que no puede capturar en el relevamiento del primero (RN-01).
2. **`EscenarioE2E`** queda como único punto de sembrado/autenticación E2E; no requiere cambios funcionales (ya seca lo necesario).

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- Las tres pruebas de `CapturaE2ETests` siguen verdes con idéntica cobertura de flujo (captura georreferenciada, ubicación manual, autorización).
- `CapturaE2ETests` no contiene andamiaje propio de sembrado/autenticación: usa exclusivamente `EscenarioE2E`.
- La suite completa de integración queda verde, sin regresión ni dependencia del orden de ejecución.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| La prueba de autorización pierde cobertura al cambiar de "agente de otra área del seed" a "segundo escenario" | Baja | Medio | El segundo escenario siembra otra área distinta; el agente sigue siendo de un área ajena a la del relevamiento (RN-01) |
| Acoplamiento residual al seed de Development | Baja | Bajo | `EscenarioE2E` siembra su propia área; se elimina el `Areas.FirstAsync()` |

## 7. Criterios de hecho del sprint

El Sprint 27 se considera completo cuando `CapturaE2ETests` corre íntegramente sobre `EscenarioE2E` sin andamiaje duplicado, la suite de integración queda verde sin regresión, y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Pruebas que consolida | E2E de captura (Sprint 20): happy path, ubicación manual, autorización |
| Helper | `EscenarioE2E` (Sprint 23) |
| RN | RN-01 (autorización por área) |
| Calidad | definition-of-done §1; retros S20 (desacoplar del seed) y S24/S25/S26 (migrar `CapturaE2ETests`) |
| Tests previstos | integración: las 3 de captura, ahora sobre el helper |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 27 (consolidación E2E): migrar `CapturaE2ETests` al helper `EscenarioE2E`, eliminando el andamiaje duplicado y la dependencia del seed de Development. Compromete 8 SP; sin lógica de dominio nueva. Generado por AG-07 |
