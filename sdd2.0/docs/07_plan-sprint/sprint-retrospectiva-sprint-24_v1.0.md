# Sprint Retrospectiva — Sprint 24

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-24_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Las dos pruebas E2E pasaron al primer intento: entender el disparador del `EdicionEnConflicto` (la segunda edición, cuando el comentario ya estaba editado) permitió escribir la secuencia mínima correcta.
- Reusar `EscenarioE2E` (con el agregado del `AgenteId`) hizo trivial montar el escenario de sincronización; el helper compartido ya rinde en su segundo sprint consecutivo.
- Verificar la idempotencia por `CambioId` en el mismo frente E2E refuerza la garantía central de la continuidad offline con muy poco código extra.
- Con esto, **toda** la deuda de E2E acumulada en las retros (S18/S20/S23) queda saldada: los flujos centrales del MVP tienen cobertura extremo a extremo.

## 2. Qué no salió bien

- El SBOM firmado del paquete (acción de la retro S22) sigue pendiente; es el único frente de supply-chain de la librería que falta.
- `CapturaE2ETests` (S20) todavía no se migró al helper `EscenarioE2E` compartido; conviven dos formas de sembrar el escenario.
- El pulido móvil (mapa interactivo, caché de fotos) acumula varias retros sin abordarse, principalmente porque el grueso es UI fuera del gate.

## 3. Qué probar

- Generar y firmar el SBOM (CycloneDX) del paquete junto al `.nupkg`.
- Migrar `CapturaE2ETests` al helper `EscenarioE2E` para unificar el sembrado de escenarios.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| SBOM CycloneDX firmado del paquete junto al `.nupkg` | AG-09 (DevOps) | 2027-05-29 | Pendiente |
| Migrar `CapturaE2ETests` al helper `EscenarioE2E` compartido | AG-05 | 2027-05-29 | Pendiente |
| Pulido móvil pendiente (mapa interactivo, caché de fotos) | AG-08 (móvil) | 2027-05-29 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 23 | Estado actual |
| --- | --- |
| Helper de sincronización con colisión + E2E de edición en conflicto | Completada (entregada en este sprint) |
| Migrar `CapturaE2ETests` al helper `EscenarioE2E` compartido | Pendiente (se reitera) |
| Pulido móvil pendiente y SBOM firmado del paquete | Pendiente (se reitera) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 24 con 3 acciones nuevas y seguimiento de las del Sprint 23 (E2E de edición en conflicto completada; toda la deuda de E2E saldada). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
