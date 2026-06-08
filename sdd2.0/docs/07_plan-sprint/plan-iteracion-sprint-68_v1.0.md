# Plan de Iteración — Sprint 68

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-68_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2029-01-22
**Fecha fin:** 2029-02-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas. Equipo: 4 (`equipo_n: 4`). Estimación en story points.
- Capacidad: promedio móvil 4,3 SP (S65–S67). **Primer sprint de la épica "Hardening de producción"**, de **alcance pleno (8 SP)**, que rompe la racha de nueve sprints acotados: prepara el backend para correr en producción de forma segura y observable.

## 2. Objetivo del sprint

**Que el backend no pueda arrancar inseguro en producción y que exponga su salud para orquestación.** Dos frentes del hardening:

- **Guardrails de configuración (fail-fast):** en producción, el backend **no debe** correr con la base **en memoria** (pierde datos) ni con la **clave JWT de desarrollo** (pública, está en el repo). Si la configuración es insegura, **falla rápido** al arrancar en vez de levantar.
- **Health checks:** exponer `/health` (liveness: el proceso responde) y `/health/ready` (readiness: la base es alcanzable), para que un contenedor/balanceador no enrute tráfico hasta que la dependencia crítica esté lista.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| HARD-CONFIG | Historia | Guardrails de arranque: fail-fast en producción con base InMemory o clave JWT de desarrollo | Alta | 5 | Backend (AG-08) | Cerrada |
| HARD-HEALTH | Historia | Health checks: liveness `/health` + readiness `/health/ready` (base de datos) | Alta | 3 | Backend (AG-08) | Cerrada |

Total: 8 SP (alcance pleno).

## 4. Alcance técnico

- **Núcleo `GuardrailsProduccion`** (`GeoVial.Application.Configuracion`, gate): `Validar(entorno, cadenaConexion, claveJwt) → IReadOnlyList<string>` (errores fatales; vacío = OK). Estricto sólo en `Production`: exige cadena de conexión real (no InMemory), y clave JWT presente, ≥ 32 caracteres y distinta de la de desarrollo. Permisivo en Development/Staging/tests. Lógica pura y testeable.
- **`Program.cs`:** al arrancar valida con `GuardrailsProduccion`; si hay errores, lanza `InvalidOperationException` (falla rápido, no levanta). Registra `AddHealthChecks` con un chequeo de base etiquetado `ready`, y mapea `/health` (liveness, sin chequear dependencias) y `/health/ready` (readiness, corre el chequeo de base). Endpoints **anónimos**.
- **`ChequeoBaseDeDatos`** (`GeoVial.Api`, `IHealthCheck`): `db.Database.CanConnectAsync()` → Healthy/Unhealthy.
- **`appsettings.Production.json`** (nuevo): sólo logging + nota; **sin secretos** — la cadena de conexión y la clave JWT se proveen por variable de entorno o secret manager (`ConnectionStrings__GeoVial`, `Jwt__ClaveSecreta`), y el arranque las exige.
- **Sin cambios de dominio ni migración.** Las pruebas de integración siguen aislando InMemory (`FabricaPruebas`); los guardrails son permisivos en su entorno (Development).

## 5. Definition of Done aplicada

- En producción sin cadena de conexión o con la clave JWT de desarrollo, el backend **falla al arrancar** (no corre inseguro); en Development arranca igual que antes.
- `/health` responde 200 si el proceso está vivo; `/health/ready` responde 200 sólo si la base es alcanzable.
- Suite del gate verde con el núcleo nuevo y los health checks cubiertos; cobertura DoD sin regresión.
- Verificado en vivo (endpoints de salud) y que Development sigue arrancando.

## 6. Riesgos y mitigaciones

| Riesgo | Prob. | Impacto | Mitigación |
| --- | --- | --- | --- |
| El guardrail rompe el arranque de Development/tests | Media | Alto | Estricto **sólo** en `Production`; cubierto por tests (permisivo fuera de prod) y verificado en vivo |
| El chequeo de readiness bloquea con InMemory en tests | Baja | Bajo | `CanConnectAsync` sobre InMemory devuelve true; test de integración lo verifica |
| Falsos negativos de readiness ante latencia de base | Baja | Bajo | El chequeo es un `CanConnect`; el orquestador reintenta |

## 7. Criterios de hecho del sprint

Completo cuando: el backend falla rápido en producción con config insegura y arranca bien en Development; `/health` y `/health/ready` responden según corresponda; el gate queda verde con lo nuevo cubierto; verificado en vivo; y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Épica "Hardening de producción" (elección del usuario tras saldar el backlog chico) |
| ADR | ADR-03 (JWT/ROPC), ADR-09 (SQL Server); supply-chain (secretos fuera del repo) |
| Componentes | `GeoVial.Application` (`GuardrailsProduccion`), `GeoVial.Api` (`Program`, `ChequeoBaseDeDatos`, `appsettings.Production.json`) |
| Calidad | definition-of-done §1.4; fail-fast de configuración |
| Tests | `GuardrailsProduccionTests` (+9) + `SaludEArranqueTests` (+3: liveness, readiness, fail-fast en prod) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Plan del Sprint 68 (hardening de producción #1: guardrails de arranque fail-fast + health checks liveness/readiness). Alcance pleno 8 SP; rompe la racha de acotados. Generado por AG-07 |
