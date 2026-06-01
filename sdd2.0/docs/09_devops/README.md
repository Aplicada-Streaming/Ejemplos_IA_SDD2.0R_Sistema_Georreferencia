# 09 DevOps — GeoVial

**Proyecto:** GeoVial
**Documento:** README.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Ingeniero DevOps Senior (AG-09), Equipo SDD 2.0

Índice navegable de los artefactos DevOps de GeoVial (tipo D8 `web-monolith`, PROJECT-README §1). Esta sección ancla la automatización del ciclo de vida del artefacto: build, validación, empaquetado, firma, publicación y rollback. Recibe upstream de 05 (arquitectura y NFR numéricos) y de 08 (Definition of Done, estrategia y cobertura); los quality gates del pipeline ejecutan la DoD de 08, no la redefinen.

## Dos tipos de artefacto

GeoVial publica dos clases de artefacto que se documentan por separado y no se confunden (publicación de paquete vs despliegue de servicio):

- Imágenes Docker del monolito desplegable (front/backend/db) → ambientes DEV/QA/STAGING/PROD.
- Paquete de la librería de sincronización `GeoVial.Sync` en GitHub Packages → canales preview/stable (ADR-07).

## Orden de lectura sugerido

| Orden | Documento | Estado | Contenido |
| --- | --- | --- | --- |
| 1 | Acuerdo de equipo (00) y `08_calidad_y_pruebas/definition-of-done_v1.0.md` | Vigente (upstream) | Branching acordado y DoD que el pipeline ejecuta como gates |
| 2 | [estrategia-versionado_v1.0.md](./estrategia-versionado_v1.0.md) | Propuesto | SemVer 2.0.0, Conventional Commits, MinVer, GitHub Flow, canales preview/stable, deprecation policy |
| 3 | [pipeline-ci-cd_v1.0.md](./pipeline-ci-cd_v1.0.md) | Propuesto | 14 stages mapeados a los 6 de README §11, matriz SO/runtime, caché, promotion, rollback, notificaciones |
| 4 | [entornos-deploy_v1.0.md](./entornos-deploy_v1.0.md) | Propuesto | Ambientes DEV/QA/STAGING/PROD (monolito) y canales preview/stable (paquete), IaC, 12-factor, secretos, promoción |
| 5 | [guia-publicacion-image-docker_v1.0.md](./guia-publicacion-image-docker_v1.0.md) | Propuesto | Publicación de las imágenes Docker front/backend/db |
| 6 | [guia-publicacion-paquete-github-packages_v1.0.md](./guia-publicacion-paquete-github-packages_v1.0.md) | Propuesto | Publicación del paquete de la librería de sync en GitHub Packages |
| 7 | [supply-chain-seguridad_v1.0.md](./supply-chain-seguridad_v1.0.md) | Propuesto | SBOM, firma, SLSA, dependency scanning, SAST/DAST, política de CVE, compliance Ley 25.326 |

## Estado de los artefactos

| Artefacto | Obligatorio | Estado |
| --- | --- | --- |
| pipeline-ci-cd_v1.0.md | Sí | Propuesto |
| estrategia-versionado_v1.0.md | Sí | Propuesto |
| entornos-deploy_v1.0.md | Sí | Propuesto |
| guia-publicacion-image-docker_v1.0.md | Sí (artefacto desplegable) | Propuesto |
| guia-publicacion-paquete-github-packages_v1.0.md | Sí (paquete publicable, ADR-07) | Propuesto |
| supply-chain-seguridad_v1.0.md | Sí | Propuesto |
| README.md | Recomendado | Propuesto |

## Plataforma y workflows

- Plataforma de CI: GitHub Actions (PROJECT-README §11). Workflows reales: pendientes de materialización en `.github/workflows/` del repositorio; los stages y comandos están definidos en `pipeline-ci-cd_v1.0.md` y son reproducibles localmente con los scripts BAT (`build-*.bat`, `publish-*.bat`).
- Registro de imágenes: GitHub Container Registry. Feed de paquetes: GitHub Packages.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Índice inicial de la sección 09 DevOps de GeoVial con orden de lectura (acuerdo de equipo/DoD → versionado → pipeline → ambientes → guías de publicación → supply chain) y estado de los siete artefactos. Generado por AG-09 |
