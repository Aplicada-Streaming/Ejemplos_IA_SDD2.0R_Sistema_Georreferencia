# Sprint Review — Sprint 18

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-18_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-18_v1.0.md`:

> Completar la resolución de conflictos de sincronización desde la web (US-26, CU-12): además de unificar o mantener separados los marcadores en un mismo radio (entregado en el Sprint 05), permitir dirimir las ediciones en conflicto (conflictos de tipo `EdicionEnConflicto` que el last-write-wins de la sincronización deja marcados, RN-04), distinguiendo el tipo de conflicto en la web y resolviendo cada uno con la acción adecuada.

Veredicto: Cumplido.

Explicación corta: el listado de conflictos pendientes ahora distingue el tipo (antes asumía que todo conflicto era de radio y llamaba `Marcadores()` para todos, lo que rompía con un conflicto de edición); para las ediciones expone el recurso. La resolución valida que la decisión corresponda al tipo: unificar/mantener separados solo para los de radio, `ConfirmarEdicion` solo para las ediciones; una decisión que no aplica responde `DECISION_CONFLICTO_INAPLICABLE`. Confirmar una edición la deja resuelta y auditada, conservando el valor consolidado por última escritura (RN-04). La pantalla web de conflictos renderiza cada tipo con su acción.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-26 | Historia | Listar conflictos pendientes con una edición en conflicto: aparece con su recurso (ya no rompe) | Listado robusto por tipo |
| US-26 | Historia | Confirmar una edición en conflicto: queda resuelta y auditada (CONFIRMAR_EDICION) | Ediciones dirimibles |
| US-26 | Historia | Aplicar una decisión que no corresponde al tipo se rechaza (`DECISION_CONFLICTO_INAPLICABLE`) | Guard correcto |
| US-26 | Historia | Resolver sobre un relevamiento cerrado responde `RELEVAMIENTO_SOLO_LECTURA` | RN-05 respetada |

## 3. Feedback recibido

- US-26 queda cerrada: la web resuelve los dos tipos de conflicto (radio y edición), cerrando el ciclo de consistencia de NB-05.
- La corrección del listado elimina un fallo latente: una edición en conflicto pendiente habría roto la consulta.
- La resolución de ediciones es deliberadamente una confirmación humana (el sistema no descarta ni reescribe automáticamente, RN-04).

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 1 (corregido: el listado rompía con ediciones en conflicto) |

Pruebas: 293 verdes (263 unitarias + 30 de integración), +4 respecto del Sprint 17 (listado con edición, confirmar edición, y las dos decisiones inaplicables). Cobertura: dominio 89,7 % líneas / 79,8 % branches; aplicación 90,0 % / 82,0 % (gate ≥ 80 % / ≥ 70 % cumplido). Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-26 | Historia | Aceptada (resolución de radio del Sprint 05 + ediciones en conflicto de este sprint; pantalla web por tipo) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 18 se traslada. |

La edición sobre el marcador desde el móvil (comentarios/etiquetas, US-15 cliente) queda en el backlog.

## 7. Decisiones tomadas durante el review

- La resolución de una edición en conflicto es una confirmación humana del valor consolidado por última escritura; no se reescribe ni descarta automáticamente.
- Validar la correspondencia decisión/tipo con un código de error específico (`DECISION_CONFLICTO_INAPLICABLE`).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 18 (cierre de US-26: resolución de ediciones en conflicto + distinción de tipos + corrección del listado). Veredicto Cumplido, velocity 8, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
