# Checklist de release (genérico) — GeoVial

**Proyecto:** GeoVial
**Documento:** checklist-release_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado (Sprint 33)
**Fecha:** 2026-06-03
**Autor:** Ingeniero DevOps Senior (AG-09), Equipo SDD 2.0
**Trazabilidad upstream:** estrategia-versionado (canales/tags); supply-chain-seguridad; pipeline-ci-cd (STAGE-09/10/11/12/13/14)
**Trazabilidad downstream:** release-libreria-sync-v1.0.0 (caso concreto del paquete v1.0.0)

Checklist obligatorio para cualquier **release por tag** de GeoVial (paquete `GeoVial.Sync` y/o imágenes Docker del monolito). Su regla central nace de un incidente real: en el primer release `v1.0.0` (Sprint 29), el **primer** push de un tag ejercitó por primera vez los workflows de publicación y afloraron **cuatro bugs latentes** (pack NU5026, flag del SBOM CycloneDX, concurrencia de la firma de imágenes y presupuesto del step de firma) que **ninguna prueba local podía detectar**. La conclusión institucional: **un workflow que sólo corre en tag se valida con un tag preview `-rc` antes del stable.**

## 1. Pre-condiciones

- [ ] `main` verde en CI (build + tests + cobertura sobre el gate).
- [ ] CHANGELOG con la sección de la versión a publicar.
- [ ] Sin secretos en el árbol; los secretos de publicación viven en GitHub Secrets (rotación 90 días).

## 2. Validación con tag preview `-rc` (OBLIGATORIA antes del stable)

> Regla: **no se taggea un stable sin haber validado el pipeline con un `-rc` verde de punta a punta.** Aplica a todo cambio en los workflows de publicación (`publish-sync.yml`, `publish-images.yml`) y a cada release.

1. [ ] Crear y empujar un tag preview: `git tag -a v<X.Y.Z>-rc.N -m "..." && git push origin v<X.Y.Z>-rc.N`.
2. [ ] Verificar que **ambos** workflows disparados por el tag terminan **verdes**:
   - `publish-sync.yml`: pack → verificación del `.nupkg` → SBOM → firma cosign + verificación → publish (canal preview).
   - `publish-images.yml`: build → SBOM por imagen → firma cosign (imágenes en serie, `max-parallel: 1`) → publish a GHCR.
3. [ ] Si algo falla: corregir en `main` (vía PR), borrar el `-rc` fallido, y repetir con `-rc.(N+1)`. Iterar hasta verde. Nunca taggear el stable con el pipeline en rojo.
4. [ ] Confirmar artefactos preview: el paquete aparece en el canal preview y las imágenes en GHCR con el tag `-rc.N`.

## 3. Tag stable y publicación

1. [ ] Con el `-rc` verde, crear y empujar el tag stable: `git tag -a v<X.Y.Z> -m "..." && git push origin v<X.Y.Z>`.
2. [ ] Verificar que ambos workflows terminan verdes para el stable (paquete al canal stable, imágenes con `:latest`).
3. [ ] Confirmar firma + SBOM de cada artefacto (registrados en Rekor; verificables por el consumidor, ver guías de publicación).

## 4. Post-publish y limpieza

- [ ] Verificación de consumo: instalación de prueba del paquete (`samples/01-sync-basico`) y `cosign verify`/`verify-blob` de los artefactos (guías de publicación).
- [ ] CHANGELOG fechado y release notes con la versión publicada.
- [ ] Borrar los tags `-rc.N` de validación (opcional; dejan rastro en el feed preview hasta delistarse).
- [ ] Rollback si hace falta: delist/deprecate + PATCH para el paquete; reversión de deploy para las imágenes (guías de publicación §4).

## 5. Notas

- El `timeout-minutes` del runner **no** mata procesos colgados en espera de red; por eso las llamadas a `cosign` se acotan además con `timeout` de coreutils. Un cuelgue debe fallar rápido y dejar log, nunca colgar indefinidamente.
- La firma keyless de imágenes corre **en serie** (`max-parallel: 1`) para no saturar el Fulcio/Rekor public-good de sigstore.

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Checklist de release genérico (Sprint 33): institucionaliza la validación con tag preview `-rc` antes de cada stable (lección del incidente del Sprint 29), más pre-condiciones, tag stable, post-publish y notas de robustez (timeout de cosign, firma en serie). Por AG-09 |
