# Sprint Retrospectiva — Sprint 71

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-71_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- **Épica cerrada con un cierre real.** S68–S71 dejan el backend listo para producción: config segura, observabilidad, robustez y despliegue de migraciones; el runbook ata todo. No quedó un cierre "de papel": cada pieza tiene test y verificación en vivo.
- **Migración segura sin sobre-ingeniería.** Un comando `migrate` + una política por entorno (`PoliticaMigracion`, núcleo del gate) resuelven el problema de réplicas migrando a la vez, sin frameworks de orquestación.
- **Verificación en vivo del paso de despliegue.** Se corrió `migrate` contra SQL real (lock + tabla de historial) y se confirmó que **sale** sin levantar el server; el readiness ampliado da Healthy con el esquema al día.

## 2. Qué no salió bien

- **El readiness de migraciones pendientes no se prueba en el gate.** InMemory no soporta `GetPendingMigrationsAsync`, así que el camino "Unhealthy por pendientes" sólo se cubre por construcción/relacional. Una prueba con SQLite/relacional y una migración sin aplicar lo cubriría; queda como mejora.
- **`SeedInicial` cambió de contrato.** Dejó de migrar; si alguien lo invoca esperando que cree el esquema, ya no lo hace. Está documentado, pero es un cambio sutil de responsabilidad (la migración la decide `Program.cs`).

## 3. Qué probar

- Deploy real: correr el job `migrate`, luego las réplicas; verificar que `/health/ready` pasa a Healthy recién cuando el esquema está al día.
- Un release con una migración nueva sin correr el job: el readiness debe dar Unhealthy (no enrutar tráfico).

## 4. Acciones concretas

| Acción | Responsable | Fecha | Estado |
| --- | --- | --- | --- |
| Prueba relacional (SQLite) del readiness con migración pendiente | AG-08 | 2029-03-30 | Pendiente |
| Rate limiting distribuido (Redis) + `ForwardedHeaders` (multi-réplica) | AG-08 | — | Backlog post-épica |
| OpenTelemetry (tracing/métricas) | AG-08 | — | Backlog post-épica |
| Readiness del almacenamiento de fotos (S3) | AG-09 | — | Backlog post-épica |
| H-07 con renderer de Shell propio (deuda de S67) | AG-08 | — | Diferido |
| Definir la próxima épica (¿reporting? ¿capacidades de campo? ¿multi-área?) | AG-07 | 2029-03-30 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 70 | Estado |
| --- | --- |
| Épica hardening #4 (cierre): contenedor productivo + readiness ampliado | **Hecho** (este sprint): migración como paso de despliegue + readiness + runbook |
| Rate limiting distribuido (Redis) + ForwardedHeaders | Backlog post-épica |
| OpenTelemetry | Backlog post-épica |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Retro del Sprint 71 (hardening #4 / cierre de la épica). Bien: cierre real con tests + verificación en vivo; migración segura sin sobre-ingeniería. Mejora: probar el readiness de migraciones pendientes en relacional. Acción: definir la próxima épica. Generada por AG-07 |
