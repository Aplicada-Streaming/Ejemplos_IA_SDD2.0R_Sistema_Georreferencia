# Plan de Iteración — Sprint 70

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-70_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2029-02-19
**Fecha fin:** 2029-03-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas. Equipo: 4 (`equipo_n: 4`). Estimación en story points.
- Capacidad: promedio móvil 5,3 SP (S67–S69). **Sprint #3 de la épica "Hardening de producción"**, de **alcance pleno (8 SP)**: robustez del backend ante errores y abuso.

## 2. Objetivo del sprint

**Que el backend no filtre detalles internos ante un error, agregue defensas de seguridad estándar y acote el abuso de autenticación.** Tres frentes:

- **Manejo global de errores:** una excepción no controlada devuelve un `ProblemDetails` 500 **genérico** (sin el mensaje ni el stack de la excepción), en cualquier entorno; el error se registra (con el id de correlación de S69).
- **Cabeceras de seguridad:** toda respuesta lleva defensas de bajo costo (`X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy`, `Cross-Origin-Resource-Policy`).
- **Rate limiting:** los endpoints de autenticación (objetivo de fuerza bruta) se acotan por IP y ventana fija; al superar el límite, **429**.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| HARD-ERRORES | Historia | Manejador global de errores → ProblemDetails 500 sin filtrar detalles | Alta | 3 | Backend (AG-08) | Cerrada |
| HARD-CABECERAS | Historia | Cabeceras de seguridad en toda respuesta | Media | 2 | Backend (AG-08) | Cerrada |
| HARD-RATELIMIT | Historia | Rate limiting de los endpoints de autenticación (429 al superar el límite) | Alta | 3 | Backend (AG-08) | Cerrada |

Total: 8 SP (alcance pleno).

## 4. Alcance técnico

- **`ManejadorExcepcionesGlobal`** (`GeoVial.Api`, `IExceptionHandler`): registra la excepción (con el id de correlación del scope) y responde un `ProblemDetails` 500 con título/detalle genéricos —sin el mensaje ni el stack—. Registrado con `AddExceptionHandler` + `app.UseExceptionHandler()` (reusa `AddProblemDetails`).
- **`CabecerasSeguridad`** (`GeoVial.Application.Configuracion`, núcleo del gate): diccionario puro nombre→valor de las cabeceras; un middleware temprano las aplica a toda respuesta.
- **Rate limiting** (`Program.cs`): `AddRateLimiter` con una política `auth` (ventana fija por IP, `PermitLimit`/`VentanaSegundos` configurables, default 60/60s); `UseRateLimiter` y `.RequireRateLimiting("auth")` sobre el grupo `/api/v1/auth`. Rechazo con 429.
- **Endpoint de diagnóstico** sólo en Development (`/_diagnostico/excepcion`) para verificar el manejador de errores.
- **Sin cambios de dominio ni migración.** Los guardrails y la observabilidad de S68/S69 quedan intactos.

## 5. Definition of Done aplicada

- Una excepción no controlada responde 500 `application/problem+json` genérico, sin filtrar la excepción; el error queda logueado y correlacionado.
- Toda respuesta trae las cuatro cabeceras de seguridad.
- Superar el límite de intentos de login devuelve 429.
- Suite del gate verde con los núcleos nuevos cubiertos; cobertura DoD sin regresión.
- Verificado en vivo (cabeceras + handler de errores) y por el gate (429).

## 6. Riesgos y mitigaciones

| Riesgo | Prob. | Impacto | Mitigación |
| --- | --- | --- | --- |
| El rate limiting rompe pruebas que loguean varias veces | Media | Alto | Default generoso (60/60s) por IP/fábrica; las pruebas no lo superan. El test del 429 usa una fábrica con límite bajo (`FabricaPruebasLimiteRapido`) |
| El manejador de errores filtra detalles | Baja | Alto | Título/detalle genéricos fijos; test verifica que NO aparezcan el mensaje ni el tipo de la excepción |
| El endpoint de diagnóstico queda en producción | Baja | Medio | Mapeado **sólo** en Development |

## 7. Criterios de hecho del sprint

Completo cuando: las excepciones no controladas devuelven ProblemDetails 500 genérico; las respuestas traen las cabeceras de seguridad; el login se acota con 429; el gate queda verde con lo nuevo cubierto; verificado en vivo; y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Épica "Hardening de producción", sprint #3 (errores + headers + rate limiting); roadmap retro S69 |
| Componentes | `GeoVial.Api` (`ManejadorExcepcionesGlobal`, `Program`), `GeoVial.Application` (`CabecerasSeguridad`) |
| Calidad | definition-of-done §1.4; OWASP (headers, rate limiting), no fuga de detalles |
| Tests | `CabecerasSeguridadTests` (+4) + `HardeningHttpTests`/`RateLimitTests` (+6: error handler, headers, 429) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Plan del Sprint 70 (hardening #3: manejador global de errores + cabeceras de seguridad + rate limiting). Alcance pleno 8 SP. Generado por AG-07 |
