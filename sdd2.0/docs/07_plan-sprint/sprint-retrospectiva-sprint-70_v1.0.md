# Sprint Retrospectiva — Sprint 70

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-70_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- **Tres defensas en un sprint, todas testeadas.** Manejador de errores, cabeceras de seguridad y rate limiting quedaron con cobertura: núcleos en el gate (`CabecerasSeguridad`) + integración (500 sin fuga, headers, 429).
- **El 429 se probó de forma determinista.** En vez de tunear el límite global y arriesgar pruebas flaky, una fábrica derivada (`FabricaPruebasLimiteRapido`) con límite bajo aísla el test del 429; el resto del gate usa el default generoso.
- **Composición limpia con S69.** El log del error reusa el id de correlación (scope de S69), así un 500 genérico al cliente es rastreable internamente.

## 2. Qué no salió bien

- **Rate limiting por IP, en memoria.** Funciona en una instancia, pero detrás de varias réplicas el límite es por-réplica (no global) y la IP real exige confiar en `X-Forwarded-For` (proxy). Suficiente para empezar; un store distribuido (Redis) y `ForwardedHeaders` quedan para cuando haya despliegue multi-réplica.
- **Endpoint de diagnóstico para testear el handler.** Hizo falta `/_diagnostico/excepcion` (sólo Development) para disparar una excepción real; es pragmático pero es andamiaje de prueba en el código de producción (guardado por entorno).

## 3. Qué probar

- En producción: un 500 nunca muestra el stack ni el mensaje de la excepción (sólo el ProblemDetails genérico + traceId); las cabeceras de seguridad están; el login se corta con 429 al pasar el límite configurado.

## 4. Acciones concretas

| Acción | Responsable | Fecha | Estado |
| --- | --- | --- | --- |
| Épica hardening #4 (posible cierre): validar arranque productivo en contenedor + readiness ampliado (S3, migraciones al desplegar) | AG-09 | 2029-03-16 | Planificado |
| Rate limiting distribuido (Redis) + `ForwardedHeaders` para multi-réplica | AG-08 | — | Backlog (cuando haya despliegue) |
| Evaluar OpenTelemetry (tracing/métricas) | AG-08 | — | Backlog |
| H-07 con renderer de Shell propio (deuda de S67) | AG-08 | — | Diferido |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 69 | Estado |
| --- | --- |
| Épica hardening #3: errores + headers + rate limiting | **Hecho** (este sprint) |
| Épica hardening #4: contenedor productivo + readiness ampliado | Planificado (próximo, posible cierre de la épica) |
| Evaluar OpenTelemetry | Backlog |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Retro del Sprint 70 (hardening #3). Bien: tres defensas testeadas, 429 determinista con fábrica derivada, composición con la correlación de S69. Límites: rate limiting en memoria/por-réplica, andamiaje de diagnóstico. Próximo: contenedor productivo (posible cierre de la épica). Generada por AG-07 |
