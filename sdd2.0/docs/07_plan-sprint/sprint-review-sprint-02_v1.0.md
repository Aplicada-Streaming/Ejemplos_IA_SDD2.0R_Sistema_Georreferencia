# Sprint Review — Sprint 02

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-02_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-02_v1.0.md`:

> Entregar el slice de relevamientos sobre backend, front web y base de datos, de modo que un jefe de área cree relevamientos de su área con su radio de agrupación, asigne y reasigne agentes de su propia área, y haga avanzar cada relevamiento por sus estados recolección → revisión → cierre con reapertura explícita, todo verificado por rol y área y registrado en auditoría.

Veredicto: Cumplido.

Explicación corta: las 5 historias y 2 BT comprometidas quedaron implementadas y verificadas; el módulo de relevamientos usa CQRS ligero (mediador) y la máquina de estados (RN-05) y el radio (RN-02) están cubiertos por pruebas. El agregado Relevamiento persiste con sus asignaciones.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-06 | Historia | Creación de un relevamiento "Puente Río 12" en recolección con radio 15 m | Alta correcta; el radio queda persistido |
| US-07 | Historia | Asignación de agentes del área del jefe y rechazo de agente de otra área (AGENTE_FUERA_DE_AREA) | Acotamiento por área respetado |
| US-08 | Historia | Reasignación: el conjunto vigente se ajusta conservando lo recolectado | Reasignación sin pérdida de datos |
| US-09 | Historia | Transición recolección → revisión → cierre con bloqueo de solo lectura | Ciclo de estados claro; cierre protege la evaluación |
| US-10 | Historia | Reapertura explícita de un relevamiento cerrado | Reapertura deliberada, auditada |
| BT-10 | Backlog técnico | Entidad Relevamiento con radio y persistencia (migración EF) | — |
| BT-03 | Backlog técnico | Módulo de relevamientos con CQRS ligero (commands/queries vía mediador) | Frontera command/query validada por el arquitecto |

## 3. Feedback recibido

- Para demostrar el alta de relevamientos desde el front se necesita un jefe de área con credencial; hoy el seed solo provee el usuario raíz. Se sugiere una US de provisión de credenciales o seed de prueba para el próximo sprint.
- Confirmar si la reasignación debe notificar a los agentes removidos (hoy solo conserva sus observaciones).

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 27 |
| Puntos completados | 27 |
| Velocity efectiva | 27 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 91 verdes (83 unitarias + 8 de integración), +28 respecto del Sprint 01. Cobertura: dominio 89,4 % líneas / 78,9 % branches; aplicación 81,9 % / 76,1 % (gate ≥ 80 % / ≥ 70 % cumplido). Build Release sin warnings tratados como error. Se agregó una prueba de round-trip EF del agregado Relevamiento (BT-07).

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-06 | Historia | Aceptada |
| US-07 | Historia | Aceptada |
| US-08 | Historia | Aceptada |
| US-09 | Historia | Aceptada |
| US-10 | Historia | Aceptada |
| BT-10 | Backlog técnico | Aceptada |
| BT-03 | Backlog técnico | Aceptada |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 02 se traslada. |

BT-07 avanzó (entidades Relevamiento y AsignacionAgente mapeadas y migradas, con prueba de round-trip); las entidades restantes (observaciones, marcadores, fotos, comentarios, etiquetas) y la integración con Testcontainers quedan para los sprints de captura (EP-03 en adelante).

## 7. Decisiones tomadas durante el review

- Incorporar al backlog una tarea de provisión de credenciales / seed de prueba para habilitar la demo de alta desde el front con un jefe de área real.
- Mantener el mediador CQRS mínimo (sin librería externa) como patrón del módulo de relevamientos y de los módulos de dominio rico siguientes.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Sprint review del Sprint 02 (slice de relevamientos). Veredicto Cumplido, velocity 27, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
