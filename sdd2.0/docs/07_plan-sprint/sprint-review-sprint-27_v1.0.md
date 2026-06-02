# Sprint Review — Sprint 27

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-27_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-27_v1.0.md`:

> Eliminar la duplicación de andamiaje en las pruebas E2E: migrar `CapturaE2ETests` al helper compartido `EscenarioE2E` (sembrado de escenario + autenticación HTTP), de modo que toda la suite E2E (captura, conflictos por radio, edición en conflicto) use el mismo sembrado autocontenido que no depende del seed de Development.

Veredicto: Cumplido.

Explicación corta: `CapturaE2ETests` se reescribió sobre `EscenarioE2E`, eliminando su `Escenario`/`Clave`/`SembrarEscenarioAsync`/`SembrarAgenteAsync`/`SembrarAgenteEnNuevaAreaAsync`/`ClienteAutenticadoAsync` privados (≈45 líneas de andamiaje duplicado). El escenario pasó a sembrar su propia área en lugar de tomar `db.Areas.FirstAsync()` del seed de Development, desacoplando las pruebas del seed (acción de la retro del Sprint 20). La prueba de autorización ahora usa un segundo escenario sembrado —cuyo agente pertenece a otra área— para verificar que no puede capturar en el relevamiento del primero (RN-01). Las tres pruebas siguen verdes con idéntica cobertura de flujo. Con esto, toda la suite E2E (captura, conflictos por radio, edición en conflicto) comparte un único punto de sembrado/autenticación y se cierra la acción reiterada en las retros S24/S25/S26.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| BT-E2E-HELPER | Calidad | Las 3 E2E de captura corren sobre `EscenarioE2E`, sin andamiaje propio | Suite E2E homogénea |
| BT-E2E-HELPER | Calidad | El escenario siembra su propia área (no depende del seed de Development) | Pruebas aisladas y reproducibles |
| RN-01 | Calidad | Autorización por área verificada con un segundo escenario (agente ajeno) | Regla de negocio cubierta sin acoplarse al seed |

## 3. Feedback recibido

- Consolidar el sembrado en un único helper elimina la deriva entre suites: un cambio de escenario se hace en un solo lugar.
- Sembrar la propia área cierra del todo el acoplamiento al seed de Development que señaló la retro S20.
- No hubo cambios de lógica de dominio/aplicación: es trabajo de calidad puro, con la red de pruebas como evidencia.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 315 verdes (278 unitarias + 37 de integración), sin cambio de conteo respecto del Sprint 26: es una migración de andamiaje sin nuevas pruebas ni lógica de dominio; las 3 E2E de captura siguen verdes tras el refactor. El gate de cobertura se mantiene. Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-E2E-HELPER | Tarea | Aceptada (`CapturaE2ETests` sobre `EscenarioE2E`, andamiaje duplicado eliminado, suite verde) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 27 se traslada. |

Siguen en el backlog: la publicación efectiva de `v1.0.0` (tag por el Release manager), el SBOM y la firma de las imágenes Docker del monolito, y el mapa interactivo (bloqueado por la clave de proveedor de mapas).

## 7. Decisiones tomadas durante el review

- Dejar `EscenarioE2E` como único punto de sembrado/autenticación E2E; las nuevas pruebas E2E parten de él.
- Modelar la autorización por área con un segundo escenario sembrado en vez de un agente suelto, para no reintroducir andamiaje específico.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 27 (migración de `CapturaE2ETests` al helper `EscenarioE2E`). Veredicto Cumplido, velocity 8, 0 carry-over, 315 pruebas verdes. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
