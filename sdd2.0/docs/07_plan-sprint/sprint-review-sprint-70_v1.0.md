# Sprint Review — Sprint 70

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-70_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita del sprint goal de `plan-iteracion-sprint-70_v1.0.md`:

> Que el backend no filtre detalles internos ante un error, agregue defensas de seguridad estándar y acote el abuso de autenticación.

Veredicto: Cumplido.

Explicación corta: sprint #3 de la épica **Hardening de producción**, alcance pleno (8 SP). **Errores:** `ManejadorExcepcionesGlobal` (`IExceptionHandler`) convierte una excepción no controlada en un `ProblemDetails` 500 **genérico** —sin el mensaje ni el stack— y la registra con el id de correlación de S69. **Cabeceras:** un middleware aplica `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy` y `Cross-Origin-Resource-Policy` a toda respuesta (núcleo `CabecerasSeguridad` en el gate). **Rate limiting:** los endpoints `/api/v1/auth/*` se acotan por IP/ventana fija (configurable, default 60/60s) y devuelven **429** al superar el límite.

## 2. Demos realizadas

| ID | Tipo | Descripción | Feedback |
| --- | --- | --- | --- |
| HARD-ERRORES | Seguridad | `/_diagnostico/excepcion` → 500 `application/problem+json` genérico (sin "excepción de prueba" ni el tipo) | No filtra detalles |
| HARD-CABECERAS | Seguridad | `/health` responde con las 4 cabeceras de seguridad | Defensas estándar |
| HARD-RATELIMIT | Seguridad | Con límite 3, el 4.º login → 429 (corta antes de validar credenciales) | Acota la fuerza bruta |

## 3. Feedback recibido

- No filtrar el stack en los 500 cierra una fuga de información típica; el id de correlación permite que soporte encuentre el error en los logs sin exponerlo al cliente.
- Las cabeceras de seguridad son defensas de bajo costo que conviene tener por defecto.
- El rate limiting en login es la primera línea contra fuerza bruta (complementa, no reemplaza, el bloqueo por cuenta).

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados | 0 |

Pruebas: **529** (474 unitarias + 55 de integración), **+10**: 4 unitarias de `CabecerasSeguridad` + 6 de integración (manejador de errores: 500 genérico sin fuga; 4 cabeceras de seguridad; rate limit 429). Cobertura DoD sin regresión. Verificado en vivo: cabeceras presentes en `/health`; `/_diagnostico/excepcion` → ProblemDetails 500 sin filtrar.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| HARD-ERRORES | Historia | Aceptada (ProblemDetails 500 genérico) |
| HARD-CABECERAS | Historia | Aceptada (4 cabeceras en toda respuesta) |
| HARD-RATELIMIT | Historia | Aceptada (429 en login al superar el límite) |

## 6. Carry-over

| ID | Puntos | Motivo |
| --- | --- | --- |
| — | 0 | Sin traslados. |

**Épica Hardening de producción — sprint 3/N.** Hecho: config segura + health (S68), observabilidad (S69), robustez de errores + headers + rate limiting (S70). Próximo (y posible cierre de la épica): **contenedor productivo** (Dockerfile de la API ya existe en supply-chain; falta validar el arranque productivo end-to-end) + readiness ampliado (S3, migraciones al desplegar).

## 7. Decisiones tomadas

- El manejador de errores devuelve un detalle **fijo genérico** (no condicionado al entorno): nunca filtra el stack, ni siquiera por error de configuración. El detalle real va al log, correlacionado.
- Rate limiting **sólo sobre `/api/v1/auth`** (el objetivo de fuerza bruta), no global, para no afectar el resto del tráfico; configurable por `RateLimit:Auth:*`.
- El endpoint de diagnóstico de excepciones se mapea **sólo en Development** (no llega a producción).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Review del Sprint 70 (hardening #3: manejador de errores + cabeceras de seguridad + rate limiting). Cumplido, velocity 8, 0 carry-over, 529 pruebas (+10). Generado por AG-07 |
