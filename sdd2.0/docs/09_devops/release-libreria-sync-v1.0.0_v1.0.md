# Release de la librería — GeoVial.Sync v1.0.0

**Proyecto:** GeoVial
**Documento:** release-libreria-sync-v1.0.0_v1.0.md
**Versión:** 1.1
**Estado:** En release (tag `v1.0.0` creado en el Sprint 29; publicación stable tras corregir NU5026)
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

### SBOM (inventario de dependencias)

El pipeline genera el SBOM **CycloneDX (JSON)** del paquete (`geovial-sync.cdx.json`, supply-chain-seguridad §1,
STAGE-09), lo firma con cosign (STAGE-10) y lo adjunta al release. El consumidor verifica su integridad igual
que el paquete:

```bash
cosign verify-blob geovial-sync.cdx.json \
  --bundle geovial-sync.cdx.json.cosign.bundle \
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

> **Release (Sprint 29).** El Release manager creó y empujó el tag anotado `v1.0.0` sobre `main` (verde en
> CI). El push disparó **ambos** workflows de publicación: `publish-sync.yml` (paquete a GitHub Packages,
> canal stable, con SBOM + firma) y `publish-images.yml` (las tres imágenes Docker a GHCR, con SBOM + firma).
> A partir de aquí la superficie pública `Abstractions` queda congelada: todo breaking change bumpea MAJOR.
> Nota: desde el Sprint 28 el tag `v*` dispara también el build/firma de las imágenes, además del paquete.
>
> **Incidencias de la primera ejecución.** Como ningún tag había ejercitado nunca los workflows, el primer
> push de `v1.0.0` afloró dos bugs latentes:
>
> 1. `publish-sync.yml` falló en el step de empaquetado con NU5026 ("la DLL no se encuentra"). Causa raíz: en
>    un checkout limpio, `GeneratePackageOnBuild=true` (que el `.csproj` mantiene para el test del gate que
>    inspecciona el `.nupkg`) interfiere con `dotnet pack`. Fix: empaquetar con
>    `-p:GeneratePackageOnBuild=false` (sólo en el workflow/script, sin tocar el `.csproj`), reproducido y
>    verificado localmente. No llegó a publicarse nada del paquete.
> 2. `publish-images.yml` construyó y publicó las tres imágenes a GHCR y generó su SBOM, pero el step de firma
>    cosign keyless se colgó (>10 min) y se canceló: las imágenes quedaron publicadas pero **sin firmar**.
>    Mitigación: `timeout-minutes` en los steps de firma de ambos workflows, `COSIGN_YES` para confirmación no
>    interactiva y subida del SBOM como artefacto antes de firmar. La causa raíz del cuelgue se investiga.
>
> La publicación **firmada** de ambos artefactos se completa re-disparando `v1.0.0` sobre `main` con los fixes
> mergeados; como nada se publicó del paquete y las imágenes se re-firman en el re-disparo, no hay artefacto
> firmado que reescribir.

## 3. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Notas de release y checklist del primer stable v1.0.0 de GeoVial.Sync (Sprint 21). Empaquetado en build verificado por prueba del contenido del `.nupkg`; publicación por tag con aprobación. Por AG-09 |
| 1.1 | 2026-06-02 | Release efectivo (Sprint 29): se creó y empujó el tag anotado `v1.0.0` sobre `main` (verde en CI), que disparó `publish-sync.yml` y `publish-images.yml` (paquete + imágenes, con SBOM + firma). Estado a Released; superficie `Abstractions` congelada. Por AG-09 |
