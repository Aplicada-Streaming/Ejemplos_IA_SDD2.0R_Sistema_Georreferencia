# Sprint Review — Sprint 24

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-24_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-24_v1.0.md`:

> Verificar de punta a punta por HTTP el ciclo de edición en conflicto (CU-07/CU-12): sincronizar un comentario y luego ediciones del mismo recurso que colisionan (el last-write-wins consolida la última escritura y marca `EdicionEnConflicto`, RN-04), listar el conflicto y confirmarlo.

Veredicto: Cumplido.

Explicación corta: una prueba E2E recorre por HTTP el ciclo completo: el agente captura un marcador, sincroniza la creación de un comentario (T0), luego una edición (T1) y una segunda edición (T2); en la segunda edición el comentario ya estaba editado, así que el last-write-wins consolida y marca `EdicionEnConflicto` (RN-04), que aparece en la respuesta de sync y en el listado de conflictos (tipo edición, con su recurso) y se confirma con la decisión `ConfirmarEdicion`. Una segunda prueba verifica la idempotencia por `CambioId` (RC-03): reenviar el mismo cambio se confirma sin reaplicar ni reabrir conflicto. Se extendió el helper `EscenarioE2E` para exponer el identificador del agente.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| E2E | Prueba | Sync crear → editar → editar (colisión) marca EdicionEnConflicto, se lista y se confirma | Ediciones concurrentes dirimibles E2E |
| E2E | Prueba | Reenviar el mismo `CambioId` se confirma sin duplicar (idempotencia RC-03) | Reintentos seguros verificados |

## 3. Feedback recibido

- El ciclo de edición en conflicto (sincronización del S09, resolución del S18) queda verificado de punta a punta por HTTP.
- Con esto, **toda** la cobertura E2E pendiente de las retros (S18/S20/S23) queda cerrada: captura, ubicación manual, revisión, edición, autorización, conflictos por radio y por edición.
- La idempotencia de la sincronización, clave para la continuidad offline, queda verificada extremo a extremo.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 307 verdes (270 unitarias + 37 de integración), +2 respecto del Sprint 23 (el ciclo de edición en conflicto y la idempotencia). El gate de cobertura de dominio/aplicación se mantiene (sin lógica nueva). Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-E2E-EDIT | Tarea | Aceptada (E2E del ciclo de edición en conflicto + idempotencia de la sincronización) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 24 se traslada. |

El SBOM firmado del paquete y el pulido móvil (mapa interactivo, caché de fotos; migrar `CapturaE2ETests` al helper compartido) siguen en el backlog.

## 7. Decisiones tomadas durante el review

- Provocar el `EdicionEnConflicto` con tres operaciones de sync sobre el mismo comentario (crear + dos ediciones), que es la secuencia mínima que dispara la marca de conflicto (RN-04).
- Verificar la idempotencia en el mismo frente E2E, por ser la garantía central de la sincronización offline.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 24 (endurecimiento E2E del ciclo de edición en conflicto + idempotencia de la sincronización). Veredicto Cumplido, velocity 8, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
