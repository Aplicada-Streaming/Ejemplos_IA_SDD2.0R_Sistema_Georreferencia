# Sprint Review — Sprint 52

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-52_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-52_v1.0.md`:

> El objetivo es filtrar en la base (área propia para esos roles; todo para raíz/jefe general), centralizando la decisión de rol en `Autorizacion` y preservando exactamente el comportamiento observable.

Veredicto: Cumplido.

Explicación corta: `ListarRelevamientosHandler` traía toda la tabla y filtraba en memoria. Ahora decide con `Autorizacion.AccedeATodasLasAreas` y filtra **en la base**: raíz/jefe general → `ListarTodosAsync`; jefe de área/agente → `ListarPorAreaAsync(areaId)`; no vigente o sin área → vacío. Mismo resultado observable que antes, sin el barrido. Cierra la acción de retro arrastrada desde S47.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| BT-LISTADO-AREA | Higiene/Robustez | El listado de un jefe de área se obtiene filtrando por su área en la base; raíz sigue viendo todo | No cambia lo que ve el usuario; escala mejor |

(Es un sprint de limpieza: sin cambios visibles de producto; la demo es técnica.)

## 3. Feedback recibido

- Cierra una deuda que se arrastraba en las retros desde S47 (reiterada cinco veces). Cuando una acción reaparece, conviene comprometerla y cerrarla.
- La decisión de rol quedó centralizada en `Autorizacion` (`AccedeATodasLasAreas`), evitando duplicar el `switch` de roles en el handler.
- Se comprometió **menos de la capacidad sugerida a propósito** (5 SP) por honestidad: es deuda técnica de bajo riesgo, no una historia; inflarla a 8 habría falseado la velocity.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 5 |
| Puntos completados | 5 |
| Velocity efectiva | 5 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: **428** (385 unitarias + 43 de integración), +7 unitarias: handler (raíz ve todas las áreas; usuario no vigente → vacío) y `Autorizacion.AccedeATodasLasAreas` (por rol ×4 + no vigente). El test de filtro por área previo sigue verde (comportamiento preservado). Cobertura del gate: Domain 88,5 % / 80,5 %; Application 87,6 % / 76,3 % (umbral 80 % / 70 %). El MAUI sigue compilando (build de control; el sprint no toca el móvil).

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-LISTADO-AREA | Tarea | Aceptada (listado de área filtra en la base, comportamiento preservado) |
| BT-LISTADO-TESTS | Tarea | Aceptada (caminos cubiertos en el gate) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 52 se traslada. |

Estado del backlog: con esto, el backlog de la **revisión funcional** queda esencialmente cerrado (captura offline, selección, reingreso, auto-sync, idempotencia, "asignados a mí", indicador de sync, avisos de conflictos, bandeja ver+ubicar, y esta limpieza). Queda pendiente el **biométrico nativo** (plataforma, requiere dispositivo) y la **verificación on-device** acumulada (indicador, bandeja, ubicar). Decisión de dirección para el Product Owner.

## 7. Decisiones tomadas durante el review

- Centralizar la decisión de rol en `Autorizacion.AccedeATodasLasAreas`.
- Filtrar en la base para usuarios de área; mantener `ListarTodosAsync` sólo para los roles que ven todo.
- Comprometer 5 SP (sprint de limpieza) en vez de forzar 8, documentándolo como outlier con motivo.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Sprint review del Sprint 52 (limpieza: listado de área en base). Veredicto Cumplido, velocity 5 (sprint de limpieza acotado), 0 carry-over, 428 pruebas (+7); cobertura del gate mantenida. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
