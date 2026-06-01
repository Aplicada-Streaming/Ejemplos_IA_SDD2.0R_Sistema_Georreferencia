# Decisiones de arquitectura — Índice de ADRs — GeoVial

**Proyecto:** GeoVial
**Documento:** decisiones-arquitectura_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0

## 1. Objetivo

Índice navegable de los Architecture Decision Records de GeoVial. No contiene el cuerpo de las decisiones: cada ADR vive en un archivo individual bajo `adrs/`, con su estado declarado e inmutable una vez aceptado (regla §3.3). Si una decisión evoluciona, se crea una ADR nueva y la anterior pasa a `Superado por ADR-YY` sin reescribirse.

## 2. Índice de ADRs

| ADR | Título | Categoría | Estado | Fecha |
| --- | --- | --- | --- | --- |
| ADR-01 | Estilo arquitectónico: monolito modular con Clean Architecture y CQRS ligero | Estilo | Aceptado | 2026-06-01 |
| ADR-02 | Backend monolítico que expone API REST a web y móvil | Comunicación | Aceptado | 2026-06-01 |
| ADR-03 | Autenticación ROPC + JWT bearer con refresh condicionado al método de seguridad del teléfono | Seguridad | Aceptado | 2026-06-01 |
| ADR-04 | Mapas con OpenStreetMap + Leaflet en web y móvil | Comunicación | Aceptado | 2026-06-01 |
| ADR-05 | SQLite + cola de cambios para soporte sin conexión en móvil | Persistencia | Aceptado | 2026-06-01 |
| ADR-06 | Resolución de conflictos last-write-wins con override manual desde la web | Persistencia | Aceptado | 2026-06-01 |
| ADR-07 | Librería de sincronización como paquete en GitHub Packages | Extensibilidad | Aceptado | 2026-06-01 |
| ADR-08 | Librería de alojamiento de archivos con backends configurables (local / S3 / otro) | Extensibilidad | Aceptado | 2026-06-01 |
| ADR-09 | Persistencia backend con SQL Server y EF Core | Persistencia | Aceptado | 2026-06-01 |
| ADR-10 | Separación de capas Clean Architecture (Domain/Application/Infrastructure/Web-API) | Estilo | Aceptado | 2026-06-01 |
| ADR-11 | Manejo de errores con Problem Details RFC 7807 | Observabilidad | Aceptado | 2026-06-01 |
| ADR-12 | Omisión de la categoría 04 (no se incorpora LLM en v1) | Estilo | Aceptado | 2026-06-01 |
| ADR-13 | Compatibilidad de plataformas (Android 8.0+, navegadores evergreen, iOS fuera de v1) | Despliegue | Aceptado | 2026-06-01 |
| ADR-14 | Compliance Ley 25.326 (retención de auditoría, tratamiento y minimización de datos personales) | Seguridad | Aceptado | 2026-06-01 |

Total: 14 ADR, todas en estado `Aceptado`. Ninguna superada ni rechazada a la fecha.

## 3. Nota de mapeo de la renumeración pre-ADR

PROJECT-README §15 rotula los ocho pre-ADR como `ADR-001` a `ADR-008` (tres dígitos). Por la convención de nomenclatura de dos dígitos de la categoría 05 (§3.1, IDs `ADR-01`..`ADR-NN`), se renumeran a dos dígitos. El mapeo es uno a uno y conserva el orden:

| Pre-ADR (PROJECT-README §15) | ADR (categoría 05) | Estado original | Estado en 05 |
| --- | --- | --- | --- |
| ADR-001 — Estilo monolito modular Clean Arch + CQRS ligero | ADR-01 | Propuesto | Aceptado |
| ADR-002 — Backend monolítico expone API REST | ADR-02 | Decidido por el cliente | Aceptado |
| ADR-003 — Autenticación ROPC + JWT con refresh condicionado | ADR-03 | Propuesto | Aceptado |
| ADR-004 — Mapas OpenStreetMap + Leaflet | ADR-04 | Decidido por el cliente | Aceptado |
| ADR-005 — SQLite + cola de cambios offline | ADR-05 | Propuesto | Aceptado |
| ADR-006 — Last-write-wins con override manual | ADR-06 | Propuesto | Aceptado |
| ADR-007 — Librería de sincronización en GitHub Packages | ADR-07 | Decidido por el cliente | Aceptado |
| ADR-008 — Librería de alojamiento con backends configurables | ADR-08 | Decidido por el cliente | Aceptado |

Los pre-ADR marcados como "Decidido por el cliente" y los "Propuesto" ya validados durante el intake se registran como `Aceptado`. Las ADR-09 a ADR-14 no provienen de §15: las agrega ingeniería para completar las cinco categorías obligatorias de `web-monolith` (§2.2: estilo, persistencia, autenticación, separación de capas, manejo de errores) y la gobernanza (omisión de 04, compatibilidad de plataformas, compliance Ley 25.326).

## 4. Cobertura de las categorías obligatorias web-monolith

| Categoría obligatoria (§2.2) | ADR que la cubre |
| --- | --- |
| Estilo | ADR-01 (con ADR-10, ADR-12) |
| Persistencia | ADR-09 (backend), ADR-05 (móvil) |
| Autenticación | ADR-03 |
| Separación de capas | ADR-10 |
| Manejo de errores | ADR-11 |

## 5. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Índice inicial con 14 ADR y nota de mapeo de la renumeración pre-ADR (ADR-001..008 → ADR-01..08). Generado por AG-05 |
