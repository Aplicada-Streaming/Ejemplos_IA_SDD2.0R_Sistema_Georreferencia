# Plan de Iteración — Sprint 10

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-10_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2026-10-13
**Fecha fin:** 2026-10-24
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (2 dev backend, 1 dev frontend, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 13,3 SP. Se comprometen 13 SP para cerrar la épica de auditoría y datos personales (EP-08), aprovechando que el registro (US-29) y la autorización transversal (US-31) ya están entregados desde sprints previos; el trabajo nuevo es la consulta del historial y el flujo de acceso a datos personales con limitación de finalidad.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 2 | 120 | 0,72 | 86 h |
| Dev frontend | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Hacer operativa la trazabilidad legal (Ley 25.326, NB-06): que el usuario raíz consulte el registro inmutable de accesos y acciones filtrando por usuario, recurso o rango de fechas dentro del período de retención, y que el acceso a los datos personales de un usuario quede acotado por rol y área, limitado a la finalidad del relevamiento y registrado, cerrando la épica de auditoría y datos personales.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-30 | Historia | Consultar el historial de auditoría con retención (rol raíz) | Alta (Must) | 5 | Dev backend A | Pendiente |
| US-31 | Historia | Autorizar el acceso a datos personales con limitación de finalidad (cierre CU-14 §5.B) | Alta (Must) | 8 | Dev backend B | Pendiente |

Total de puntos comprometidos: 13 SP. US-30 (Must, `Ready`) materializa la consulta de CU-13. US-31 ya entregó la autorización transversal por rol y área (Sprint 01, CU-14); este sprint cierra el flujo §5.B —acceso a datos personales de un usuario con limitación de finalidad (RN-08)— que faltaba, y formaliza la inmutabilidad del registro (RN-07). El registro de accesos y acciones (US-29) ya está operativo (toda acción y todo login se auditan), por lo que se da por cerrado sin trabajo adicional.

## 4. Alcance técnico

Componentes que se construyen o modifican:

1. US-30 — Consulta del historial de auditoría (CU-13 §4.4-5; RN-07, RN-01): una consulta sobre el registro inmutable, restringida al rol raíz, que filtra por autor, recurso y rango de fechas; por defecto acota al período de retención (últimos doce meses). Un usuario sin rol raíz recibe `ACCESO_NO_AUTORIZADO` y su intento queda registrado (CU-13 CA-02 lado consulta). El registro es de solo agregación: no existe ruta de modificación ni borrado, lo que satisface la inmutabilidad (RN-07); se formaliza el código `AUDITORIA_INMUTABLE` y se cubre con una prueba que verifica la ausencia de mutadores.
2. US-31 (cierre §5.B) — Acceso a datos personales con finalidad (CU-14 §5.B; RN-08): una consulta que devuelve los datos personales de un usuario solo si el solicitante puede acceder por rol y área (RN-01) y declara una finalidad permitida (los fines del relevamiento y su evaluación, RN-08); una finalidad ajena se bloquea con `FINALIDAD_NO_PERMITIDA` y un acceso fuera de alcance con `ACCESO_DATO_PERSONAL_NO_AUTORIZADO`; todo acceso queda registrado (RN-07, RN-08).

La autorización transversal por rol y área (el grueso de US-31) y el registro de accesos/acciones (US-29) ya estaban entregados; este sprint completa la consulta de auditoría y el flujo de datos personales con finalidad, cerrando EP-08 en el backend.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- El usuario raíz consulta el registro filtrando por dato y rango y obtiene los accesos dentro de los doce meses; un no-raíz recibe `ACCESO_NO_AUTORIZADO` y su intento queda registrado.
- El registro de auditoría no se puede alterar ni eliminar (inmutable por construcción).
- El acceso a datos personales respeta el alcance por rol y área y la finalidad permitida; una finalidad ajena responde `FINALIDAD_NO_PERMITIDA`; todo acceso queda registrado.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| La consulta de auditoría podría exponer datos a un rol no autorizado | Baja | Alto | Restringir la consulta al rol raíz y registrar todo intento rechazado; pruebas que verifican el bloqueo |
| La limitación de finalidad podría quedar como un control nominal | Media | Medio | Validar la finalidad declarada contra el conjunto permitido (relevamiento/evaluación) antes de devolver datos; prueba que bloquea una finalidad ajena |
| Una consulta de auditoría sin acotar podría devolver un volumen excesivo | Baja | Bajo | Acotar por defecto al período de retención (doce meses) y exponer filtros por autor, recurso y fechas |

## 7. Criterios de hecho del sprint

El Sprint 10 se considera completo cuando US-30 y el cierre §5.B de US-31 están terminados según la DoD con sus pruebas verdes; la consulta del historial restringida al raíz, la inmutabilidad del registro y el acceso a datos personales con finalidad quedan demostrados en el sprint review; y se facilitan el sprint review y la retrospectiva con sus artefactos. Con ello queda cerrada la épica EP-08.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US que avanzan | US-30 (consulta de auditoría), US-31 (cierre del acceso a datos personales §5.B) |
| CU que avanzan | CU-13 (registro y consulta de auditoría), CU-14 (autorización y acceso a datos personales) |
| NB que avanzan | NB-06 (trazabilidad y protección de datos personales) |
| RN que cierran | RN-07 (retención e inmutabilidad), RN-08 (tratamiento de datos personales), RN-01 |
| ADRs que gobiernan | ADR-14 (compliance Ley 25.326), ADR-11 (Problem Details), ADR-02 (API REST) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Plan inicial del Sprint 10 (cierre de EP-08: consulta de auditoría con retención y acceso a datos personales con finalidad). Compromete US-30 y el cierre §5.B de US-31 (13 SP). US-29 y la autorización transversal de US-31 ya estaban entregados. Generado por AG-07 |
