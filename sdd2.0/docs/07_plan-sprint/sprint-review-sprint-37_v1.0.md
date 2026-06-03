# Sprint Review — Sprint 37

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-37_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-37_v1.0.md`:

> Un workflow que, para los PRs de Dependabot de tipo **minor/patch** que pasen CI, los apruebe y habilite **auto-merge**, dejando las **mayores** para revisión manual.

Veredicto: Cumplido.

Explicación corta: se agregó `.github/workflows/dependabot-auto-merge.yml` (patrón oficial de GitHub): dispara en `pull_request`, sólo para el actor `dependabot[bot]`, lee el tipo de actualización con `dependabot/fetch-metadata` y, si es `semver-minor` o `semver-patch`, **aprueba** el PR y habilita **auto-merge (squash)** con `gh pr merge --auto` (permisos mínimos `contents`/`pull-requests` write, `GITHUB_TOKEN`). Los `semver-major` quedan fuera del `if` → **no se auto-mergean**, conservando la revisión manual que ya evitó problemas reales (FluentAssertions 8 por licencia, AWSSDK.S3 v4 por migración). El gate de `ci.yml` es la barrera previa al merge. Se documentaron en `supply-chain-seguridad` (v1.5) los prerrequisitos del repo que no se pueden fijar por código: "Allow auto-merge" y la protección de `main` con `ci.yml` requerido (para que el auto-merge **espere a CI verde**). Sin cambios de backend ni dominio.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| EP-09 | Supply-chain | Workflow que auto-aprueba + auto-mergea minor/patch de Dependabot que pasen CI | Menos triage manual |
| EP-09 | Criterio | Los majors no se auto-mergean: quedan para revisión manual | Mantiene el control sobre breaking changes |
| EP-09 | Documentación | Prerrequisitos del repo (Allow auto-merge + branch protection) documentados | Setup reproducible |

## 3. Feedback recibido

- El auto-merge ataca el dolor concreto de S35 (triage manual de 7 PRs): los minor/patch fluyen solos con CI verde.
- Mantener los majors a mano preserva el criterio que ya pagó (dos majors riesgosos detenidos en S35/S36).
- Ser explícito sobre los prerrequisitos manuales (Allow auto-merge, branch protection) evita la falsa sensación de "todo automatizado" sin la config del repo.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 332 verdes (295 unitarias + 37 de integración), sin cambio: es un sprint de DevOps/automatización sin lógica de dominio nueva, por lo que el gate de cobertura se mantiene. Verificación específica: revisión estructural del workflow (patrón oficial de GitHub); su efecto real se observará con el próximo PR de Dependabot. Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-DEPS-AUTOMERGE | Tarea | Aceptada (workflow de auto-merge minor/patch; majors a mano; prerrequisitos documentados) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 37 se traslada. |

Backlog restante: habilitar los prerrequisitos del repo (acto del admin: Allow auto-merge + branch protection); mejoras opcionales del mapa (offline de teselas en el móvil); prueba de S3 contra LocalStack; y el mantenimiento continuo.

## 7. Decisiones tomadas durante el review

- Usar el patrón oficial de GitHub (fetch-metadata + `gh pr merge --auto`) con permisos mínimos.
- Auto-mergear **sólo** minor/patch; los majors nunca (revisión manual obligatoria).
- Documentar los prerrequisitos del repo en vez de pretender que el workflow basta por sí solo.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Sprint review del Sprint 37 (auto-merge de Dependabot para minor/patch). Veredicto Cumplido, velocity 8, 0 carry-over, 332 pruebas verdes. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
