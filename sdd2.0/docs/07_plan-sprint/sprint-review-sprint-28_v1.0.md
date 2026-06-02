# Sprint Review — Sprint 28

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-28_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-28_v1.0.md`:

> Extender el endurecimiento de supply-chain (ya completo para el paquete `GeoVial.Sync` en S22/S25) a las tres imágenes Docker del monolito (front/backend/db): generar un SBOM CycloneDX por imagen (STAGE-09) y firmar cada imagen y su SBOM con cosign keyless (STAGE-10), verificándolos en el pipeline antes de publicar a GHCR (STAGE-14).

Veredicto: Cumplido.

Explicación corta: se agregaron los Dockerfiles multi-stage que la guía ya referenciaba y que no existían (`src/GeoVial.Api/Dockerfile`, `src/GeoVial.Web/Dockerfile`, `infra/db/Dockerfile`), un `.dockerignore`, y el workflow `publish-images.yml`. El workflow, sobre una matriz de las tres imágenes, construye y publica cada una a GHCR etiquetada con la versión MinVer, el SHA corto y `:latest` en stable (STAGE-12/14), genera un SBOM CycloneDX (JSON) por imagen (STAGE-09), firma la imagen por digest y atesta su SBOM con cosign keyless —verificando firma y atestación en el propio pipeline— (STAGE-10), y dispara sólo en tag `v*`. Se añadieron los scripts de reproducción local `build-images.bat`/`publish-images.bat`. Con esto, `supply-chain-seguridad §1/§2` queda implementado también para las imágenes (doc a v1.3) y el endurecimiento SBOM + firma está completo para los dos tipos de artefacto del proyecto: el paquete y las imágenes. Cierra la acción reiterada en las retros S25/S27.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| EP-09 | Supply-chain | El workflow construye y publica las 3 imágenes a GHCR en tag `v*` (STAGE-12/14) | Artefacto desplegable trazable |
| EP-09 | Supply-chain | Un SBOM CycloneDX por imagen (STAGE-09) | Inventario de dependencias por imagen |
| EP-09 | Supply-chain | Firma cosign de la imagen + atestación del SBOM, verificadas antes de publicar (STAGE-10) | Integridad e inventario verificables por terceros |

## 3. Feedback recibido

- El monolito alcanza la misma garantía de supply-chain que la librería: imágenes firmadas con SBOM atestado, registrado en Rekor.
- Tener los Dockerfiles en el repo desbloquea el build/deploy reproducible que la guía describía pero no podía ejecutarse.
- La verificación de firma y atestación corre dentro del pipeline, por lo que una imagen no se publica sin pasar el control.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 315 verdes (278 unitarias + 37 de integración), sin cambio respecto del Sprint 27: es un sprint de supply-chain/DevOps sin lógica de dominio nueva, por lo que el gate de cobertura .NET se mantiene. La verificación específica del sprint es el build + SBOM + firma + atestación dentro del workflow de publicación de imágenes, que corre en GitHub Actions al taggear `v*` (no en el gate .NET local, igual que `publish-sync.yml`); en este sprint se revisó estructuralmente contra el workflow del paquete ya en producción. Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-SUPPLY-IMG | Tarea | Aceptada (Dockerfiles + `publish-images.yml` con SBOM CycloneDX + firma/atestación cosign verificadas + scripts) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 28 se traslada. |

Siguen en el backlog: la publicación efectiva de `v1.0.0` (tag por el Release manager, que ahora dispara también el build/firma de imágenes) y el mapa interactivo (bloqueado por la clave de proveedor de mapas).

## 7. Decisiones tomadas durante el review

- Reusar el mismo modo keyless (OIDC de Actions, Rekor) del paquete para las imágenes: firma por digest + atestación CycloneDX del SBOM.
- Modelar las tres imágenes como una matriz del mismo job para no triplicar la lógica del workflow.
- Etiquetar `:latest` sólo en stable (versión sin sufijo de prerelease), coherente con la estrategia de versionado.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 28 (SBOM + firma de las imágenes Docker del monolito). Veredicto Cumplido, velocity 8, 0 carry-over, 315 pruebas verdes. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
