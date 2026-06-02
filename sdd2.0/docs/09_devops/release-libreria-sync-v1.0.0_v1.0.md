# Release de la librería — GeoVial.Sync v1.0.0

**Proyecto:** GeoVial
**Documento:** release-libreria-sync-v1.0.0_v1.0.md
**Versión:** 1.0
**Estado:** Preparado (pendiente de tag por el Release manager)
**Fecha:** 2026-06-02
**Autor:** Ingeniero DevOps Senior (AG-09), Equipo SDD 2.0
**Trazabilidad:** ADR-07 (publicación/versionado); estrategia-versionado §3/§5; guia-publicacion-paquete-github-packages; definition-of-done §1.4

## 1. Notas de release — v1.0.0 (primer stable)

Primer release **stable** de la librería de sincronización `GeoVial.Sync`. Materializa EP-09: una librería
reutilizable de continuidad operativa (cola local, motor de sincronización last-write-wins e idempotencia,
reporte de conflictos) con una superficie pública estable versionada por SemVer.

A partir de `v1.0.0` la superficie pública (`Abstractions`) queda **congelada**: todo cambio incompatible
bumpea MAJOR (ADR-07, estrategia-versionado §6).

### Superficie pública (estable)

- Interfaces: `IChangeQueue`, `ISyncEngine`, `ISyncBackendClient`, `IConflictReporter`, `IConnectivityMonitor`.
- Tipos: `ChangeRecord`, `SyncResult`, `ConflictInfo`, `OperationType`, `ConflictKind`, `SyncOptions`.
- Implementaciones: `ColaCambiosSqlite`, `MotorSincronizacion`, `ClienteSyncHttp`.
- Captura/auto-sync: `ColectorOffline`, `ChangeRecordFactory`, `ObservacionCapturada`, `PayloadComentario`, `CoordinadorAutoSync`.
- Excepciones: `ConsolidationException`, `ConflictNotMarkedException`, `SyncInterruptedException`, `AlmacenamientoLocalInsuficienteException`.

### Verificación previa al release

- El build en Release produce el `.nupkg` (`GeneratePackageOnBuild`); una prueba del gate verifica su contenido
  (DLL `lib/net10.0/GeoVial.Sync.dll`, `README.md`, nuspec con `id=GeoVial.Sync` y licencia `MIT`).
- 303+ pruebas verdes; cobertura de dominio/aplicación sobre el gate.

### Firma del artefacto (supply-chain)

El paquete se firma con **cosign (sigstore) en modo keyless** usando el OIDC de GitHub Actions (sin llaves
privadas de larga vida, supply-chain-seguridad §2). La firma es un bundle detached (`<paquete>.cosign.bundle`)
con certificado efímero y entrada en el transparency log de sigstore (Rekor), adjuntado al release como
artefacto. El propio workflow `publish-sync.yml` firma y **verifica** la firma antes de publicar (STAGE-10 → STAGE-13).

Un consumidor verifica la integridad y la procedencia con:

```bash
cosign verify-blob GeoVial.Sync.1.0.0.nupkg \
  --bundle GeoVial.Sync.1.0.0.nupkg.cosign.bundle \
  --certificate-oidc-issuer https://token.actions.githubusercontent.com \
  --certificate-identity-regexp "^https://github.com/Aplicada-Streaming/Ejemplos_IA_SDD2.0R_Sistema_Georreferencia/.+"
```

## 2. Checklist de release (Release manager)

1. **Pre-condiciones**: `main` verde en CI (build + tests + cobertura); CHANGELOG con la sección `[1.0.0]`.
2. **Tag**: crear el tag de versión sobre `main` y empujarlo:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```
   MinVer toma la versión del tag (`MinVerTagPrefix=v`) → el paquete sale como `1.0.0` (canal **stable**).
3. **Publicación automática**: el push del tag `v*` dispara el workflow `.github/workflows/publish-sync.yml`
   (STAGE-11 pack + STAGE-13 push a GitHub Packages con el token de Actions, `packages: write`).
4. **Verificación post-publish** (guia-publicacion-paquete-github-packages §3):
   - Confirmar que `GeoVial.Sync 1.0.0` aparece en el feed de GitHub Packages.
   - Instalación de prueba en un consumidor limpio (`samples/01-sync-basico`): `dotnet add package GeoVial.Sync --version 1.0.0 --source github` → compila y sincroniza contra el backend en memoria.
5. **Rollback** (si hace falta): delist/deprecate la versión y publicar un PATCH (`v1.0.1`) con el fix
   (guia-publicacion-paquete-github-packages §4); nunca se reescribe una versión publicada.

> Este sprint deja todo preparado y verificado; la creación del tag `v1.0.0` y la aprobación del release
> son decisión del Release manager y quedan fuera del alcance automatizado.

## 3. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Notas de release y checklist del primer stable v1.0.0 de GeoVial.Sync (Sprint 21). Empaquetado en build verificado por prueba del contenido del `.nupkg`; publicación por tag con aprobación. Por AG-09 |
