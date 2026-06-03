# Plan de Iteración — Sprint 37

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-37_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-11-02
**Fecha fin:** 2027-11-13
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S34–S36); capacidad sugerida estricta 9 SP. Se compromete automatizar el auto-merge de Dependabot para minor/patch que pasen CI (8 SP). DevOps; sin lógica de dominio nueva, el gate se mantiene.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Reducir el trabajo manual de triage de Dependabot (acción reiterada de S35/S36): un workflow que, para los PRs de Dependabot de tipo **minor/patch** que pasen CI, los apruebe y habilite **auto-merge**, dejando las **mayores** para revisión manual (criterio que ya evitó problemas reales: FluentAssertions 8 por licencia, AWSSDK.S3 v4 por migración).

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-DEPS-AUTOMERGE | Tarea | Workflow de auto-merge de Dependabot para minor/patch que pasen CI; majors a revisión manual | Media | 8 | DevOps (AG-09) | Pendiente |

Total de puntos comprometidos: 8 SP. Mantenimiento/automatización de supply-chain; sin cambios de backend ni dominio.

## 4. Alcance técnico

1. **`.github/workflows/dependabot-auto-merge.yml`**: dispara en `pull_request`; sólo para el actor `dependabot[bot]`. Lee el tipo de actualización con `dependabot/fetch-metadata`; si es `semver-minor` o `semver-patch`, **aprueba** el PR y habilita **auto-merge (squash)** con `gh pr merge --auto`. Los `semver-major` se omiten (quedan para revisión manual). Permisos mínimos: `contents: write`, `pull-requests: write`; usa el `GITHUB_TOKEN`.
2. **Prerrequisitos del repo (manuales, no se pueden fijar por código)**:
   - Habilitar "Allow auto-merge" en Settings → General.
   - Protección de rama en `main` con el check de `ci.yml` requerido, para que el auto-merge **espere a CI verde** antes de mergear (sin esto, `--auto` mergea apenas el PR es mergeable).
   Ambos se documentan en `supply-chain-seguridad`.
3. **Documentación**: `supply-chain-seguridad` a v1.5 (§4: auto-merge de minor/patch documentado, con los prerrequisitos y el límite de "majors a mano").

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica. Criterios específicos:

- El workflow auto-aprueba y habilita auto-merge **sólo** para minor/patch de Dependabot; los majors no se tocan.
- Permisos mínimos (`contents`/`pull-requests` write) y filtro por `dependabot[bot]`.
- Los prerrequisitos del repo (Allow auto-merge + branch protection con ci.yml) quedan documentados.
- La suite .NET (332) permanece verde; el gate no se ve afectado (DevOps, sin lógica nueva).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Auto-merge sin "Allow auto-merge" habilitado | Alta | Bajo | Documentado como prerrequisito; el workflow no rompe (el step falla y el PR queda manual) |
| Sin branch protection, el auto-merge no espera a CI | Media | Medio | Documentar la protección de rama con `ci.yml` requerido; sólo entonces el merge se gatilla con CI verde |
| Un minor/patch malo se auto-mergea | Baja | Medio | El gate de `ci.yml` (build + 332 pruebas + cobertura) es la barrera; los majors nunca se auto-mergean |
| El workflow no es testeable localmente | Alta | Bajo | Se revisa estructuralmente (patrón oficial de GitHub); se valida con el próximo PR de Dependabot |

## 7. Criterios de hecho del sprint

El Sprint 37 se considera completo cuando el workflow de auto-merge está en el repo (minor/patch → aprobar + auto-merge; majors a mano), los prerrequisitos del repo están documentados, la suite .NET sigue verde, y se facilitan el sprint review y la retrospectiva.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Política | supply-chain-seguridad §4 (dependency scanning / Dependabot) |
| Origen | retros S35/S36 (auto-merge de minor/patch que pasen CI) |
| Artefacto | `.github/workflows/dependabot-auto-merge.yml` |
| Calidad | definition-of-done §1.4 |
| Tests previstos | sin pruebas .NET nuevas; verificación = estructura del workflow + próximo PR de Dependabot |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Plan inicial del Sprint 37 (auto-merge de Dependabot para minor/patch que pasen CI; majors a revisión manual), con los prerrequisitos del repo documentados. Atiende la acción de S35/S36. Compromete 8 SP; sin lógica de dominio nueva. Generado por AG-07 |
