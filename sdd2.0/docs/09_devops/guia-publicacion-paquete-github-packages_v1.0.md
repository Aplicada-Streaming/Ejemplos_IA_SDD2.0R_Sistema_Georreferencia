# Guía de publicación — paquete en GitHub Packages — GeoVial

**Proyecto:** GeoVial
**Documento:** guia-publicacion-paquete-github-packages_v1.0.md
**Versión:** 1.1
**Estado:** Aceptado (implementado en el Sprint 16)
**Fecha:** 2026-06-02
**Autor:** Ingeniero DevOps Senior (AG-09), Equipo SDD 2.0
**Trazabilidad upstream:** 05 (ADR-07 publicación/versionado, contratos-abstractions-sync §6); PROJECT-README §1, §10, §14 (stage 4 empaquetado); 08 (definition-of-done §1.4)
**Trazabilidad downstream:** pipeline-ci-cd_v1.0.md (STAGE-11 package, STAGE-13 publish); estrategia-versionado_v1.0.md (canales); 11_examples (samples/01-sync-basico, samples/02-sync-maui-demo)

Esta guía cubre la publicación de la librería de sincronización `GeoVial.Sync` como paquete en GitHub Packages, con canales preview (prerelease) y stable (ADR-07). Es publicación de paquete, no despliegue de servicio: el monolito de tres contenedores se publica/despliega por separado (`guia-publicacion-image-docker_v1.0.md`, `entornos-deploy_v1.0.md`).

## 1. Pre-requisitos

- Feed: GitHub Packages del repositorio (`https://nuget.pkg.github.com/<organizacion>/index.json`).
- Credencial: token con scopes mínimos `write:packages` y `read:packages`. En CI lo provee el token de GitHub Actions con permiso `packages: write` (OIDC); en publicación manual, un Personal Access Token con esos scopes. El token vive en GitHub Secrets con rotación a 90 días (PROJECT-README §8); prohibido el commit.
- Herramientas locales (publicación manual reproducible): .NET 9 SDK, `dotnet` con la fuente de GitHub Packages configurada (`dotnet nuget add source`).
- Superficie pública estable: la capa Abstractions de `GeoVial.Sync` documentada en `contratos-abstractions-sync_v1.0.md`. Todo breaking change de esa superficie bumpea MAJOR (estrategia-versionado §1, §6).
- Versión calculada por MinVer desde el tag Git; el sufijo del tag determina el canal (estrategia-versionado §5).

## 2. Comando o stage de publicación

Stage automatizado: STAGE-11 (package) y STAGE-13 (publish) del pipeline (pipeline-ci-cd §1), invocables localmente por los scripts BAT homónimos.

```bash
# Empaquetado local reproducible (equivalente a STAGE-11)
scripts/publish-pack-sync.bat
# Internamente:
#   dotnet pack src/GeoVial.Sync -c Release -o ./artifacts
#   (MinVer calcula la versión; el sufijo -alpha/-beta/-rc del tag => canal preview)

# Publicación (equivalente a STAGE-13), sólo en tag
dotnet nuget add source https://nuget.pkg.github.com/<organizacion>/index.json \
  --name github --username <usuario> --password $GH_PACKAGES_TOKEN --store-password-in-clear-text
scripts/publish-sync.bat          # dotnet nuget push ./artifacts/GeoVial.Sync.<version>.nupkg --source github
```

Variables de entorno requeridas: `GH_PACKAGES_TOKEN` (token con `write:packages`, desde GitHub Secrets), `PACKAGE_VERSION` (versión SemVer de MinVer). Reglas de canal (pipeline-ci-cd §4.2): tag con sufijo `-alpha.N/-beta.N/-rc.N` publica al canal preview de forma automática; tag `v<X.Y.Z>` sin sufijo publica al canal stable con aprobación del Release manager.

## 3. Verificación post-publish (instalación de prueba)

```bash
# 1. Confirmar que el paquete aparece en el feed con la versión y canal esperados
dotnet nuget list source

# 2. Instalación de prueba en un consumidor limpio (sample 01-sync-basico)
cd samples/01-sync-basico
dotnet add package GeoVial.Sync --version <version> --source github
dotnet build && dotnet run        # debe compilar y sincronizar contra el backend mock

# 3. Para el canal stable, ejecutar la demo MAUI autónoma (sample 02-sync-maui-demo)
#    alta local -> sync contra mock server -> estado de la cola -> resolución básica de conflictos (ADR-07 §8)

# 4. Verificar la firma del paquete (supply-chain-seguridad §2) antes de declararlo consumible
```

La instalación de prueba contra `samples/01-sync-basico` y la demo `samples/02-sync-maui-demo` (PROJECT-README §14, 11_examples) son la verificación de que la API pública es consumible. Para stable, además, los contract tests (TC-26) deben estar verdes (definition-of-done §1.4).

## 4. Rollback

Mecanismo: delist/deprecate en GitHub Packages + PATCH con el fix; no se rompe a los consumidores ya instalados (pipeline-ci-cd §5.2).

```bash
# 1. Marcar la versión rota como deprecada o no listada en GitHub Packages
scripts/publish-sync-deprecate.bat <version-rota> "motivo; usar <version-recomendada>"
# 2. Publicar PATCH con el fix por el flujo normal (tag v<X.Y.Z+1>)
# 3. Si el defecto es breaking, MAJOR + guía de migración en el CHANGELOG (estrategia-versionado §6)
```

Ventana de gracia y comunicación: la versión delistada queda no listada en el feed, pero un consumidor con la referencia fija puede seguir restaurándola; el retiro se comunica en el CHANGELOG (Keep a Changelog) y en las release notes, indicando la versión recomendada. A diferencia del rollback de imagen (reversión de deploy), aquí no se "revierte" una versión publicada: se publica una nueva y se desaconseja la anterior.

## 5. Métricas

| Métrica | Fuente | Objetivo |
| --- | --- | --- |
| Descargas del paquete por versión y canal | estadísticas de GitHub Packages | trazabilidad de adopción |
| Tasa de adopción de stable vs preview | proporción de descargas por canal | migración sana de preview a stable |
| Vulnerabilidades detectadas post-publish | escaneo de dependencias del paquete (supply-chain-seguridad §4) | 0 críticas; SLA por severidad (supply-chain-seguridad §6) |
| Tiempo medio hasta detección de regresión | feedback de consumidores + contract tests de CI | minimizar; PATCH/MAJOR rápido |
| Breaking changes señalados por MAJOR | comparación de superficie Abstractions entre versiones | 100% de los breaking change bumpean MAJOR (ADR-07 §8) |

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Guía de publicación inicial del paquete de la librería de sincronización en GitHub Packages: pre-requisitos (feed, token con scopes mínimos `write:packages`/`read:packages`, rotación 90 días), comando/stage (STAGE-11/STAGE-13, scripts de README §11), verificación por instalación de prueba contra samples 01 y 02, rollback por delist/deprecate + PATCH/MAJOR y métricas de adopción. Canales preview/stable (ADR-07). Generada por AG-09 |
| 1.1 | 2026-06-02 | Implementada en el Sprint 16: `GeoVial.Sync.csproj` con metadatos NuGet + MinVer (prefijo `v`); scripts `scripts/publish-pack-sync.bat`, `scripts/publish-sync.bat`, `scripts/publish-sync-deprecate.bat`; workflow `.github/workflows/publish-sync.yml` (STAGE-11/13, dispara en tag `v*` con el token de Actions `packages: write`); consumidor de prueba `samples/01-sync-basico`; CHANGELOG inicial. Verificado: `dotnet pack` produce el `.nupkg` con la DLL + README (versión MinVer preview). Estado a Aceptado. Por AG-09 |
