# Plan de Iteración — Sprint 71

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-71_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2029-03-05
**Fecha fin:** 2029-03-16
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas. Equipo: 4 (`equipo_n: 4`). Estimación en story points.
- Capacidad: promedio móvil 7,0 SP (S68–S70). **Sprint #4 y cierre de la épica "Hardening de producción"**: desplegar las migraciones de forma segura y completar el readiness, con un runbook de producción.

## 2. Objetivo del sprint

**Que el esquema se migre de forma segura al desplegar (no desde varias réplicas) y que el readiness no dé OK con el esquema desactualizado; documentar el despliegue productivo.**

- **Migración como paso de despliegue:** en producción el backend **no** auto-migra al arrancar; un job/init-container con el comando `migrate` aplica las migraciones y termina. En Development se conserva el auto-migrate (instancia única).
- **Readiness ampliado:** `/health/ready` da Unhealthy si hay **migraciones pendientes** (además de la conectividad), para no servir tráfico con el esquema viejo.
- **Runbook de despliegue:** documentar config, orden de despliegue, sondas y observabilidad/seguridad.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| HARD-MIGRA | Historia | Migración como paso de despliegue (comando `migrate`) + política por entorno + readiness de migraciones pendientes | Alta | 4 | Backend (AG-08) | Cerrada |
| HARD-RUNBOOK | Tarea | Runbook de despliegue en producción (devops) | Media | 1 | DevOps (AG-09) | Cerrada |

Total: 5 SP. **Cierra la épica de hardening.**

## 4. Alcance técnico

- **Núcleo `PoliticaMigracion`** (`GeoVial.Application.Configuracion`, gate): `DebeAutoMigrarAlArrancar(entorno)` (true sólo en Development) y `EsComandoMigrar(args)` (primer argumento `migrate`). Lógica pura y testeable.
- **`Program.cs`:** si `EsComandoMigrar(args)` → aplica migraciones y **termina** (paso de despliegue). En arranque normal, auto-migra sólo si `DebeAutoMigrarAlArrancar` y el proveedor es relacional. La migración sale de `SeedInicial` (que ahora sólo siembra el raíz).
- **`ChequeoBaseDeDatos`:** el readiness, además de `CanConnectAsync`, verifica `GetPendingMigrationsAsync` (relacional) y da Unhealthy si hay pendientes.
- **`SeedInicial`:** deja de migrar; asume el esquema (o la base en memoria que se crea sola).
- **Runbook** `09_devops/despliegue-produccion_v1.0.md`.
- **Sin migración nueva ni cambios de dominio.**

## 5. Definition of Done aplicada

- `dotnet GeoVial.Api.dll migrate` aplica migraciones y termina (no levanta el server).
- En producción el backend no auto-migra; en Development sí (verificado: arranca y `/health/ready` Healthy).
- `/health/ready` da Unhealthy si hay migraciones pendientes; Healthy si el esquema está al día.
- Suite del gate verde con el núcleo nuevo cubierto; cobertura DoD sin regresión.
- Runbook de despliegue publicado.

## 6. Riesgos y mitigaciones

| Riesgo | Prob. | Impacto | Mitigación |
| --- | --- | --- | --- |
| Producción no migra y el arranque falla al sembrar el raíz | Media | Medio | El orden de despliegue (runbook) corre `migrate` antes; el readiness no da OK hasta migrar |
| `GetPendingMigrationsAsync` falla en InMemory (tests) | Media | Bajo | Sólo se consulta en proveedor relacional; InMemory queda Healthy |
| Quitar el migrate de `SeedInicial` rompe el arranque de Development | Media | Alto | `Program.cs` auto-migra en Development antes de sembrar; verificado en vivo |

## 7. Criterios de hecho del sprint

Completo cuando: el comando `migrate` migra y sale; Development auto-migra y arranca; producción no auto-migra; el readiness contempla migraciones pendientes; el gate queda verde con lo nuevo cubierto; el runbook está publicado; y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Épica "Hardening de producción", sprint #4 / cierre; roadmap retro S70 |
| Componentes | `GeoVial.Application` (`PoliticaMigracion`), `GeoVial.Api` (`Program`, `ChequeoBaseDeDatos`), `GeoVial.Infrastructure` (`SeedInicial`), `09_devops` (runbook) |
| Calidad | definition-of-done §1.4; despliegue seguro de esquema |
| Tests | `PoliticaMigracionTests` (+7); readiness verificado en vivo (relacional) y por el gate (InMemory) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Plan del Sprint 71 (hardening #4 / cierre: migración como paso de despliegue + readiness de migraciones pendientes + runbook). 5 SP. Generado por AG-07 |
