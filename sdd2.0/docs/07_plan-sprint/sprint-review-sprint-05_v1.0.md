# Sprint Review — Sprint 05

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-05_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-05_v1.0.md`:

> Permitir que un jefe de área detecte los marcadores de un relevamiento que quedan dentro de un mismo radio configurable y resuelva esos conflictos desde la web —unificando los marcadores en uno o manteniéndolos separados— sin que el sistema unifique ni descarte nada de forma automática, sobre relevamientos no cerrados, verificado por área y auditado.

Veredicto: Cumplido.

Explicación corta: la detección por radio lista los pares cercanos y los registra como conflictos pendientes de forma idempotente, sin unificar ni descartar (RN-02); la resolución manual unifica reasignando observaciones, fotos y comentarios del marcador absorbido y eliminándolo, o mantiene ambos separados, en ambos casos levantando la marca de conflicto y auditando (RN-07); el ajuste del radio reevalúa la cercanía; y se respeta el solo-lectura sobre relevamientos cerrados (RN-05).

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-25 | Historia | Detección por radio: dos marcadores a ~1 m generan un conflicto pendiente; el ajuste del radio reevalúa la lista; la detección repetida no duplica | Detección no destructiva clara |
| US-26 | Historia | Resolución unificar: las observaciones, fotos y comentarios del absorbido pasan al resultante y el absorbido se elimina | Fusión consistente, sin huérfanos |
| US-26 | Historia | Resolución mantener separados: ambos marcadores se conservan y se levanta la marca | Decisión humana respetada |
| US-26 | Historia | Conflicto inexistente o ya resuelto responde `CONFLICTO_INEXISTENTE`; sobre cerrado, `RELEVAMIENTO_SOLO_LECTURA` | Concurrencia y bloqueo cubiertos |

## 3. Feedback recibido

- El ciclo capturar → revisar → detectar conflictos → resolver queda demostrable end-to-end por la interfaz con un jefe de área real.
- Con NB-05 (consistencia ante duplicados y conflictos) cerrada por la vía de detección por radio y resolución manual, se sugiere abordar la exportación/importación del relevamiento (EP-07) y la consolidación por sincronización (CU-07, EP-04, last-write-wins efectivo) en los próximos sprints.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 13 |
| Puntos completados | 13 |
| Velocity efectiva | 13 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 159 verdes (145 unitarias + 14 de integración), +29 respecto del Sprint 04. Cobertura: dominio 89,7 % líneas / 80,0 % branches; aplicación 87,3 % / 80,0 % (gate ≥ 80 % / ≥ 70 % cumplido). Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-25 | Historia | Aceptada |
| US-26 | Historia | Aceptada |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 05 se traslada. |

La marca de última escritura efectiva ante ediciones concurrentes (RN-04, CU-07) depende del módulo de sincronización (EP-04), no incluido en este sprint; este sprint cubre la detección por radio y la resolución manual. La cola SQLite del cliente móvil (BT-07, parte móvil) y la integración con Testcontainers continúan diferidas.

## 7. Decisiones tomadas durante el review

- Dar por cerrada la necesidad NB-05 con detección por radio y resolución manual.
- Planificar EP-07 (exportación/importación) y EP-04 (sincronización) en los próximos sprints.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Sprint review del Sprint 05 (detección por radio y resolución de conflictos). Veredicto Cumplido, velocity 13, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
