# Plan de pruebas — GeoVial

**Proyecto:** GeoVial
**Documento:** plan-pruebas_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Ingeniero QA / SDET Senior (AG-08), Equipo SDD 2.0

## 1. Alcance del plan

Este plan cubre la validación de GeoVial a lo largo de las fases del plan de desarrollo (PROJECT-README §4), arrancando por el Sprint 00 (walking skeleton) y el Sprint 01 (slice de jerarquía y manejo de usuarios) de 07, y extendiéndose por la matriz de cobertura a los catorce CU, las ocho RN y los NFR numéricos de 05.

| Incluido | Excluido |
| --- | --- |
| Dominio y aplicación (jerarquía, estados, agrupación por radio, georreferenciación, consolidación, idempotencia) | Captura automatizada por drones o estaciones (Won't Have v1, BRIEF §9) |
| API REST + persistencia SQL Server vía EF Core; contrato OpenAPI | Generación automática de informes de evaluación (excluida, BRIEF §9) |
| Front web Blazor (componentes con bUnit) | App móvil en plataformas distintas de Android (iOS/tablets fuera de v1, ADR-13) |
| App móvil Android: captura georreferenciada y sincronización offline | Distribución por tiendas de aplicaciones (fuera del ciclo de desarrollo, README §16) |
| Librería de sincronización `GeoVial.Sync` y librería de archivos `GeoVial.FileHosting` (backends local/S3) | Pruebas sobre integraciones con sistemas externos (no hay integración en v1, BRIEF §10) |
| Auditoría e inmutabilidad; autorización por rol y área; compliance Ley 25.326 | Límite numérico exacto de tamaño de foto (supuesto a validar, README §13) |

## 2. Criterios de entrada

El plan (o su porción por sprint) se ejecuta cuando:

1. Las US y BT del alcance cumplen la Definition of Ready de 06 (criterios de aceptación Given/When/Then presentes y verificables).
2. El build compila sin warnings tratados como error (gate de build, README §11 stage 2).
3. El ambiente de integración está disponible: Testcontainers con SQL Server levanta localmente y en CI, con fallback documentado a SQL Server local (sprint-00 §6).
4. El dataset sintético de prueba está generado y versionado (estrategia-testing §6).
5. Para la UI móvil, hay un dispositivo Android conectado por USB en modo desarrollador (README §11, §16).

## 3. Criterios de salida

El plan se declara ejecutado con éxito cuando:

1. Cobertura por capa alcanzada y gate global de CI verde: líneas ≥ 80%, branches ≥ 70% (README §9/§11), con los pisos por capa de estrategia-testing §2.
2. Cada CU crítico del alcance tiene al menos un TC verde que cubre sus criterios Given-When-Then (matriz-cobertura-pruebas, tabla CU↔Tests).
3. Cada RN del alcance tiene su TC verde (matriz, tabla RN↔Tests).
4. Cada NFR numérico del alcance valida su SLA con su tooling de medición (matriz, tabla NFR↔Tests); la disponibilidad se observa como SLO en 09, no como test unitario.
5. Defectos blockers (severidad alta que impide el flujo) cerrados; cada bug cerrado generó al menos un test de regresión (regla §4.10).
6. Suite de regresión verde: ningún test verde de la versión anterior pasó a rojo sin justificación documentada (criterios-validación §4).

## 4. Riesgos de calidad

Alineados con los riesgos de negocio del BRIEF §11 y los riesgos arquitectónicos de 05 §9.

| ID | Riesgo de calidad | Probabilidad | Impacto | Mitigación de testing | Origen |
| --- | --- | --- | --- | --- | --- |
| RC-Q1 | Marcadores duplicados en un mismo radio que generan datos inconsistentes | Media | Alto | Unit de agrupación por radio (RN-02, CU-11); integration de detección al sincronizar; TC de resolución unificar/separar (CU-12) | R-01 / RA-01 |
| RC-Q2 | Pérdida o duplicación de observaciones capturadas sin conexión | Media | Alto | TC de idempotencia de la cola (RC-03), de operación 8 h sin pérdida (CU-06) y de reanudación sin duplicar tras corte (CU-07 CA-03) | R-02 / RA-02 |
| RC-Q3 | Last-write-wins descarta cambios concurrentes sin dejar rastro | Media | Medio | TC de consolidación por marca temporal con marca de conflicto persistente (RN-04, CU-07 CA-02); verificación de que ningún descarte es silencioso | R-? / RA-03 |
| RC-Q4 | Georreferenciación de baja calidad cuando la foto no trae metadatos | Media | Medio | TC de cadena EXIF→manual→bandeja sin georreferenciar (RN-03, CU-05); métrica de ≥ 95% de coordenada automática | R-03 / RA-06 |
| RC-Q5 | Acceso o retención indebidos de datos personales (incumplimiento Ley 25.326) | Baja | Alto | TC de autorización por rol y área (RN-01, CU-14), de auditoría inmutable y retención ≥ 12 meses (RN-07), de acceso acotado a datos personales (RN-08) | RA-08 |
| RC-Q6 | Inestabilidad de Testcontainers en entornos locales del equipo | Media | Medio | Validación de la imagen en cada máquina el día 1; fallback a SQL Server local compartida (sprint-00 §6) | RA — operativo |
| RC-Q7 | UI móvil costosa y frágil de automatizar sobre Android por USB | Media | Medio | Acotar e2e móvil al 10% de la pirámide y a los flujos críticos (captura, sync); empujar la lógica a unit | RA-05 / README §16 |
| RC-Q8 | Breaking change no controlado en la API pública de `GeoVial.Sync` | Baja | Medio | Contract tests de la superficie pública de la librería; SemVer estricto (cualquier breaking change bumpea MAJOR) | RA-07 |

## 5. Plan por sprint

Cubre los sprints planificados en 07; las fases posteriores (PROJECT-README §4 fases 3-6) heredan los mismos criterios de entrada/salida y se incorporan a la matriz al planificarse.

| Sprint | Alcance de testing | Recursos | Entregables de testing |
| --- | --- | --- | --- |
| Sprint 00 (walking skeleton) | Pipeline de CI con gates de build y cobertura operativos; integración EF Core + SQL Server vía Testcontainers; smoke del recorrido command → handler → persistencia → API; dominio de Usuario/Área testeable sin infraestructura (BT-01) | QA part-time + 2 dev backend; Testcontainers; CI | Gates de CI configurados; primer test de integración verde; DoD canónica acordada y formalizada por el equipo |
| Sprint 01 (jerarquía y usuarios) | Unit de jerarquía y validación de nivel administrable (BT-02); autorización por rol y área (US-31, CU-14); login/relogueo y refresh condicionado (CU-02); alta/baja jerárquica y asociación a área (CU-03); componentes Blazor de administración (bUnit) | QA part-time + 2 dev backend + 1 dev frontend; CI; bUnit | TC de CU-02, CU-03, CU-14, RN-01, RN-06; suite unitaria de dominio/aplicación verde; matriz actualizada |
| Fases siguientes (4-6) | Relevamientos y asignación (CU-01), captura georreferenciada (CU-04/05), offline y sync (CU-06/07), revisión y export/import (CU-08), marcadores y carrusel (CU-09), estados (CU-10), conflictos (CU-11/12), auditoría (CU-13), backends de archivos | Equipo completo; dispositivo Android por USB; MAUI UI/Appium | TC restantes de la matriz; NFR de sincronización y latencia medidos; contract tests de FileHosting |

## 6. Recursos

- Personas: 4 integrantes (2 dev backend, 1 dev frontend, 1 QA part-time, AG-08 titular). Apoyo de AG-05 para validación de NFR y AG-09 para materializar los gates en el pipeline (09).
- Ambientes: CI en GitHub Actions; base efímera con Testcontainers (SQL Server); SQL Server local como fallback; dispositivo Android por USB para UI móvil; endpoint no productivo o doble compatible con S3 para contract tests de FileHosting.
- Datasets: dataset sintético de relevamientos y fotos (con y sin metadatos), lote de ≈100 observaciones con fotos para el NFR de sincronización, versionado en `tests/`.
- Herramientas: xUnit + FluentAssertions, WebApplicationFactory + Testcontainers, bUnit, .NET MAUI UI testing / Appium, Coverlet para cobertura por capa.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Plan de pruebas inicial: alcance, criterios de entrada/salida, riesgos de calidad alineados a BRIEF §11 y 05 §9, plan por sprint (Sprint 00 walking skeleton + Sprint 01 jerarquía/usuarios) y recursos. Generado por AG-08 |
