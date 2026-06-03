# Sprint Review — Sprint 39

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-39_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-39_v1.0.md`:

> Cubrir el comportamiento **real** de `AlmacenS3` con AWS SDK v4 (migrado en S36) ejecutándolo contra **LocalStack** (S3 emulado en Docker), sin cuenta AWS.

Veredicto: Cumplido.

Explicación corta: se agregó al job `build-test` de `ci.yml` un **service container de LocalStack** (`localstack/localstack:3`, `SERVICES=s3`, puerto `4566`) y la variable `LOCALSTACK_S3_URL`. La prueba de integración `AlmacenS3LocalStackTests` configura un `AmazonS3Client` contra LocalStack (ServiceURL + `ForcePathStyle` + credenciales de prueba), crea un bucket y ejercita `AlmacenS3` de punta a punta: guardar → existe → recuperar (round-trip), un objeto inexistente (404 → null/false), y eliminar. Si `LOCALSTACK_S3_URL` no está (local, sin Docker), la prueba hace **no-op**; en CI corre completa, esperando la disponibilidad de LocalStack (reintenta ~30 s) antes de fallar. Así se cierra la brecha del Sprint 36 (la migración a v4 estaba verificada por compilación + mocks, pero no contra S3 real). Sin cambios de backend ni dominio.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| ADR-08 | Calidad | `AlmacenS3` ejercitado contra LocalStack en CI: round-trip + 404 + eliminar | El backend S3 verificado contra un S3 real |
| EP-09 | DevOps | Service container de LocalStack en `ci.yml`; la prueba corre en cada PR/push | Cobertura de integración sin cuenta AWS |
| ADR-08 | Robustez | Sin Docker local, la prueba hace no-op (no rompe el build) | El equipo trabaja sin Docker; CI cubre |

## 3. Feedback recibido

- Cerrar la brecha de S36 con LocalStack da confianza de que la API v4 de S3 se comporta como esperamos (no sólo que compila): bucket, put/get/exists/delete y el manejo de 404.
- El patrón "no-op si no hay LocalStack, corre en CI" permite tener la prueba sin imponer Docker en cada máquina de desarrollo.
- La verificación efectiva es la CI del PR (`ci.yml` con el service container); es el mismo patrón que ya usamos para validar cambios que no se pueden correr localmente.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 338 (300 unitarias + 38 de integración), +1 de integración (`AlmacenS3LocalStackTests`). Localmente la nueva prueba hace no-op (sin Docker); su ejecución real ocurre en la **CI del PR**, contra el service container de LocalStack. El gate de cobertura se mantiene; build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-S3-LOCALSTACK | Tarea | Aceptada (prueba de `AlmacenS3` contra LocalStack en CI; no-op local; cierra la brecha de S36) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 39 se traslada. |

Backlog restante: unificar la URL de teselas en el JS web; prueba manual de offline en el dispositivo; y el mantenimiento continuo (ahora con auto-merge de minor/patch).

## 7. Decisiones tomadas durante el review

- Usar un service container de LocalStack en `ci.yml` (en vez de Testcontainers) para no agregar dependencias NuGet (riesgo de warnings de vulnerabilidad) y mantener la prueba simple.
- Gatear la ejecución por `LOCALSTACK_S3_URL`: corre en CI, no-op local sin Docker.
- Reintentar la disponibilidad de LocalStack en la prueba para evitar falsos negativos por el arranque del contenedor.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Sprint review del Sprint 39 (prueba de `AlmacenS3` contra LocalStack en CI). Veredicto Cumplido, velocity 8, 0 carry-over, 338 pruebas (la nueva corre en CI). Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
