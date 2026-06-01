# Plan de Iteración — Sprint 05

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-05_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2026-08-04
**Fecha fin:** 2026-08-15
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (2 dev backend, 1 dev frontend, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 26,3 SP; tope del 110 % = 29 SP. Se comprometen 13 SP. El margen respecto del tope se reserva por la complejidad de la fusión de marcadores (reasignación de observaciones, fotos y comentarios) y por ser un módulo con efectos estructurales sobre datos consolidados.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 2 | 120 | 0,72 | 86 h |
| Dev frontend | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Permitir que un jefe de área detecte los marcadores de un relevamiento que quedan dentro de un mismo radio configurable y resuelva esos conflictos desde la web —unificando los marcadores en uno o manteniéndolos separados— sin que el sistema unifique ni descarte nada de forma automática, sobre relevamientos no cerrados, verificado por área y auditado.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-25 | Historia | Detectar marcadores en conflicto por radio configurable | Media | 5 | Dev backend A | Pendiente |
| US-26 | Historia | Resolver conflictos de sincronización desde la web | Media | 8 | Dev backend B / Dev frontend | Pendiente |

Total de puntos comprometidos: 13 SP. US-25 y US-26 (Should de EP-06) se refinaron a `Ready`: completan la necesidad NB-05 de consistencia ante conflictos y se apoyan en la lógica de marcadores y radio ya construida (Sprint 03).

## 4. Alcance técnico

Componentes que se construyen o modifican (sobre la arquitectura de 05, sin redefinirla):

1. US-25 — Detección por radio (CU-11): un command que, sobre los marcadores consolidados de un relevamiento, identifica los pares que quedan a una distancia menor que el radio de agrupación (RN-02), los registra como conflictos pendientes (entidad ConflictoSync, modelo-datos-logico §1.11) sin unificarlos ni descartarlos, y los lista. Incluye el ajuste del radio del relevamiento (CU-11 §5.A), que reevalúa la cercanía. Depende de la persistencia de marcadores (Sprint 03).
2. US-26 — Resolución desde la web (CU-12): un command de resolución por decisión humana. Unificar reasigna las observaciones, fotos y comentarios del marcador absorbido al marcador resultante y elimina el absorbido (RN-02, CU-12 §5.A); mantener separados conserva ambos marcadores. En ambos casos levanta la marca de conflicto, deja la base consistente y audita (RN-07). Rechaza sobre relevamiento cerrado (RN-05) y conflictos inexistentes (CU-12 CA-03 / concurrencia).

El sistema nunca unifica ni descarta marcadores de forma automática (RN-02): toda fusión es una decisión humana. La marca de última escritura en ediciones concurrentes (RN-04) proviene de la sincronización (CU-07, EP-04, no incluida en este sprint); este sprint cubre la detección por radio y la resolución manual.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- La detección lista los marcadores dentro del radio sin unificarlos ni descartarlos (RN-02); el ajuste del radio reevalúa la lista.
- La resolución es siempre una decisión humana; unificar reasigna el contenido del marcador absorbido y lo elimina; mantener separados conserva ambos.
- La resolución sobre un relevamiento cerrado se rechaza (RN-05); un conflicto ya resuelto responde `CONFLICTO_INEXISTENTE`.
- El ciclo capturar → revisar → detectar conflictos → resolver queda demostrable end-to-end en el sprint review.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| La fusión de marcadores puede dejar observaciones o fotos huérfanas si la reasignación no es completa | Media | Alto | Reasignar observaciones, fotos y comentarios del marcador absorbido en una sola transacción; pruebas que verifican el conteo tras la unificación |
| Una detección repetida podría duplicar los conflictos persistidos | Media | Medio | Detección idempotente: no se crea un conflicto pendiente si ya existe uno para el mismo par de marcadores |
| La resolución concurrente del mismo conflicto puede aplicar dos veces la decisión | Baja | Medio | La primera resolución marca el conflicto como resuelto; la segunda recibe `CONFLICTO_INEXISTENTE` (CU-12 §13) |

## 7. Criterios de hecho del sprint

El Sprint 05 se considera completo cuando US-25 y US-26 están terminadas según la DoD con sus pruebas verdes; la detección por radio y la resolución (unificar y mantener separados) quedan demostradas end-to-end en el sprint review; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU que avanzan | CU-11 (detección y listado de marcadores en conflicto por radio: US-25), CU-12 (resolución de conflictos desde la web: US-26) |
| NB que avanzan | NB-05 (consistencia de datos ante duplicados y conflictos) |
| ADRs que gobiernan | ADR-06 (last-write-wins con override manual), ADR-04 (mapas OSM + Leaflet), ADR-01 (CQRS ligero), ADR-09 (persistencia EF Core) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Plan inicial del Sprint 05 (resolución de conflictos). Compromete US-25 y US-26 (13 SP). Cierra la necesidad NB-05 con detección por radio y resolución manual; la consolidación por sincronización (RN-04) llega con EP-04. Generado por AG-07 |
