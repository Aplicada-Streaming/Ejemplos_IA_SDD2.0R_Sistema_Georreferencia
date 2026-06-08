# Despliegue en producción — GeoVial backend

**Proyecto:** GeoVial
**Documento:** despliegue-produccion_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-08
**Autor:** AG-09 (DevOps), Equipo SDD 2.0

> Runbook del backend `GeoVial.Api` en producción. Consolida el hardening de la épica (S68–S71): configuración segura, observabilidad, robustez y despliegue de migraciones. La imagen del contenedor del backend se construye con el `Dockerfile` multi-stage de supply-chain (S28) y se publica firmada a GHCR (S29/S33).

## 1. Configuración (secretos por entorno, NO en el repo)

El arranque **falla rápido** (`GuardrailsProduccion`, S68) si en `Production` falta algo de esto:

| Variable de entorno | Obligatoria | Descripción |
| --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT=Production` | sí | Activa los guardrails, los logs JSON y desactiva el auto-migrate. |
| `ConnectionStrings__GeoVial` | sí | Cadena de SQL Server real (no se permite la base en memoria). |
| `Jwt__ClaveSecreta` | sí | Clave HS256 propia, ≥ 32 caracteres, **distinta** de la de desarrollo. |
| `Almacen__Backend` / `Almacen__BucketS3` / `Almacen__RegionS3` | según backend | `Local` o `S3` (ADR-08). |
| `RateLimit__Auth__PermitLimit` / `RateLimit__Auth__VentanaSegundos` | no | Límite de `/api/v1/auth` (default 60/60s, S70). |

`appsettings.Production.json` queda en el repo **sin secretos** (sólo logging); los secretos van por variable de entorno o secret manager.

## 2. Orden de despliegue

1. **Migrar la base (paso explícito, una sola vez por release).** En producción el backend **no** auto-migra al arrancar (varias réplicas migrando a la vez es frágil; `PoliticaMigracion`, S71). Correr un job / init-container con el comando `migrate`, que aplica las migraciones y **termina**:

   ```bash
   # con la misma imagen y env del backend
   dotnet GeoVial.Api.dll migrate
   ```

2. **Levantar las instancias del backend** (las que sirven tráfico). Arrancan sin migrar; si el esquema no está al día, el readiness no da OK (ver §3).

3. **Habilitar el tráfico** cuando el readiness esté Healthy.

## 3. Sondas de salud (orquestador / balanceador)

| Endpoint | Tipo | Da OK cuando | Uso |
| --- | --- | --- | --- |
| `GET /health` | liveness | el proceso responde | reiniciar el pod si falla |
| `GET /health/ready` | readiness | la base es **alcanzable** y **sin migraciones pendientes** | no enrutar tráfico hasta que dé OK |

Ambos son **anónimos**. El readiness ampliado (S71) impide servir tráfico con el esquema desactualizado (entre el deploy del código nuevo y la corrida del job de migración).

## 4. Observabilidad (S69)

- **Logs estructurados (JSON)** fuera de Development, listos para un agregador (ELK / Loki / CloudWatch).
- **Id de correlación** por request (`X-Correlation-ID`, reusado o generado) en el scope de logging y en la respuesta: agrupa todos los logs de una petición. Un error 500 al cliente es genérico (sin stack, S70) pero rastreable por su id en los logs.

## 5. Seguridad (S70)

- Excepciones no controladas → `ProblemDetails` 500 **genérico** (sin filtrar mensaje/stack).
- Cabeceras de seguridad en toda respuesta (`X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy`, `Cross-Origin-Resource-Policy`).
- Rate limiting de `/api/v1/auth` (429). Detrás de un proxy/balanceador, configurar `ForwardedHeaders` para que la IP del cliente sea la real (pendiente; ver retro S70).

## 6. Pendientes (backlog post-épica)

- Rate limiting **distribuido** (Redis) y `ForwardedHeaders` para despliegue multi-réplica detrás de proxy.
- **OpenTelemetry** (tracing/métricas) si se requiere correlación entre servicios.
- Readiness del **almacenamiento de fotos** (S3) además de la base.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Runbook inicial de despliegue en producción del backend. Consolida el hardening S68–S71 (config segura + fail-fast, observabilidad, seguridad, migraciones como paso de despliegue + readiness ampliado). Generado por AG-09 |
