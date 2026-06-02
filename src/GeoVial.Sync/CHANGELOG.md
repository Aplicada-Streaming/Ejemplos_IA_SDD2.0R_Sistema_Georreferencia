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

> Materializado al crear el tag `v1.0.0` sobre `main` (estrategia-versionado §3, §5): el push del tag dispara
> `publish-sync.yml` (paquete a GitHub Packages, canal **stable**, con SBOM CycloneDX + firma cosign keyless)
> y `publish-images.yml` (las tres imágenes Docker a GHCR, con SBOM + firma). En la primera ejecución del
> tag, `publish-sync.yml` falló con NU5026 (interacción de `GeneratePackageOnBuild` con `dotnet pack` en
> checkout limpio); se corrigió el step de empaquetado (`-p:GeneratePackageOnBuild=false`) y la publicación
> stable se completa re-disparando el tag sobre `main` con el fix.
