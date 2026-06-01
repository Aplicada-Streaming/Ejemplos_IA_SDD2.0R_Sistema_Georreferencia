# ADR-02 — Backend monolítico que expone API REST a web y móvil

**Proyecto:** GeoVial
**Documento:** ADR-02-backend-monolitico-expone-api-rest_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Categoría:** Comunicación

## 1. Contexto

El front web administrativo (Blazor Interactive Server) y la app móvil de captura (.NET MAUI) necesitan operar sobre el mismo modelo de relevamientos, observaciones y usuarios. La app móvil sincroniza cambios capturados sin conexión (CU-06, CU-07) y el front web revisa, exporta e importa (CU-08). El cliente decidió un backend monolítico que expone la superficie de API dentro del propio monolito (PROJECT-README §1, §6). Motivan esta decisión los CU-04 a CU-09 y los NFR de latencia API (p95 ≤ 500 ms) y disponibilidad (SLO 99%).

## 2. Decisión

El backend monolítico expone una API REST (JSON) versionada por URL `/api/v1/`, consumida por el front web Blazor y por la app móvil MAUI. La superficie de API queda contenida en el monolito (`GeoVial.Api`); no se publica un servicio de API separado. El contrato se documenta con OpenAPI 3.x generado desde la API.

## 3. Estado

Aceptado el 2026-06-01. Renumeración de `ADR-002` (PROJECT-README §15, decidido por el cliente) a `ADR-02`.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| API REST dentro del monolito (elegido) | Un solo artefacto; reuso del dominio; contrato OpenAPI estándar consumido por web y móvil | El front y la API comparten ciclo de vida de despliegue |
| Servicio de API separado del front | Escalado independiente del front | Sobredimensionado; el cliente pide monolito y tres contenedores |
| Comunicación por RPC/gRPC en lugar de REST | Contratos fuertes y binarios | Menor interoperabilidad con clientes web; el cliente fijó REST/JSON |

## 5. Consecuencias positivas

1. Un único contrato OpenAPI sirve a web y móvil, reduciendo divergencia.
2. Versionado por URL `/api/v1/` permite evolucionar sin romper consumidores.
3. El dominio se reutiliza entre los flujos de captura, sincronización y revisión.

## 6. Consecuencias negativas y trade-offs

1. La API y el resto del backend comparten despliegue; no escalan por separado. Aceptado por carga interna.
2. Un cambio incompatible de contrato obliga a una nueva versión de URL; se asume el costo de versionado.

## 7. Implementación

`GeoVial.Api` expone los endpoints REST bajo `/api/v1/`. El contrato se formaliza en `contratos-rest_v1.0.md` (OpenAPI 3.x, errores Problem Details RFC 7807 por ADR-11, contrato de exportación/importación en ZIP). La autenticación es ROPC/JWT (ADR-03).

## 8. Métricas de validación

- Latencia p95 de lecturas administrativas ≤ 500 ms.
- Contrato OpenAPI generado y versionado en el repositorio, validado en CI.
- Cobertura de los endpoints críticos por pruebas de integración (WebApplicationFactory).

## 9. Referencias

- PROJECT-README §1, §6 (comunicación e integración).
- CU-04, CU-05, CU-06, CU-07, CU-08, CU-09.
- ADR-03 (auth), ADR-11 (errores), `contratos-rest_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. Decisión aceptada. Renumeración de ADR-002 a ADR-02 |
