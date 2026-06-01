# 05 — Arquitectura técnica — GeoVial

**Proyecto:** GeoVial
**Tipo D8:** web-monolith (sub-proyectos: mobile-app-maui, library de sincronización, library de alojamiento)
**Versión:** 1.0
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0

Índice navegable de la arquitectura técnica de GeoVial. Punto de entrada para revisores (AG-02, AG-06, AG-08, AG-09).

## Documento maestro

- [arquitectura-solucion_v1.0.md](arquitectura-solucion_v1.0.md) — estilo monolito modular Clean Architecture + CQRS ligero; cuatro vistas (lógica, procesos, despliegue, datos); cross-cutting; NFR con métricas numéricas; riesgos; trazabilidad.

## Índice de ADRs

- [decisiones-arquitectura_v1.0.md](decisiones-arquitectura_v1.0.md) — índice y nota de mapeo de la renumeración pre-ADR (README §15 ADR-001..008 → ADR-01..08).

| ADR | Título | Categoría | Estado |
| --- | --- | --- | --- |
| [ADR-01](adrs/ADR-01-estilo-monolito-modular-clean-architecture_v1.0.md) | Estilo monolito modular Clean Architecture + CQRS ligero | Estilo | Aceptado |
| [ADR-02](adrs/ADR-02-backend-monolitico-expone-api-rest_v1.0.md) | Backend monolítico expone API REST | Comunicación | Aceptado |
| [ADR-03](adrs/ADR-03-autenticacion-ropc-jwt-refresh-condicionado_v1.0.md) | Autenticación ROPC + JWT con refresh condicionado | Seguridad | Aceptado |
| [ADR-04](adrs/ADR-04-mapas-openstreetmap-leaflet_v1.0.md) | Mapas OpenStreetMap + Leaflet | Comunicación | Aceptado |
| [ADR-05](adrs/ADR-05-sqlite-cola-cambios-offline_v1.0.md) | SQLite + cola de cambios offline | Persistencia | Aceptado |
| [ADR-06](adrs/ADR-06-conflictos-last-write-wins-override-manual_v1.0.md) | Last-write-wins con override manual | Persistencia | Aceptado |
| [ADR-07](adrs/ADR-07-libreria-sincronizacion-github-packages_v1.0.md) | Librería de sincronización en GitHub Packages | Extensibilidad | Aceptado |
| [ADR-08](adrs/ADR-08-libreria-alojamiento-backends-configurables_v1.0.md) | Librería de alojamiento con backends configurables | Extensibilidad | Aceptado |
| [ADR-09](adrs/ADR-09-persistencia-backend-sql-server-ef-core_v1.0.md) | Persistencia backend SQL Server + EF Core | Persistencia | Aceptado |
| [ADR-10](adrs/ADR-10-separacion-capas-clean-architecture_v1.0.md) | Separación de capas Clean Architecture | Estilo | Aceptado |
| [ADR-11](adrs/ADR-11-manejo-errores-problem-details_v1.0.md) | Manejo de errores Problem Details RFC 7807 | Observabilidad | Aceptado |
| [ADR-12](adrs/ADR-12-omision-categoria-04-sin-llm_v1.0.md) | Omisión de la categoría 04 (sin LLM en v1) | Estilo | Aceptado |
| [ADR-13](adrs/ADR-13-compatibilidad-plataformas_v1.0.md) | Compatibilidad de plataformas | Despliegue | Aceptado |
| [ADR-14](adrs/ADR-14-compliance-ley-25326_v1.0.md) | Compliance Ley 25.326 | Seguridad | Aceptado |

Total: 14 ADR vigentes, todas `Aceptado`. Ninguna superada ni rechazada.

## Modelo de datos lógico

- [modelo-datos-logico_v1.0.md](modelo-datos-logico_v1.0.md) — 12 entidades con tipos físicos (SQL Server backend + SQLite cola local), índices, restricciones, migración inicial `20260601_InitialCreate` y trazabilidad al modelo conceptual de 02. multi_tenant=false.

## Flujo de ejecución

- [flujo-ejecucion_v1.0.md](flujo-ejecucion_v1.0.md) — pipeline de sincronización offline-first con last-write-wins y resolución de conflictos; flujo de georreferenciación EXIF→manual→bandeja sin georreferenciar.

## Contratos externos

- [contratos-rest_v1.0.md](contratos-rest_v1.0.md) — API REST OpenAPI 3.x, versionado por URL `/api/v1/`, errores Problem Details RFC 7807, contrato de exportación/importación en ZIP.
- [contratos-abstractions-sync_v1.0.md](contratos-abstractions-sync_v1.0.md) — superficie pública de la librería de sincronización; política SemVer (breaking change → MAJOR).

## Extensibilidad

- [extensibilidad_v1.0.md](extensibilidad_v1.0.md) — puntos de extensión de la librería de alojamiento de archivos (backends configurables local/S3/otro), contrato de backend, registro por configuración del usuario raíz.

## NFR (resumen)

| NFR | Objetivo | ADR |
| --- | --- | --- |
| Operación offline | ≥ 8 h | ADR-05 |
| Tiempo de sincronización | ≤ 5 min / ≈100 obs | ADR-05, ADR-06 |
| Latencia API (lecturas) | p95 ≤ 500 ms | ADR-02, ADR-09 |
| Disponibilidad backend | SLO 99% | ADR-13 |
| Confiabilidad georreferenciación | ≥ 95% | ADR-04 |

Detalle y mecanismos de medición en `arquitectura-solucion_v1.0.md` §8.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | README inicial de la sección 05. Generado por AG-05 |
