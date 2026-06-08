# Sprint Review — Sprint 69

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-69_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita del sprint goal de `plan-iteracion-sprint-69_v1.0.md`:

> Que cada petición sea rastreable de punta a punta y que los logs sean legibles por una máquina.

Veredicto: Cumplido.

Explicación corta: sprint #2 de la épica **Hardening de producción** (observabilidad). **Id de correlación:** el núcleo `Correlacion` (gate) reusa el id que envíe el cliente/proxy en `X-Correlation-ID` (si es válido) o genera un GUID compacto; un middleware temprano lo pone en el **scope de logging** (todos los logs del request lo llevan) y lo **devuelve** en la respuesta. **Logs estructurados:** fuera de Development los logs salen en **JSON** (parseables por un agregador); en Development se conserva la consola legible. **Log de request:** método, ruta, estado y duración por petición. Sin tocar dominio.

## 2. Demos realizadas

| ID | Tipo | Descripción | Feedback |
| --- | --- | --- | --- |
| OBS-CORRELACION | Observabilidad | Respuesta sin cabecera → id generado (`3f0175…`); con cabecera → eco (`traza-de-prueba-42`) | Request rastreable |
| OBS-LOGS | Observabilidad | Logs JSON fuera de Development; cada request logueado (método/ruta/estado/duración) | Listo para agregador |

## 3. Feedback recibido

- El id de correlación es la base para diagnosticar incidentes en producción: un solo id agrupa todos los logs de una petición (y, a futuro, su traza entre servicios).
- Reusar el id entrante permite correlacionar a través de un proxy/balanceador o de un cliente que ya lo setea.
- Los logs JSON habilitan búsqueda/alertas en un agregador sin parseo frágil.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 5 |
| Puntos completados | 5 |
| Velocity efectiva | 5 |
| Ratio de completitud | 100 % |
| Defectos detectados | 0 |

Pruebas: **519** (470 unitarias + 49 de integración), **+9**: 7 unitarias de `Correlacion` + 2 de integración (`ObservabilidadTests`: generación y eco de la cabecera). Cobertura DoD sin regresión (núcleo nuevo cubierto en `GeoVial.Application`). Verificado en vivo: la cabecera `X-Correlation-ID` se genera sin entrada y se reusa con entrada.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| OBS-CORRELACION | Historia | Aceptada (reuso/generación + scope + cabecera) |
| OBS-LOGS | Historia | Aceptada (JSON fuera de Development + log de request) |

## 6. Carry-over

| ID | Puntos | Motivo |
| --- | --- | --- |
| — | 0 | Sin traslados. |

**Épica Hardening de producción — sprint 2/N.** Hecho: config segura + health (S68), observabilidad (S69). Próximos: handler global de errores (ProblemDetails, sin filtrar stack en prod) + headers de seguridad + rate limiting; contenedor productivo + readiness ampliado (S3, migraciones).

## 7. Decisiones tomadas

- Se reusa el id entrante **si es válido** (no vacío, ≤ 128, sin caracteres de control); si no, se genera — para no propagar una cabecera abusiva a los logs.
- El middleware de correlación va **temprano** (antes del log HTTP y de la autenticación), para que incluso los fallos de auth queden correlacionados.
- JSON sólo fuera de Development (en Development, consola legible para el desarrollador).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Review del Sprint 69 (hardening #2: observabilidad — correlación de request + logs estructurados). Cumplido, velocity 5, 0 carry-over, 519 pruebas (+9). Generado por AG-07 |
