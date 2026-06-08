# Sprint Review — Sprint 71

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-71_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita del sprint goal de `plan-iteracion-sprint-71_v1.0.md`:

> Que el esquema se migre de forma segura al desplegar (no desde varias réplicas) y que el readiness no dé OK con el esquema desactualizado; documentar el despliegue productivo.

Veredicto: Cumplido. **Cierra la épica de Hardening de producción.**

Explicación corta: la migración pasa a ser un **paso explícito del despliegue**: invocar el backend como `migrate` aplica las migraciones y **termina** (un job/init-container migra una vez, antes de levantar las réplicas). En **Development** se conserva el auto-migrate (instancia única); en **producción** no auto-migra (lo decide `PoliticaMigracion`, gate). El **readiness** ampliado: `/health/ready` da Unhealthy si hay **migraciones pendientes** (además de la conectividad), así el orquestador no enruta tráfico con el esquema viejo. Se publicó el **runbook de despliegue** (`09_devops`) que consolida el hardening de S68–S71.

## 2. Demos realizadas

| ID | Tipo | Descripción | Feedback |
| --- | --- | --- | --- |
| HARD-MIGRA | Despliegue | `dotnet GeoVial.Api.dll migrate` aplica migraciones y sale (no levanta el server) | Migración segura en deploy |
| HARD-MIGRA | Operación | `/health/ready` Healthy con el esquema al día; Unhealthy con migraciones pendientes | No sirve con esquema viejo |
| HARD-MIGRA | Compatibilidad | Development auto-migra y arranca; login `campo1` → 200 | Sin regresión en desarrollo |
| HARD-RUNBOOK | Devops | Runbook de despliegue (config, orden, sondas, observabilidad/seguridad) | Despliegue documentado |

## 3. Feedback recibido

- Separar la migración del arranque evita que varias réplicas migren a la vez (problema real en producción) y deja el deploy controlado.
- El readiness que contempla migraciones pendientes cierra la ventana entre "código nuevo desplegado" y "migración corrida".
- El runbook hace explícito el orden de despliegue y los secretos por entorno; baja el riesgo de un despliegue inseguro.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 5 |
| Puntos completados | 5 |
| Velocity efectiva | 5 |
| Ratio de completitud | 100 % |
| Defectos detectados | 0 |

Pruebas: **536** (481 unitarias + 55 de integración), **+7** unitarias de `PoliticaMigracion`. Cobertura DoD sin regresión. Verificado en vivo: el comando `migrate` migra y sale (lock + tabla de historial, contra SQL real); el backend normal arranca, `/health` y `/health/ready` Healthy (readiness al día), login OK. El readiness de migraciones pendientes es relacional; en el gate (InMemory) queda Healthy por construcción.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| HARD-MIGRA | Historia | Aceptada (migrate mode + política + readiness de migraciones) |
| HARD-RUNBOOK | Tarea | Aceptada (runbook de despliegue) |

## 6. Carry-over

| ID | Puntos | Motivo |
| --- | --- | --- |
| — | 0 | Sin traslados. |

**Épica Hardening de producción: cerrada (S68–S71).** Config segura + fail-fast (S68), observabilidad —correlación + logs estructurados— (S69), robustez —errores + headers + rate limiting— (S70), despliegue de migraciones + readiness + runbook (S71). El backend queda listo para un despliegue productivo seguro y observable. **Backlog post-épica** (cuando haya despliegue multi-réplica real): rate limiting distribuido (Redis) + `ForwardedHeaders`, OpenTelemetry, readiness del almacenamiento de fotos.

## 7. Decisiones tomadas

- En producción el backend **no** auto-migra; la migración es un paso de despliegue (`migrate`). En Development se conserva el auto-migrate por conveniencia (instancia única).
- La migración sale de `SeedInicial` (que ahora sólo siembra el raíz) y la decide `Program.cs` según `PoliticaMigracion`.
- El readiness consulta migraciones pendientes **sólo en proveedor relacional** (InMemory no lo soporta y no aplica).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Review del Sprint 71 (hardening #4 / cierre: migración como paso de despliegue + readiness de migraciones pendientes + runbook). Cumplido, velocity 5, 0 carry-over, 536 pruebas (+7). **Cierra la épica de Hardening de producción** (S68–S71). Generado por AG-07 |
