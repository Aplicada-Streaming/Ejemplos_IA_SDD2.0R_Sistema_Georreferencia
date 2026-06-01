# Guía de publicación — imágenes Docker — GeoVial

**Proyecto:** GeoVial
**Documento:** guia-publicacion-image-docker_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Ingeniero DevOps Senior (AG-09), Equipo SDD 2.0
**Trazabilidad upstream:** 05 (arquitectura-solucion §5 tres contenedores, §8 disponibilidad); PROJECT-README §11 (stage 5 build de imágenes, stage 6 publish); 08 (definition-of-done §1.4)
**Trazabilidad downstream:** pipeline-ci-cd_v1.0.md (STAGE-12 build, STAGE-14 publish); entornos-deploy_v1.0.md (ambientes)

Esta guía cubre la publicación de las tres imágenes Docker del monolito desplegable: front (`GeoVial.Web`), backend (`GeoVial.Api` + `GeoVial.Application` + `GeoVial.Infrastructure` + `GeoVial.FileHosting`) y base de datos (SQL Server). Es publicación del artefacto desplegable del monolito; el paquete de la librería de sync se publica por separado (`guia-publicacion-paquete-github-packages_v1.0.md`).

## 1. Pre-requisitos

- Registro de imágenes: GitHub Container Registry (`ghcr.io/<organizacion>/geovial-<front|backend|db>`), coherente con el repo en GitHub (PROJECT-README §11).
- Credencial: token con scope de escritura del registry (`write:packages`). En CI lo provee el token de GitHub Actions (OIDC) con permiso `packages: write`; en publicación manual, un Personal Access Token con `write:packages` cargado por `docker login ghcr.io`. El token vive en GitHub Secrets con rotación a 90 días (PROJECT-README §8); nunca en commit.
- Herramientas locales (para build/publicación manual reproducible): Docker con Buildx, .NET 9 SDK.
- Dockerfiles multi-stage en el repo: uno por imagen (front, backend, db). La imagen db parte de la imagen oficial de SQL Server 2022.
- Versión calculada por MinVer desde el tag (estrategia-versionado §3).

## 2. Comando o stage de publicación

Stage automatizado: STAGE-12 (build) y STAGE-14 (publish) del pipeline (pipeline-ci-cd §1), invocables localmente por los scripts BAT homónimos (PROJECT-README §11).

```bash
# Build local reproducible de las tres imágenes (equivalente a STAGE-12)
scripts/build-images.bat          # docker build de front, backend y db
# Internamente, por imagen:
#   docker build -f src/GeoVial.Web/Dockerfile         -t ghcr.io/<organizacion>/geovial-front:<version> .
#   docker build -f src/GeoVial.Api/Dockerfile         -t ghcr.io/<organizacion>/geovial-backend:<version> .
#   docker build -f infra/db/Dockerfile                -t ghcr.io/<organizacion>/geovial-db:<version> .

# Publicación (equivalente a STAGE-14), sólo en tag/promoción
echo $GHCR_TOKEN | docker login ghcr.io -u <usuario> --password-stdin
scripts/publish-images.bat        # docker push de las tres imágenes con tag <version> y, en stable, también :latest
```

Variables de entorno requeridas: `GHCR_TOKEN` (token de escritura del registry, desde GitHub Secrets), `IMAGE_VERSION` (la versión SemVer calculada por MinVer), `IMAGE_REGISTRY` (`ghcr.io/<organizacion>`). Cada imagen se etiqueta con la versión SemVer y con el SHA corto del commit para trazabilidad.

## 3. Verificación post-publish

```bash
# 1. Confirmar que las tres imágenes están en el registry con la versión esperada
docker pull ghcr.io/<organizacion>/geovial-backend:<version>
docker pull ghcr.io/<organizacion>/geovial-front:<version>
docker pull ghcr.io/<organizacion>/geovial-db:<version>

# 2. Verificar la firma cosign (supply-chain-seguridad §2)
cosign verify ghcr.io/<organizacion>/geovial-backend:<version> \
  --certificate-identity-regexp '.*' --certificate-oidc-issuer https://token.actions.githubusercontent.com

# 3. Levantar el stack en un ambiente y comprobar el health check del backend
scripts/deploy.bat <ambiente> <version>
# health check del contenedor backend en verde => disponibilidad medible (NFR SLO 99%, arquitectura-solucion §8)

# 4. Verificar el SBOM adjunto de cada imagen (CycloneDX JSON, supply-chain-seguridad §1)
```

La verificación de la firma y del SBOM debe pasar antes de habilitar la promoción al siguiente ambiente. El SBOM y la firma se generan automáticamente en el pipeline (STAGE-09, STAGE-10).

## 4. Rollback

Mecanismo: reversión de deploy a la imagen previa (web-monolith; pipeline-ci-cd §5.1).

```bash
# 1. Re-desplegar las tres imágenes de la versión estable anterior en el ambiente afectado
scripts/deploy-rollback.bat <ambiente> <version-anterior>
# 2. Verificar health check del backend y latencia p95 (NFR p95 <= 500 ms)
# 3. Registrar el rollback en auditoría (Ley 25.326, retención >= 1 año, ADR-14)
```

Ventana y comunicación: el rollback es ejecutable en minutos; se comunica en el canal de equipo y se registra en auditoría (pipeline-ci-cd §6). Consideración de datos: si la versión incluía una migración de esquema EF Core, la imagen db no se "revierte" por reimagen; la migración debe ser compatible hacia atrás o disponer de un script de reversión documentado, ya que el volumen de datos persiste entre versiones.

## 5. Métricas

| Métrica | Fuente | Objetivo |
| --- | --- | --- |
| Disponibilidad del backend tras publicación | health check del contenedor backend en horario laboral | SLO 99% (NFR arquitectura-solucion §8) |
| Latencia API tras publicación | medición p95 sobre endpoints de revisión | p95 ≤ 500 ms (NFR, TC-24) |
| Tiempo medio hasta detección de regresión | comparación de reportes de CI y monitoreo post-deploy | minimizar; rollback en minutos |
| Vulnerabilidades detectadas post-publish | escaneo continuo de las imágenes publicadas (supply-chain-seguridad §4) | 0 críticas; SLA por severidad (supply-chain-seguridad §6) |
| Tasa de éxito de promoción a PROD | registro auditable de promociones | tendencia al alza, sin rollbacks no planificados |

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Guía de publicación inicial de las imágenes Docker (front/backend/db) de GeoVial: pre-requisitos (GHCR, token con scope mínimo, rotación 90 días), comando/stage (scripts de imágenes de README §11, STAGE-12/STAGE-14), verificación post-publish (pull, firma cosign, health check, SBOM), rollback por reversión de deploy y métricas ligadas a los NFR. Generada por AG-09 |
