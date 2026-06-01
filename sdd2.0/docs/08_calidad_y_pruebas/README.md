# 08 Calidad y pruebas — GeoVial

**Proyecto:** GeoVial
**Tipo de proyecto:** web-monolith
**Pirámide objetivo:** 70 unit / 20 integración / 10 componente-UI
**Fecha:** 2026-06-01
**Autor:** Ingeniero QA / SDET Senior (AG-08), Equipo SDD 2.0

Índice navegable de los artefactos de calidad y pruebas de GeoVial para revisores (AG-02, AG-05, AG-06, AG-07, AG-09, AG-10).

## Artefactos

| Artefacto | Estado | Descripción |
| --- | --- | --- |
| [estrategia-calidad_v1.0.md](estrategia-calidad_v1.0.md) | Vigente | Definición de calidad, atributos ISO/IEC 25010 priorizados con NFR de origen, quality gates, roles QA y cadencia. |
| [estrategia-testing_v1.0.md](estrategia-testing_v1.0.md) | Vigente | Pirámide 70/20/10 justificada, cobertura por capa, tooling real, BDD, mocks/fixtures, dataset sintético y ambiente efímero. |
| [plan-pruebas_v1.0.md](plan-pruebas_v1.0.md) | Vigente | Alcance, criterios de entrada/salida, riesgos de calidad, plan por sprint (Sprint 00 + Sprint 01) y recursos. |
| [matriz-cobertura-pruebas_v1.0.md](matriz-cobertura-pruebas_v1.0.md) | Vigente | Tres tablas obligatorias (CU↔Tests, NFR↔Tests, RN↔Tests), cobertura por capa y gaps. |
| [casos-prueba-referenciales_v1.0.md](casos-prueba-referenciales_v1.0.md) | Vigente | Catálogo de 26 TC con setup, pasos Given-When-Then, expected, actual (pendiente) y status. |
| [criterios-validacion_v1.0.md](criterios-validacion_v1.0.md) | Vigente | Criterios funcionales, no funcionales, regresión, calidad de código y excepciones para declarar el sistema validado. |
| [definition-of-done_v1.0.md](definition-of-done_v1.0.md) | Vigente | DoD canónica por capa (US, BT, sprint, release). Fuente única referenciada por 07. |
| [guia-testing-extensibilidad_v1.0.md](guia-testing-extensibilidad_v1.0.md) | Vigente | Contract tests de los backends configurables de `GeoVial.FileHosting` sin modificar el núcleo. |

## Definition of Done canónica

La DoD canónica del proyecto vive en [definition-of-done_v1.0.md](definition-of-done_v1.0.md). Es la fuente única: los planes de sprint de 07 (Sprint 00 y Sprint 01) la referencian por nombre y ubicación y no la redefinen. No solapa la Definition of Ready de 06 (la DoR es el filtro de entrada; la DoD es el filtro de salida).

## Quality gates configurados en CI

Materializados como stages bloqueantes del pipeline en 09 (PROJECT-README §11):

- Build sin warnings tratados como error (stage Build).
- Cobertura de líneas ≥ 80% y branches ≥ 70% (stage Tests), con pisos por capa: dominio 85/75, aplicación 80/70, infraestructura 70/60, presentación 60/50.
- Tests unitarios + integración 100% verdes (xUnit; WebApplicationFactory + Testcontainers con SQL Server).
- Contrato OpenAPI 3.x generado coincide con la API y está versionado.
- Conformidad de licencias (MIT/Apache/BSD; sin GPL en componentes distribuidos).

## Conteo y cobertura

- Casos de prueba referenciales: 26 (TC-01 a TC-26), numeración contigua de dos dígitos.
- CU cubiertos: 14 de 14 (CU-01..14) con al menos un TC por sus criterios Given-When-Then.
- RN cubiertas: 8 de 8 (RN-01..08).
- NFR numéricos de 05 §8 con test asociado: todos; disponibilidad y confiabilidad de georreferenciación se observan además como SLO/métrica en 09.

## Tooling de testing (PROJECT-README §9)

- Unit: xUnit + FluentAssertions.
- Integración: WebApplicationFactory + Testcontainers (SQL Server).
- Componente Blazor: bUnit.
- UI móvil: .NET MAUI UI testing / Appium sobre Android.
- Cobertura por capa: Coverlet.
