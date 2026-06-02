# Changelog — GeoVial.Sync

Todas las novedades relevantes de la librería de sincronización. El formato sigue
[Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/) y el versionado es
[SemVer 2.0.0](https://semver.org/lang/es/) calculado por MinVer desde los tags Git con prefijo `v`
(ver `09_devops/estrategia-versionado`).

## [No publicado]

_(sin cambios pendientes)_

## [1.0.0] — pendiente de tag `v1.0.0`

Primer release **stable** de la librería de sincronización. La superficie pública (`Abstractions`) queda
congelada bajo SemVer: a partir de aquí, todo cambio incompatible bumpea MAJOR (ADR-07, estrategia-versionado §6).

### Added
- Empaquetado NuGet de la librería (PackageId `GeoVial.Sync`) con versionado automático por MinVer y
  publicación en GitHub Packages (canales preview/stable, ADR-07).
- `GeneratePackageOnBuild` en Release: el build produce el `.nupkg`, verificado por una prueba sobre su
  contenido (DLL + README + metadatos).
- Firma del paquete con **cosign keyless** (sigstore, OIDC de GitHub Actions) en el pipeline de publicación,
  verificada antes de publicar; bundle de verificación adjunto al release (supply-chain-seguridad §2).
- Consumidor de prueba `samples/01-sync-basico` y demo MAUI autónoma `samples/02-sync-maui-demo`.

### Superficie pública (estable, SemVer)
- `IChangeQueue`, `ISyncEngine`, `ISyncBackendClient`, `IConflictReporter`, `IConnectivityMonitor`.
- `ChangeRecord`, `SyncResult`, `ConflictInfo`, `OperationType`, `ConflictKind`, `SyncOptions`.
- `ColaCambiosSqlite`, `MotorSincronizacion`, `ClienteSyncHttp`.
- `ColectorOffline`, `ChangeRecordFactory`, `ObservacionCapturada`, `PayloadComentario`, `CoordinadorAutoSync`.
- Excepciones: `ConsolidationException`, `ConflictNotMarkedException`, `SyncInterruptedException`,
  `AlmacenamientoLocalInsuficienteException`.

> El primer release **stable** (`v1.0.0`) se materializa al crear el tag `v1.0.0` sobre `main`
> (estrategia-versionado §3, §5). Hasta entonces, los artefactos son del canal **preview** (prerelease).
