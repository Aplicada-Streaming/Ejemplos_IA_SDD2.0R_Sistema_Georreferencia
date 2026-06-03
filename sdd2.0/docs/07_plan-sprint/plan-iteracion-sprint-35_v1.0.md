# Plan de Iteración — Sprint 35

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-35_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-10-05
**Fecha fin:** 2027-10-16
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S32–S34); capacidad sugerida estricta 9 SP. Se compromete el **triage de los PRs de Dependabot** (8 SP): integrar las actualizaciones seguras y decidir sobre las mayores con criterio. Mantenimiento; sin lógica de dominio nueva.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Consumir el primer lote de Dependabot (validó la configuración del Sprint 33: abrió 7 PRs, #37–#43): integrar las actualizaciones de bajo riesgo verificándolas con el gate, y **decidir con criterio** las mayores —no auto-mergear breaking changes—. En particular, se detecta que **FluentAssertions 8 cambió a licencia comercial**; se declina y se queda en la última 7.x libre.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-DEPS-TRIAGE | Tarea | Triage del primer lote de Dependabot: integrar lo seguro, diferir/declinar las mayores con justificación | Media | 8 | DevOps (AG-09) | Pendiente |

Total de puntos comprometidos: 8 SP. Mantenimiento de dependencias; sin cambios de dominio. Verificable por el gate (build + 332 pruebas + cobertura).

## 4. Alcance técnico

**Integrar (seguro):**

1. **#39 grupo minor/patch** (NuGet): AWSSDK.S3 3.7.405.4→3.7.511.8 (sigue en v3), SkiaSharp 2.88.8→2.88.9 (×2), Microsoft.IdentityModel.JsonWebTokens 8.18.0→8.19.1, Microsoft.Data.Sqlite 10.0.0→10.0.8, FluentAssertions 7.2.0→7.2.2 (última 7.x libre), xunit.runner.visualstudio 3.1.4→3.1.5.
2. **#41 coverlet.collector** 6.0.4→10.0.1 y **#43 Microsoft.NET.Test.Sdk** 17.14.1→18.6.0 (tooling de tests; verificado por el gate).
3. **#38 grupo de Actions**: checkout v4→v6, setup-dotnet v4→v5, upload-artifact v4→v7, docker/login-action v3→v4, docker/setup-buildx-action v3→v4, docker/build-push-action v6→v7. Los bumps de `ci.yml` los valida el propio PR (corre en `pull_request`); los de los workflows de publicación se validan en el próximo release con un tag `-rc` (checklist de release).
4. **#37 Docker**: imagen de la db `mssql/server` 2022-latest→2025-latest (imagen de deploy; bajo riesgo).

**Diferir:**

5. **#40 AWSSDK.S3 4.0.24** (mayor): salto de major del SDK de AWS (el backend de fotos S3 lo usa). Requiere migración y prueba real contra S3; se queda en v3 (último patch de #39) y se difiere a un sprint dedicado.

**Declinar:**

6. **#42 FluentAssertions 8.10.0** (mayor): la v8 pasó a **licencia comercial** (de pago para uso comercial) y trae cambios de API. Se declina; se queda en la última 7.x libre (7.2.2). Se cierra el PR con la justificación.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica. Criterios específicos:

- Las actualizaciones seguras quedan integradas y la suite (332) + cobertura del gate siguen verdes.
- Las mayores diferidas/declinadas quedan documentadas con su justificación; los PRs de Dependabot consumidos se cierran.
- Build Release sin warnings tratados como error (las nuevas versiones no introducen warnings que rompan el gate).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Un bump menor introduce un warning bajo `TreatWarningsAsErrors` | Media | Bajo | Se compila y testea localmente antes de integrar; se revierte el bump problemático |
| Los bumps de Actions de los workflows de publicación sólo se validan en tag | Media | Bajo | El checklist de release exige un `-rc` antes del stable; los de `ci.yml` los valida este PR |
| FluentAssertions 8 (licencia) entra por descuido | Baja | Medio | Se declina explícitamente y se queda en 7.2.2; se documenta |

## 7. Criterios de hecho del sprint

El Sprint 35 se considera completo cuando las actualizaciones seguras están integradas con el gate verde (332 pruebas + cobertura), las mayores diferidas/declinadas están documentadas con su justificación (FluentAssertions por licencia, AWSSDK.S3 por migración), los PRs de Dependabot consumidos están cerrados, y se facilitan el sprint review y la retrospectiva.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Política | supply-chain-seguridad §4 (Dependabot), §6 (CVE) |
| Origen | PRs de Dependabot #37–#43 (primer lote tras la config de S33) |
| Calidad | definition-of-done §1; retro S33/S34 (atender PRs de Dependabot) |
| Tests previstos | sin pruebas nuevas; verificación = gate verde con las versiones actualizadas |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Plan inicial del Sprint 35 (triage del primer lote de Dependabot): integrar minor/patch + Actions + Docker + tooling de tests; diferir AWSSDK.S3 v4; declinar FluentAssertions v8 (licencia comercial). Compromete 8 SP; sin lógica de dominio nueva. Generado por AG-07 |
