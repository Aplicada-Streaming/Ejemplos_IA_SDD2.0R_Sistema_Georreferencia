# Sprint Retrospectiva — Sprint 69

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-69_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- **Núcleo chico, valor grande.** La correlación se modeló como una función pura (`Correlacion`) cubierta por el gate (+7), y el resto es middleware/config; el id de correlación es la base para diagnosticar en producción.
- **Verificación end-to-end barata.** El comportamiento (generar/eco de la cabecera) se probó por integración (+2) y en vivo con `curl -D -`, sin andamiaje.
- **Sin fricción de plataforma.** A diferencia de S67, todo es backend estándar (middleware + logging de ASP.NET Core); compiló y corrió a la primera.

## 2. Qué no salió bien

- **Observabilidad mínima, no completa.** Hay id de correlación + logs JSON + log de request, pero falta métrica (latencias/percentiles), tracing distribuido (OpenTelemetry) y correlación con el front/móvil. Es el piso; se amplía si el despliegue lo pide.
- **El log de request no incluye el id en el mismo evento de forma explícita.** El `CorrelationId` viaja en el scope (lo agrega el logger JSON), pero no se configuró un campo dedicado en el log HTTP; suficiente con JSON + scope, mejorable.

## 3. Qué probar

- En un entorno no-Development: los logs salen en JSON y cada request tiene su línea con método/ruta/estado/duración.
- Un cliente que setea `X-Correlation-ID`: el mismo id aparece en la respuesta y en los logs del request.

## 4. Acciones concretas

| Acción | Responsable | Fecha | Estado |
| --- | --- | --- | --- |
| Épica hardening #3: handler global de errores (ProblemDetails, sin stack en prod) + headers de seguridad + rate limiting | AG-08 | 2029-03-02 | Planificado |
| Épica hardening #4: contenedor productivo + readiness ampliado (S3, migraciones al desplegar) | AG-09 | 2029-03-16 | Pendiente |
| Evaluar OpenTelemetry (tracing/métricas) si el despliegue lo requiere | AG-08 | — | Backlog |
| H-07 con renderer de Shell propio (deuda de S67) | AG-08 | — | Diferido |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 68 | Estado |
| --- | --- |
| Épica hardening #2: observabilidad | **Hecho** (este sprint) |
| Épica hardening #3: errores + headers + rate limiting | Planificado (próximo) |
| Épica hardening #4: contenedor productivo + readiness ampliado | Pendiente |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Retro del Sprint 69 (hardening #2: observabilidad). Bien: núcleo chico testeable, verificación barata, sin fricción de plataforma. Mejora: ampliar a métricas/tracing si el despliegue lo pide. Próximo: hardening #3 (errores + headers + rate limiting). Generada por AG-07 |
