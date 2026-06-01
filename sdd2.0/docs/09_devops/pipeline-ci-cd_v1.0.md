# Pipeline CI/CD — GeoVial

**Proyecto:** GeoVial
**Documento:** pipeline-ci-cd_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Ingeniero DevOps Senior (AG-09), Equipo SDD 2.0
**Trazabilidad upstream:** 05 (arquitectura-solucion §5 despliegue, §8 NFR numéricos, ADR-07); 08 (definition-of-done, estrategia-testing, criterios-validacion); PROJECT-README §9, §10, §11, §12
**Trazabilidad downstream:** 10_developer_guide (cita los comandos del pipeline para reproducción local); entornos-deploy_v1.0.md, guia-publicacion-image-docker_v1.0.md, guia-publicacion-paquete-github-packages_v1.0.md, supply-chain-seguridad_v1.0.md de esta misma sección

## 0. Alcance

GeoVial es de tipo D8 `web-monolith` (PROJECT-README §1). El pipeline construye, valida, empaqueta, firma y publica dos clases de artefacto distintas que no deben confundirse:

- Imágenes Docker del monolito desplegable: front (`GeoVial.Web`), backend (`GeoVial.Api` + capas + `GeoVial.FileHosting`) y base de datos. Se despliegan a ambientes DEV/QA/STAGING/PROD (ver `entornos-deploy_v1.0.md`).
- Paquete de la librería de sincronización `GeoVial.Sync`, publicado en GitHub Packages con canales preview (prerelease) y stable (ADR-07). Es publicación de paquete, no despliegue de servicio.

La distinción entre publicar un paquete y desplegar un servicio se respeta a lo largo de todo el documento (anti-patrón §4.8 de la regla 09). La plataforma de CI es GitHub Actions (PROJECT-README §11). El pipeline ejecuta como gates la Definition of Done de 08; no la redefine.

## 1. Stages obligatorios

Los seis stages funcionales de PROJECT-README §11 (Restore, Build, Tests, Empaquetado de la librería, Build de imágenes, Publish) se materializan en la siguiente cadena de stages técnicos. Cada stage declara su comando, su tooling, su criterio de éxito (quality gate) y el criterio DoD de 08 o el NFR de 05 que verifica.

| STAGE | Stage | Tooling | Comando representativo | Quality gate | Verifica (DoD 08 / NFR 05) | Bloqueante | Mapeo README §11 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| STAGE-01 | Lint / formato | `dotnet format`; analizadores Roslyn | `dotnet format --verify-no-changes`; `scripts/build-lint.bat` | 0 violaciones de formato; 0 warnings nuevos de análisis estático respecto de la línea base | DoD US/BT "análisis estático sin warnings nuevos" (criterios-validacion §5) | Sí en PR | (previo a stage 2) |
| STAGE-02 | Restore | `dotnet restore`; caché NuGet | `dotnet restore GeoVial.sln`; `scripts/build-restore.bat` | Restore reproducible; lock file respetado; licencias MIT/Apache/BSD, sin GPL en componentes distribuidos | DoD release "dependencias cumplen la política de licencias" (criterios-validacion §5) | Sí | Stage 1 (Restore) |
| STAGE-03 | Build | `dotnet build` con `TreatWarningsAsErrors=true` | `dotnet build -c Release -warnaserror`; `scripts/build-all.bat` | 0 errores de compilación; compila sin warnings tratados como error | DoD US/BT "compila sin warnings tratados como error" (definition-of-done §1.1/§1.2; README §11 stage 2) | Sí | Stage 2 (Build, gate) |
| STAGE-04 | Test unit | xUnit + FluentAssertions; Coverlet | `dotnet test tests/GeoVial.UnitTests` | 100% verde; cobertura dominio ≥ 85/75, aplicación ≥ 80/70 (pisos por capa de estrategia-testing §2) | DoD "tests unitarios pasan al 100%"; pirámide 70% (estrategia-testing §1) | Sí | Stage 3 (Tests, gate) |
| STAGE-05 | Test integración | WebApplicationFactory + Testcontainers (SQL Server); Coverlet | `dotnet test tests/GeoVial.IntegrationTests` | Verde; incluye TC-24 (latencia p95 ≤ 500 ms), TC-10 (sync ≤ 5 min y payload de foto), TC-25 (OpenAPI coincide) | DoD "OpenAPI regenerado coincide"; NFR latencia y sincronización de 05 §8 | Sí | Stage 3 (Tests, gate) |
| STAGE-06 | Test componente | bUnit; Coverlet | `dotnet test tests/GeoVial.ComponentTests` | Verde; presentación ≥ 60/50 | DoD US (criterios Given/When/Then verdes); pirámide 10% | Sí | Stage 3 (Tests, gate) |
| STAGE-07 | Gate de cobertura | Coverlet + reporte agregado | `scripts/build-coverage.bat` (reportgenerator) | Líneas ≥ 80%, branches ≥ 70% sobre dominio+aplicación+infraestructura; pisos por capa de estrategia-testing §2 no bajan | DoD sprint/release "gate de cobertura" (definition-of-done §1.3/§1.4; README §9/§11) | Sí | Stage 3 (Tests, gate) |
| STAGE-08 | SCA | escáner de composición de dependencias (Dependabot/escáner de SCA del pipeline) | `scripts/build-sca.bat` | 0 CVE críticas; 0 altas sin excepción registrada por ADR; licencias conformes | DoD release "dependencias cumplen política"; supply-chain-seguridad §4 | Sí | (refuerza Stage 3) |
| STAGE-09 | SBOM | generador CycloneDX para .NET y para imágenes | `scripts/build-sbom.bat` | SBOM CycloneDX JSON generado por artefacto y adjunto al release | DoD release (artefactos del release completos); supply-chain-seguridad §1 | Sí | (refuerza Stage 6) |
| STAGE-10 | Firma | cosign (sigstore) keyless con OIDC de GitHub Actions | `scripts/sign-artifacts.bat` | Firma válida registrada en transparency log; firma del propio SBOM | supply-chain-seguridad §2 | Sí | (refuerza Stage 6) |
| STAGE-11 | Package (librería de sync) | `dotnet pack`; MinVer | `dotnet pack src/GeoVial.Sync -c Release`; `scripts/publish-pack-sync.bat` | `.nupkg` generado con versión SemVer derivada de tags; canal según sufijo del tag | DoD release "empaquetado de la librería de sincronización verde" (definition-of-done §1.4; README §11 stage 4) | Sí | Stage 4 (Empaquetado librería) |
| STAGE-12 | Build de imágenes Docker | `docker build` (multi-stage); Buildx | `scripts/build-images.bat` (front, backend, db) | Las tres imágenes (front, backend, db) construyen; etiquetadas con la versión SemVer y el SHA | DoD release "build de imágenes Docker verde" (definition-of-done §1.4; README §11 stage 5) | Sí | Stage 5 (Build de imágenes) |
| STAGE-13 | Publish paquete | `dotnet nuget push` a GitHub Packages | `scripts/publish-sync.bat` | Paquete disponible en el canal (preview o stable); ver `guia-publicacion-paquete-github-packages_v1.0.md` | DoD release | Solo en tag | Stage 6 (Publish) |
| STAGE-14 | Publish imágenes | `docker push` a GitHub Container Registry | `scripts/publish-images.bat` | Las tres imágenes disponibles en el registry con su tag de versión; ver `guia-publicacion-image-docker_v1.0.md` | DoD release | Solo en tag/promoción | Stage 6 (Publish) |

Notas:

- La suite móvil de UI (`tests/GeoVial.UiTests`, .NET MAUI UI testing / Appium sobre Android) no corre en el pipeline hosteado de GitHub Actions en v1: requiere dispositivo Android físico por USB en modo desarrollador (PROJECT-README §11, §16; estrategia-testing §7). Se ejecuta de forma manual asistida en máquina del desarrollador antes del release y su resultado (TC-09 operación offline, TC-23 detección de conectividad) se adjunta como evidencia de la DoD de release. Esta excepción está prevista por la DoD (definition-of-done §2) y por criterios-validacion §6, y queda registrada como tal.
- Cada stage es reproducible localmente mediante los scripts BAT homónimos (`build-*.bat`, `publish-*.bat`, PROJECT-README §11), evitando el anti-patrón "pipeline irreproducible localmente" (§4.8). La developer guide de 10 cita estos comandos.

## 2. Matriz de SO y runtime

| Trigger | Sistemas operativos | Runtime | Justificación |
| --- | --- | --- | --- |
| PR a `main` | `ubuntu-latest`, `windows-latest` | .NET 9 (LTS vigente declarada en PROJECT-README §2) | El backend y el front se despliegan en contenedores Linux (arquitectura-solucion §5), pero el desarrollo es íntegramente local en Windows (PROJECT-README §16). La matriz cruzada en PR detecta diferencias de separador de path, line endings y mayúsculas de filesystem antes del merge. |
| Push a `main` | `ubuntu-latest` | .NET 9 | Build de validación de integración continua sobre la rama protegida. |
| Tag `v<X.Y.Z>` o `v<X.Y.Z>-<sufijo>` | `ubuntu-latest` | .NET 9 | Las imágenes objetivo son Linux (contenedores front/backend/db, arquitectura-solucion §5); el empaquetado y publicación se hacen sobre Linux para coincidir con el runtime de despliegue. |
| Integración con SQL Server | `ubuntu-latest` (Testcontainers levanta SQL Server 2022) | imagen oficial de SQL Server | El contenedor de base de datos del despliegue es SQL Server sobre Linux (arquitectura-solucion §5; modelo-datos-logico). |

UI móvil (Android, API 26+) fuera de la matriz CI hosteada: se valida en el equipo del desarrollador sobre dispositivo por USB (PROJECT-README §12, §16). iOS y tablets están fuera de alcance en v1 (ADR-13). Justificación de la matriz: cubre los consumidores reales (Windows en desarrollo, Linux en runtime de contenedor) sin inflar minutos de CI con combinaciones que el proyecto no despliega.

## 3. Caché y artefactos

Caché:

| Caché | Llave | Contenido | Expiración |
| --- | --- | --- | --- |
| NuGet | hash de `**/packages.lock.json` + SO | paquetes restaurados (`~/.nuget/packages`) | invalidada al cambiar cualquier lock file |
| Capas Docker | hash de `Dockerfile` + contexto de build (Buildx layer cache) | capas intermedias de las imágenes front/backend/db | reuso entre builds; invalidada por cambio de capa |
| Reportes de análisis | por SHA | línea base de análisis estático para comparar warnings nuevos | retención 30 días |

Artefactos producidos y retención:

| Artefacto | Stage productor | Retención |
| --- | --- | --- |
| Reporte de cobertura (Coverlet + HTML) | STAGE-07 | 90 días |
| `.nupkg` de `GeoVial.Sync` | STAGE-11 | hasta publicación; copia adjunta al release |
| Imágenes Docker front/backend/db | STAGE-12 | hasta push al registry; tag persistente en el registry |
| SBOM CycloneDX (JSON) por artefacto | STAGE-09 | adjunto permanente al release de GitHub |
| Firmas cosign y bundle de transparencia | STAGE-10 | adjunto permanente al release; registradas en transparency log |
| OpenAPI 3.x versionado generado | STAGE-05 | adjunto al release; versionado en el repo (PROJECT-README §6) |
| Evidencia de UI móvil (TC-09, TC-23) | manual pre-release | adjunta al release |

## 4. Promotion rules

GeoVial tiene dos planos de promoción independientes: ambientes de despliegue del monolito y canales del paquete de la librería. No se mezclan.

### 4.1 Promoción de ambientes (imágenes Docker del monolito)

| Transición | Trigger | Prerequisitos | Aprobador |
| --- | --- | --- | --- |
| build → DEV | merge a `main` (GitHub Flow, PROJECT-README §10) | pipeline verde hasta STAGE-14 build de imágenes; SBOM y firma presentes | Automático |
| DEV → QA | tag `v<X.Y.Z>-rc.N` | DoD de sprint verde (definition-of-done §1.3); cobertura ≥ 80/70 | QA lead |
| QA → STAGING | aprobación manual tras verde en QA | NFR de integración verdes (TC-24 latencia p95 ≤ 500 ms, TC-10 sync ≤ 5 min); ventana de soak | Release manager |
| STAGING → PROD | tag `v<X.Y.Z>` sin sufijo + aprobación | DoD de release completa (definition-of-done §1.4); criterios-validacion §2–§5; evidencia UI móvil (TC-09, TC-23) | Release manager + aprobación de negocio (jefe general) |

Cada NFR numérico de 05 §8 tiene un gate antes de promover a PROD: latencia API (TC-24) y tiempo de sincronización (TC-10) son gates de integración bloqueantes en STAGE-05; operación offline (TC-09) y detección de conectividad (TC-23) son evidencia de UI móvil exigida antes de PROD; disponibilidad (SLO 99%) y confiabilidad de georreferenciación (≥ 95%) se observan como métricas estadísticas en operación (criterios-validacion §3 y §6), no como test bloqueante de la suite, conforme a la excepción documentada de la DoD.

### 4.2 Promoción de canales (paquete de la librería de sync)

| Transición | Trigger | Prerequisitos | Aprobador |
| --- | --- | --- | --- |
| build → preview | tag con sufijo `-alpha.N`, `-beta.N` o `-rc.N` | pipeline verde hasta STAGE-13; SBOM + firma del paquete | Automático |
| preview → stable | tag `v<X.Y.Z>` sin sufijo | API pública de Abstractions estable; sin breaking change no señalado por MAJOR (contratos-abstractions-sync §6); contract tests TC-26 verdes | Release manager |

Detalle operativo en `entornos-deploy_v1.0.md` (canales) y `guia-publicacion-paquete-github-packages_v1.0.md`.

## 5. Rollback

Procedimiento por tipo de artefacto, ejecutable en minutos. Cada tipo tiene su propio mecanismo.

### 5.1 Rollback de despliegue (imágenes Docker del monolito)

Mecanismo: reversión de deploy a la imagen previa (web-monolith, §1.2 de la regla: "rollback por reversión de deploy").

```bash
# 1. Identificar el tag de imagen estable anterior
docker pull ghcr.io/<organizacion>/geovial-backend:<version-anterior>
# 2. Re-desplegar las tres imágenes de la versión previa en el ambiente afectado
scripts/deploy-rollback.bat <ambiente> <version-anterior>   # re-aplica front/backend/db de la versión anterior
# 3. Verificar health check del contenedor backend y latencia p95
# 4. Registrar el rollback en auditoría (Ley 25.326, retención >= 1 año)
```

Nota de datos: el contenedor de base de datos no se "revierte" por reimagen si hubo migración de esquema aplicada; las migraciones EF Core deben ser compatibles hacia atrás o acompañarse de un script de reversión de migración documentado. El volumen de datos persiste entre versiones de imagen.

### 5.2 Rollback de publicación de paquete (librería de sync)

Mecanismo: delist/deprecate en GitHub Packages + PATCH con el fix (no se rompe a los consumidores ya instalados).

```bash
# 1. Marcar la versión rota como deprecada/delistada en GitHub Packages
scripts/publish-sync-deprecate.bat <version-rota> "motivo y versión recomendada"
# 2. Publicar PATCH con el fix por el flujo normal (tag v<X.Y.Z+1>)
# 3. Si el defecto es breaking, MAJOR + guía de migración en el CHANGELOG
```

Ventana de gracia: la versión delistada queda no listada pero los consumidores con la referencia fija siguen pudiendo restaurarla; se comunica en el CHANGELOG y en las release notes. Detalle en `guia-publicacion-paquete-github-packages_v1.0.md`.

### 5.3 Rollback de versión / tag

```bash
# Revertir un tag publicado por error antes de la promoción
git push --delete origin v<X.Y.Z>
```

La reversión de tag solo es válida antes de que la promoción al canal/ambiente se haya consumado; una vez publicado, se usa 5.1 o 5.2.

## 6. Notificaciones

| Evento | Canal | Severidad | Escalamiento |
| --- | --- | --- | --- |
| Pipeline verde en `main` | canal de equipo (chat/Teams del organismo) | informativo | — |
| Falla de gate en PR (lint/build/test/cobertura/SCA) | comentario en el PR + canal de equipo | media | autor del PR |
| Falla en STAGE-08 SCA con CVE crítica | canal de equipo + mención a DevOps | alta | DevOps + Arquitecto (AG-05); ver SLA en supply-chain-seguridad §6 |
| Falla en publicación (STAGE-13/14) | canal de equipo + Release manager | alta | Release manager |
| Promoción a PROD ejecutada | canal de equipo + registro de auditoría | informativo con traza auditable | Release manager (firma la aprobación) |
| Rollback ejecutado | canal de equipo + auditoría | alta | Release manager + DevOps |

Dashboards visibles al equipo: estado del último run por rama (badge en el README del repo), tendencia de cobertura (reporte de Coverlet retenido 90 días) y panel de vulnerabilidades abiertas del escáner de SCA/Dependabot. El registro de promociones y rollbacks a PROD se conserva con retención ≥ 1 año por compliance Ley 25.326 (ADR-14).

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Pipeline CI/CD inicial de GeoVial sobre GitHub Actions: 14 stages técnicos mapeados a los 6 stages de README §11, matriz SO/runtime, caché, artefactos, promotion rules separadas por ambientes (monolito) y canales (paquete sync), rollback por tipo de artefacto y notificaciones. Cada gate referencia la DoD de 08 o el NFR de 05. Generado por AG-09 |
