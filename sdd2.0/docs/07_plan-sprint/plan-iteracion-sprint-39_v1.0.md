# Plan de Iteración — Sprint 39

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-39_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-11-30
**Fecha fin:** 2027-12-11
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S36–S38); capacidad sugerida estricta 9 SP. Se compromete la prueba de `AlmacenS3` contra LocalStack (8 SP). Cierra la brecha de verificación de S36 (comportamiento real de AWS SDK v4); en el gate de CI.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Cubrir el comportamiento **real** de `AlmacenS3` con AWS SDK v4 (migrado en S36) ejecutándolo contra **LocalStack** (S3 emulado en Docker), sin cuenta AWS. La migración de S36 se verificó por compilación + tests mockeados; este sprint cierra la brecha de "no se ejercita S3 real" con una prueba de integración que corre en CI contra un service container de LocalStack.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-S3-LOCALSTACK | Tarea | Prueba de integración de `AlmacenS3` contra LocalStack en CI; no-op local sin Docker | Media | 8 | QA / Dev backend | Pendiente |

Total de puntos comprometidos: 8 SP. Cierra deuda de verificación; sin cambios de backend ni dominio.

## 4. Alcance técnico

1. **`ci.yml`**: se agrega un **service container de LocalStack** (`localstack/localstack:3`, `SERVICES=s3`, puerto `4566`) al job `build-test`, y la variable `LOCALSTACK_S3_URL=http://localhost:4566`. Así `dotnet test` ejecuta la prueba de S3 contra LocalStack en CI.
2. **`GeoVial.IntegrationTests`**: `AlmacenS3LocalStackTests` configura un `AmazonS3Client` contra LocalStack (ServiceURL + `ForcePathStyle`, credenciales de prueba), crea un bucket y ejercita `AlmacenS3`: guardar → existe → recuperar (round-trip), 404 → null/false, eliminar. **Si `LOCALSTACK_S3_URL` no está** (local, sin Docker) la prueba hace **no-op** (retorno temprano); en CI corre completa. Espera la disponibilidad de LocalStack (reintenta) antes de fallar.
3. **Referencia**: se agrega `GeoVial.Infrastructure` al proyecto de integración (para `AlmacenS3`).

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica. Criterios específicos:

- La prueba ejercita `AlmacenS3` contra LocalStack (round-trip + 404 + eliminar) y pasa en CI.
- Sin `LOCALSTACK_S3_URL`, la prueba no rompe el build local (no-op); con ella, corre.
- La suite local (337 + la nueva) sigue verde; la verificación real ocurre en la CI del PR (`ci.yml` con el service container).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Sin Docker local no se puede verificar la prueba aquí | Alta | Medio | Se verifica en la CI del PR (`ci.yml` corre en `pull_request` con LocalStack); local se confirma compilación + no-op |
| LocalStack tarda en levantar y la prueba falla por timing | Media | Bajo | La prueba reintenta la conexión (~30 s) antes de fallar |
| `ForcePathStyle`/endpoint mal configurados para LocalStack | Media | Bajo | Config estándar de LocalStack (ServiceURL + ForcePathStyle + región de autenticación) |

## 7. Criterios de hecho del sprint

El Sprint 39 se considera completo cuando la prueba de `AlmacenS3` contra LocalStack está en el repo, corre en CI contra el service container y pasa, no rompe el build local (no-op sin Docker), y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | brecha de verificación de S36 (AWS SDK v4 sin S3 real); retro S36 (LocalStack) |
| Componente | `GeoVial.Infrastructure/Alojamiento/AlmacenS3.cs`; `ci.yml` |
| Arquitectura | ADR-08 (alojamiento de fotos con backend configurable) |
| Calidad | definition-of-done §1.4 |
| Tests previstos | `AlmacenS3LocalStackTests` (round-trip / 404 / eliminar contra LocalStack) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Plan inicial del Sprint 39 (prueba de `AlmacenS3` contra LocalStack en CI): service container en `ci.yml` + test de integración con no-op local. Cierra la brecha de verificación de AWS SDK v4 de S36. Compromete 8 SP. Generado por AG-07 |
