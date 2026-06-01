# Sprint Review — Sprint 04

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-04_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-04_v1.0.md`:

> Cerrar el ciclo de revisión: que un jefe de área revise un relevamiento sobre el mapa recorriendo sus marcadores con sus observaciones, fotos, comentarios y etiquetas, pueda comentar y etiquetar el contenido del marcador respetando el bloqueo de solo lectura, y que el equipo disponga de la provisión de credenciales para operar la jerarquía completa por la interfaz.

Veredicto: Cumplido.

Explicación corta: la revisión devuelve los marcadores con su contenido consolidado, el comentado y etiquetado respeta el solo-lectura (RN-05) y las cardinalidades (RC-04), y la provisión de credenciales habilita el inicio de sesión de roles distintos del raíz, resolviendo la deuda recurrente de las retros.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| BT-23 | Backlog técnico | Un jefe general provee credencial a un jefe de área; el jefe de área inicia sesión | Demos por UI desbloqueadas |
| US-21 | Historia | Revisión sobre mapa: marcadores con sus fotos, comentarios, etiquetas y la bandeja sin georreferenciar | Vista consolidada clara |
| US-15 | Historia | Comentar y etiquetar el contenido del marcador; rechazo sobre relevamiento cerrado | Cardinalidades y solo-lectura respetados |
| US-22 | Historia | Recorrido de los marcadores y sus fotos desde el front | Navegación funcional |

## 3. Feedback recibido

- Con la provisión de credenciales operativa, se puede recorrer el ciclo completo capturar → revisar → comentar por la interfaz con un jefe de área real.
- Se sugiere abordar la exportación/importación del relevamiento (CU-08 §5.A/§5.B, EP-07) y la resolución de conflictos (EP-06) en los próximos sprints.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 24 |
| Puntos completados | 24 |
| Velocity efectiva | 24 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 130 verdes (120 unitarias + 10 de integración), +23 respecto del Sprint 03. Cobertura: dominio 88,3 % líneas / 78,8 % branches; aplicación 83,0 % / 74,8 % (gate ≥ 80 % / ≥ 70 % cumplido). Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-23 | Backlog técnico | Aceptada |
| US-21 | Historia | Aceptada |
| US-15 | Historia | Aceptada (acotada a comentarios y etiquetas; alta/baja de fotos se nutre de la captura del Sprint 03) |
| US-22 | Historia | Aceptada |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 04 se traslada. |

BT-07 alcanzó las 12 entidades backend del modelo lógico (se sumaron Comentario, Etiqueta y las uniones FotoEtiqueta/ComentarioEtiqueta). La exportación/importación (CU-08 §5.A/§5.B), el filtrado por etiquetas (US-23), el visor a pantalla completa (US-24) y la integración con Testcontainers quedan para sprints posteriores.

## 7. Decisiones tomadas durante el review

- Planificar EP-06 (resolución de conflictos) y EP-07 (exportación/importación) en los próximos sprints.
- Cerrar la deuda de retro de provisión de credenciales como Completada.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Sprint review del Sprint 04 (revisión sobre mapa, comentarios/etiquetas y provisión de credenciales). Veredicto Cumplido, velocity 24, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
