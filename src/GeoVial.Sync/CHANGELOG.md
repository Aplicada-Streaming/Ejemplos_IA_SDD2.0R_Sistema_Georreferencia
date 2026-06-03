# Changelog — GeoVial.Sync

Todas las novedades relevantes de la librería de sincronización. El formato sigue
[Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/) y el versionado es
[SemVer 2.0.0](https://semver.org/lang/es/) calculado por MinVer desde los tags Git con prefijo `v`
(ver `09_devops/estrategia-versionado`).

## [No publicado]

_(sin cambios pendientes)_

## [1.0.0] — 2026-06-02

Primer release **stable** de la librería de sincronización (tag `v1.0.0`). La superficie pública
(`Abstractions`) queda congelada bajo SemVer: a partir de aquí, todo cambio incompatible bumpea MAJOR
(ADR-07, estrategia-versionado §6).

### Added
- Empaquetado NuGet de la librería (PackageId `GeoVial.Sync`) con versionado automático por MinVer y
  publicación en GitHub Packages (canales preview/stable, ADR-07).
- `GeneratePackageOnBuild` en Release: el build produce el `.nupkg`, verificado por una prueba sobre su
  contenido (DLL + README + metadatos).
- Firma del paquete con **cosign keyless** (sigstore, OIDC de GitHub Actions) en el pipeline de publicación,
  verificada antes de publicar; bundle de verificación adjunto al release (supply-chain-seguridad §2).
- SBOM **CycloneDX** (JSON) del paquete generado y firmado con cosign en el pipeline, adjunto al release
  (supply-chain-seguridad §1).
- Consumidor de prueba `samples/01-sync-basico` y demo MAUI autónoma `samples/02-sync-maui-demo`.

### Superficie pública (estable, SemVer)
- `IChangeQueue`, `ISyncEngine`, `ISyncBackendClient`, `IConflictReporter`, `IConnectivityMonitor`.
- `ChangeRecord`, `SyncResult`, `ConflictInfo`, `OperationType`, `ConflictKind`, `SyncOptions`.
- `ColaCambiosSqlite`, `MotorSincronizacion`, `ClienteSyncHttp`.
- `ColectorOffline`, `ChangeRecordFactory`, `ObservacionCapturada`, `PayloadComentario`, `CoordinadorAutoSync`.
- Excepciones: `ConsolidationException`, `ConflictNotMarkedException`, `SyncInterruptedException`,
  `AlmacenamientoLocalInsuficienteException`.

> **Publicado** al crear el tag `v1.0.0` sobre `main` (estrategia-versionado §3, §5): `publish-sync.yml`
> publicó el paquete al canal **stable** de GitHub Packages con SBOM CycloneDX + firma cosign keyless
> (ambos verificados en el pipeline). El pipeline se validó antes con tags preview `v1.0.0-rc.1`…`rc.4`,
> que expusieron y permitieron corregir cuatro bugs latentes de los workflows (pack NU5026, flag del SBOM
> CycloneDX 6.x, concurrencia de la firma de imágenes y presupuesto del step de firma) sin arriesgar el
> stable. En paralelo, `publish-images.yml` publicó las tres imágenes Docker del monolito a GHCR firmadas y
> con SBOM atestado.
