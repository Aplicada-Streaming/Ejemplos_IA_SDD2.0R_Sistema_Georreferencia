# Plan de Iteración — Sprint 28

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-28_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-06-29
**Fecha fin:** 2027-07-10
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S25–S27); capacidad sugerida estricta 9 SP. Se compromete el endurecimiento supply-chain de las imágenes Docker del monolito (8 SP): SBOM CycloneDX + firma cosign de las tres imágenes. Es trabajo DevOps; no hay lógica de dominio nueva, por lo que el gate de cobertura .NET se mantiene sin cambios.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Extender el endurecimiento de supply-chain (ya completo para el paquete `GeoVial.Sync` en S22/S25) a las tres imágenes Docker del monolito (front/backend/db): generar un SBOM CycloneDX por imagen (STAGE-09) y firmar cada imagen y su SBOM con cosign keyless (STAGE-10), verificándolos en el pipeline antes de publicar a GHCR (STAGE-14). Cierra la acción reiterada en las retros S25/S27 y deja `supply-chain-seguridad §1/§2` implementado también para las imágenes.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-SUPPLY-IMG | Tarea | SBOM CycloneDX + firma cosign de las imágenes Docker del monolito en el pipeline de publicación de imágenes | Media | 8 | DevOps (AG-09) / Dev backend | Pendiente |

Total de puntos comprometidos: 8 SP. No agrega historias nuevas: completa el endurecimiento supply-chain del artefacto desplegable (las imágenes), análogo a lo hecho para el paquete. Incluye, como prerrequisito, los Dockerfiles multi-stage que la `guia-publicacion-image-docker` ya referencia y que aún no existían en el repo.

## 4. Alcance técnico

1. **Dockerfiles multi-stage** (prerrequisito; `guia-publicacion-image-docker §1`):
   - `src/GeoVial.Api/Dockerfile` (backend): build con el SDK .NET 10, runtime ASP.NET; expone 8080.
   - `src/GeoVial.Web/Dockerfile` (front, Blazor Interactive Server): build con el SDK, runtime ASP.NET; expone 8080.
   - `infra/db/Dockerfile` (db): parte de la imagen oficial de SQL Server 2022.
2. **Workflow `publish-images.yml`** (STAGE-12/09/10/14), dispara en tag `v*`:
   - Build de las tres imágenes con etiqueta de versión (MinVer) y SHA corto (STAGE-12).
   - SBOM CycloneDX (JSON) por imagen con un generador de SBOM de contenedor (STAGE-09).
   - Firma cosign keyless (OIDC de Actions, `id-token: write`) de cada imagen y atestación del SBOM (CycloneDX); verificación de firma y atestación en el propio pipeline (STAGE-10).
   - Publicación a GHCR (`ghcr.io/<org>/geovial-<front|backend|db>`) con `packages: write` (STAGE-14).
3. **Scripts BAT** de reproducción local (`scripts/build-images.bat`, `scripts/publish-images.bat`), homónimos de los stages, según `guia-publicacion-image-docker §2`.
4. **Documentación**: `supply-chain-seguridad` a v1.3 (§1/§2 implementados también para imágenes); control de cambios de `guia-publicacion-image-docker`.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- El workflow genera un SBOM CycloneDX (JSON) por imagen y firma cada imagen y su SBOM con cosign keyless, verificándolos antes de publicar.
- Las imágenes se publican a GHCR sólo en tag `v*`, etiquetadas con la versión MinVer y el SHA corto.
- Los Dockerfiles son multi-stage (build SDK → runtime mínimo) y no incluyen secretos.
- La suite .NET (315 pruebas) permanece verde; el gate de cobertura no se ve afectado (cambio de DevOps, sin lógica nueva).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El build de imagen no es verificable localmente (sin Docker en el entorno de CI .NET) | Alta | Bajo | El workflow corre en Actions sólo en tag `v*`, igual que `publish-sync.yml`; se revisa estructuralmente y se valida la sintaxis YAML/Dockerfile |
| Diferencia de puerto entre dev (5080/5180) y contenedor (8080) | Media | Bajo | Las imágenes exponen 8080 (convención de las imágenes ASP.NET); el deploy mapea puertos (entornos-deploy) |
| La imagen db con datos personales | Baja | Medio | La imagen db es la oficial de SQL Server; el esquema/seed se aplica por migración EF, no se hornea data; auditoría/retención por ADR-14 |

## 7. Criterios de hecho del sprint

El Sprint 28 se considera completo cuando el workflow de publicación de imágenes genera y firma el SBOM CycloneDX de las tres imágenes y firma las imágenes con cosign keyless (verificando ambos antes del push a GHCR), los Dockerfiles y scripts están en el repo, `supply-chain-seguridad` queda en v1.3 con §1/§2 implementados para imágenes, la suite .NET sigue verde, y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Política | supply-chain-seguridad §1 (SBOM), §2 (firma), §3 (SLSA L2) |
| Pipeline | pipeline-ci-cd STAGE-12 (build), STAGE-09 (SBOM), STAGE-10 (firma), STAGE-14 (publish) |
| Guía | guia-publicacion-image-docker (Dockerfiles, scripts, verificación) |
| Artefacto | tres imágenes Docker del monolito (front/backend/db) |
| Calidad | definition-of-done §1.4; retros S25/S27 (SBOM/firma de imágenes) |
| Tests previstos | sin pruebas .NET nuevas; verificación dentro del workflow (firma + atestación) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 28 (supply-chain de imágenes Docker): SBOM CycloneDX + firma cosign keyless de las tres imágenes del monolito en el workflow de publicación de imágenes, con Dockerfiles multi-stage y scripts de reproducción local. Compromete 8 SP; sin lógica de dominio nueva. Generado por AG-07 |
