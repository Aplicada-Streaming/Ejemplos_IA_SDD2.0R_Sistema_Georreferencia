# Plan de Iteración — Sprint 29

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-29_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-07-13
**Fecha fin:** 2027-07-24
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S26–S28); capacidad sugerida estricta 9 SP. Se compromete la publicación efectiva del primer release stable `v1.0.0` (8 SP): el acto del Release manager que cierra EP-09. Es DevOps/release; no hay lógica de dominio nueva.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Publicar el primer release **stable** `v1.0.0`: crear y empujar el tag `v1.0.0` sobre `main`, que dispara los dos workflows de publicación —`publish-sync.yml` (paquete `GeoVial.Sync` a GitHub Packages, con SBOM + firma) y `publish-images.yml` (las tres imágenes Docker a GHCR, con SBOM + firma)— y deja los artefactos firmados y verificables. Cierra EP-09 y la acción reiterada en las retros S25/S27/S28 (publicar v1.0.0). MinVer toma la versión del tag (`MinVerTagPrefix=v`) → canal stable.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-RELEASE-1.0.0 | Tarea | Tag `v1.0.0` sobre `main` + verificación de que disparó los workflows de publicación; cierre del CHANGELOG y del release doc | Alta | 8 | Release manager (AG-09) | Pendiente |

Total de puntos comprometidos: 8 SP. No agrega historias nuevas: es el acto de release del trabajo ya entregado (paquete preparado en S21, firmado en S22, con SBOM en S25; imágenes con SBOM + firma en S28). A partir de `v1.0.0` la superficie pública `Abstractions` queda congelada (ADR-07; todo breaking change bumpea MAJOR).

## 4. Alcance técnico

1. **Pre-condiciones** (checklist del Release manager, `release-libreria-sync-v1.0.0 §2`): `main` verde en CI (build + tests + cobertura) y CHANGELOG con la sección `[1.0.0]`. Verificadas antes de taggear.
2. **Tag `v1.0.0`** sobre `main`, empujado a `origin`. Es un tag anotado con la nota de release.
3. **Disparo automático**: el push del tag `v*` dispara `publish-sync.yml` (STAGE-11/09/10/13) y `publish-images.yml` (STAGE-12/09/10/14). Se verifica que ambos workflows arrancaron.
4. **Cierre documental**: CHANGELOG `[1.0.0]` con la fecha de release; `release-libreria-sync-v1.0.0` a estado *Released* con nota de los dos workflows disparados.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- El tag `v1.0.0` existe en `origin`, sobre un `main` verde en CI.
- El push del tag dispara los dos workflows de publicación (paquete + imágenes); se evidencia su arranque.
- El CHANGELOG `[1.0.0]` queda fechado y el release doc a *Released*.
- La superficie pública `Abstractions` se documenta como congelada para `v1.0.0`.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Un workflow de publicación falla en Actions (secretos/OIDC/registro) | Media | Medio | Se verifica el arranque; ante fallo, se revisa el log y se republica (el tag no se reescribe; un fix sale como `v1.0.1`) |
| Publicación pública irreversible por error de versión | Baja | Alto | Pre-condiciones verificadas (CI verde, CHANGELOG); no se reescribe una versión publicada; rollback por delist/deprecate + PATCH (guia-publicacion-paquete §4) |
| Desfase entre el código tagueado y el CHANGELOG fechado | Baja | Bajo | El binario lo determina el commit/tag, no el CHANGELOG; el fechado es documental y se registra en el cierre |

## 7. Criterios de hecho del sprint

El Sprint 29 se considera completo cuando el tag `v1.0.0` está en `origin` sobre un `main` verde, los dos workflows de publicación arrancaron por el tag, el CHANGELOG `[1.0.0]` quedó fechado y el release doc a *Released*, y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Épica | EP-09 (librería de sincronización publicable) |
| ADR | ADR-07 (publicación/versionado; congelamiento de la superficie en MAJOR) |
| Pipeline | publish-sync.yml (paquete) + publish-images.yml (imágenes); STAGE-09/10/11/12/13/14 |
| Release | release-libreria-sync-v1.0.0; estrategia-versionado §3/§5 (canal stable por tag sin sufijo) |
| Calidad | definition-of-done §1.4; retros S25/S27/S28 (publicar v1.0.0) |
| Tests previstos | sin pruebas .NET nuevas; verificación = arranque de los workflows + artefactos firmados |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 29 (release v1.0.0): tag `v1.0.0` sobre `main` que dispara la publicación del paquete y de las imágenes (con SBOM + firma), cierre del CHANGELOG y del release doc. Compromete 8 SP; cierra EP-09. Generado por AG-07 |
