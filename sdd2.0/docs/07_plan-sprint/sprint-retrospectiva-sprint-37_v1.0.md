# Sprint Retrospectiva — Sprint 37

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-37_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- El auto-merge cierra el ciclo de mantenimiento de dependencias: configurar (S33) → consumir el primer lote (S35) → migrar el major (S36) → automatizar lo rutinario (S37).
- Se usó el patrón oficial de GitHub (fetch-metadata + `gh pr merge --auto`) con permisos mínimos; poco código, bajo riesgo.
- Se mantuvo la línea que ya pagó: los majors nunca se auto-mergean; sólo minor/patch con CI verde.
- Documentar los prerrequisitos del repo (Allow auto-merge, branch protection) evitó vender una automatización a medias.

## 2. Qué no salió bien

- El workflow depende de dos settings del repo que no se pueden fijar por código (Allow auto-merge + branch protection con `ci.yml` requerido); si el admin no los habilita, el auto-merge no opera (queda manual, sin romper).
- No es verificable localmente: su efecto real se verá recién con el próximo PR minor/patch de Dependabot.
- Sin branch protection, `--auto` podría mergear apenas el PR es mergeable, sin esperar a CI; por eso la protección de rama es parte del prerrequisito, no opcional.

## 3. Qué probar

- Confirmar, con el próximo lote semanal de Dependabot, que un minor/patch se auto-aprueba, espera a `ci.yml` verde y se mergea solo; y que un major queda manual.
- Habilitar la protección de rama de `main` con `ci.yml` requerido (cierra el flanco de "mergea sin esperar CI").
- Considerar una etiqueta/recordatorio para los majors abiertos, para que no se acumulen sin revisión.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Habilitar "Allow auto-merge" y branch protection (ci.yml requerido) en el repo | AG-09 (DevOps/admin) | 2027-11-27 | Pendiente |
| Verificar el auto-merge con el próximo PR minor/patch de Dependabot | AG-09 (DevOps) | 2027-11-27 | Pendiente |
| Mejoras del mapa: offline de teselas en el móvil | AG-08 (móvil) | 2027-11-27 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 36 | Estado actual |
| --- | --- |
| Evaluar prueba de `AlmacenS3` contra LocalStack | Pendiente (se reitera) |
| Auto-merge de Dependabot para minor/patch que pasen CI | Completada (entregada en este sprint; pendiente habilitar los settings del repo) |
| Mejoras del mapa: offline de teselas en el móvil | Pendiente (se reitera) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Retrospectiva del Sprint 37 (auto-merge de Dependabot): cierra el ciclo de mantenimiento de dependencias; 3 acciones nuevas (settings del repo, verificación, mapa móvil). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
