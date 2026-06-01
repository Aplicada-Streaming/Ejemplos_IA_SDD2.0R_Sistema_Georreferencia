# Plan de Iteración — Sprint 02

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-02_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha inicio:** 2026-06-23
**Fecha fin:** 2026-07-04
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles), duración estándar (§3.2 de las reglas).
- Equipo: 4 integrantes (2 dev backend, 1 dev frontend, 1 QA part-time), conforme `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci), heredada de 06; no se re-estima en el sprint.
- Capacidad declarada: 42 story points (igual que S01; se mantiene el factor de focus conservador hasta consolidar el promedio móvil en S03). Comprometidos: 27 SP, por debajo del tope del 110 %.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 2 | 120 | 0,70 | 84 h |
| Dev frontend | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Entregar el slice de relevamientos sobre backend, front web y base de datos, de modo que un jefe de área cree relevamientos de su área con su radio de agrupación, asigne y reasigne agentes de su propia área, y haga avanzar cada relevamiento por sus estados recolección → revisión → cierre con reapertura explícita, todo verificado por rol y área y registrado en auditoría.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-10 | Backlog técnico | Entidad Relevamiento con radio de agrupación y seed inicial | Alta | 3 | Dev backend A | Pendiente |
| BT-03 | Backlog técnico | Módulo de relevamientos y asignación (CQRS ligero) | Alta | 5 | Dev backend A | Pendiente |
| US-06 | Historia | Crear relevamiento con radio de agrupación | Alta | 5 | Dev backend B | Pendiente |
| US-07 | Historia | Asignar agentes al relevamiento | Alta | 3 | Dev backend B | Pendiente |
| US-08 | Historia | Reasignar agentes de un relevamiento | Media | 3 | Dev backend B | Pendiente |
| US-09 | Historia | Transicionar estados del relevamiento | Alta | 5 | Dev backend A / Dev frontend | Pendiente |
| US-10 | Historia | Reabrir un relevamiento cerrado | Media | 3 | Dev frontend | Pendiente |

Total de puntos comprometidos: 27 SP. Las US-08 y US-10 (Should) se refinaron a `Ready` en la sesión de refinamiento previa: comparten dominio y validaciones con US-06/US-07/US-09 y no agregan riesgo nuevo.

## 4. Alcance técnico

Componentes que se construyen o modifican, en orden de dependencias (sobre la arquitectura de 05, sin redefinirla):

1. BT-10 — Entidad de dominio `Relevamiento` con identificación de obra, estado (recolección/revisión/cierre, RC-06) y radio de agrupación positivo (RN-02), más `AsignacionAgente`; mapeo EF Core y migración (avanza BT-07 sobre el modelo lógico §1.3/§1.4). Depende de BT-07 (slice del Sprint 00/01).
2. BT-03 — Módulo de relevamientos con CQRS ligero vía mediador (ADR-01, PROJECT-README §3): commands de creación, asignación, reasignación y transición de estado, y query de listado por área. Valida pertenencia de área (RN-01) y audita cada acción (RN-07). Depende de BT-10.
3. US-06 y US-07 — Sobre el módulo de relevamientos: alta de relevamiento en estado recolección con su radio (CU-01 CA-01) y asignación de agentes del área del jefe, rechazando agentes de otra área con `AGENTE_FUERA_DE_AREA` (CU-01 CA-02).
4. US-08 — Reasignación de agentes sobre un relevamiento en recolección o revisión (CU-01 §5.A), conservando lo recolectado.
5. US-09 y US-10 — Transiciones recolección → revisión → cierre con bloqueo de solo lectura tras el cierre y reapertura explícita cerrado → recolección (CU-10, RN-05); transición inválida con `TRANSICION_INVALIDA`, reapertura sin acción explícita con `REAPERTURA_NO_AUTORIZADA`.

Dependencias inter-BT: BT-03 depende de BT-10; ambas se apoyan en la persistencia y la autorización entregadas en S00/S01. El slice incluye front web Blazor (alta, asignación y transición) y pruebas unitarias y de integración.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica del proyecto (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Este plan no la redefine.

Criterios específicos del Sprint 02:

- El módulo de relevamientos usa CQRS ligero (commands/queries vía mediador), conforme ADR-01 y PROJECT-README §3.
- Cada transición de estado y cada asignación queda auditada (RN-07) y verificada por rol y área (RN-01).
- El radio de agrupación persiste y se valida positivo (RN-02); el relevamiento cerrado queda de solo lectura (RN-05).
- El slice queda demostrable end-to-end (crear → asignar → transicionar → cerrar → reabrir) en el sprint review.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| La introducción del mediador CQRS agrega complejidad si se sobre-ingenieriza para un módulo acotado | Media | Medio | Mediador mínimo (petición → manejador) resuelto por inyección de dependencias; sin librería externa; revisión del arquitecto (AG-05) sobre la frontera command/query |
| La máquina de estados (RN-05) puede permitir transiciones inválidas si la validación no es exhaustiva | Media | Alto | Batería de pruebas unitarias de todas las transiciones válidas e inválidas y de la reapertura explícita antes de exponer la API |
| El mapeo EF de la colección de asignaciones del agregado Relevamiento puede arrastrar problemas de carga | Media | Medio | Cargar el agregado con sus asignaciones explícitamente; prueba de integración de creación + asignación; sin lazy loading |

## 7. Criterios de hecho del sprint

El Sprint 02 se considera completo cuando todas las US y BT comprometidas están en estado terminado según la DoD canónica con sus pruebas verdes; el slice de relevamientos (crear, asignar, reasignar, transicionar, reabrir) queda demostrado end-to-end en el sprint review sobre el entorno de prueba; y se facilitan el sprint review y la retrospectiva con sus artefactos completados.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU que avanzan | CU-01 (crear relevamiento y asignar/reasignar agentes: US-06, US-07, US-08), CU-10 (transicionar estados y reabrir: US-09, US-10) |
| NB que avanzan | NB-01 (organización del trabajo de recolección por área), NB-04 (revisión centralizada: el ciclo de estados ordena recolección y evaluación) |
| ADRs que gobiernan | ADR-01 (monolito modular + CQRS ligero), ADR-09 (persistencia SQL Server + EF Core), ADR-14 (compliance Ley 25.326), ADR-10 (separación de capas) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Plan inicial del Sprint 02 (slice de relevamientos y asignación). Compromete US-06, US-07, US-08, US-09, US-10 y las BT de soporte BT-10 y BT-03 (27 SP). Generado por AG-07 |
