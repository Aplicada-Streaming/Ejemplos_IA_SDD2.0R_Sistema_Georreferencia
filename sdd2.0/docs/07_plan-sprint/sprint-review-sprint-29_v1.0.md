# Sprint Review — Sprint 29

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-29_v1.0.md
**Versión:** 1.1
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-29_v1.0.md`:

> Publicar el primer release **stable** `v1.0.0`: crear y empujar el tag `v1.0.0` sobre `main`, que dispara los dos workflows de publicación —`publish-sync.yml` (paquete `GeoVial.Sync` a GitHub Packages, con SBOM + firma) y `publish-images.yml` (las tres imágenes Docker a GHCR, con SBOM + firma)— y deja los artefactos firmados y verificables.

Veredicto: Cumplido — `v1.0.0` stable publicado y firmado (paquete + 3 imágenes); el camino expuso y corrigió cuatro bugs latentes de los workflows de publicación, validados con tags preview antes del stable.

Explicación corta: el primer release real ejercitó por primera vez ambos workflows de publicación (el repo no tenía tags), lo que afloró **cuatro bugs latentes** —ninguno detectable sin disparar un tag—. En vez de arriesgar el stable, se adoptó la acción de la propia retro: **validar con tags preview `v1.0.0-rc.N`** hasta dejar el pipeline verde, y recién entonces taggear `v1.0.0` stable. Bugs encontrados y corregidos:

1. **NU5026** (`publish-sync.yml`, pack): en checkout limpio, `GeneratePackageOnBuild=true` (que el `.csproj` mantiene para el test del gate del `.nupkg`) interfiere con `dotnet pack`. Fix: `-p:GeneratePackageOnBuild=false` sólo en el workflow/script. Confirmado en rc.1.
2. **SBOM CycloneDX** (`publish-sync.yml`): la CLI cambió en 6.x (el flag JSON pasó de `-j` a `-F Json`). Fix: `-F Json` + anclar el tool a 6.2.0. Confirmado en rc.2.
3. **Cuelgue de la firma cosign keyless** (`publish-images.yml`): la matriz firmaba las 3 imágenes en paralelo y saturaba el Fulcio/Rekor public-good de sigstore → la firma se colgaba >20 min. El paquete (1 job) firmaba sin problema. Fix: `max-parallel: 1` (serializar). Confirmado en rc.3 (sign + attest + verify de imagen completaron OK).
4. **Presupuesto del step de firma** (`publish-images.yml`): cuatro ceremonias keyless+registro encadenadas (sign, attest, verify, verify-attestation) excedían el timeout del step. Fix: el pipeline de imágenes sólo **firma y atesta** (lo que queda en el registro y en Rekor); la verificación se difiere al promotor entre ambientes y al consumidor (`guia-publicacion-image-docker §3`). Confirmado en rc.4.

Con rc.4 verde de punta a punta, se taggeó **`v1.0.0` stable**: `publish-sync.yml` publicó el paquete `GeoVial.Sync 1.0.0` al canal **stable** de GitHub Packages (firmado + SBOM, ambos verificados en el pipeline), y `publish-images.yml` publicó las tres imágenes a GHCR con tag `1.0.0`, `sha-<corto>` y `:latest`, cada una firmada con cosign keyless y con su SBOM CycloneDX atestado. EP-09 queda **cerrada**; la superficie pública `Abstractions` queda congelada a partir de `v1.0.0` (ADR-07).

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| EP-09 | Release | `GeoVial.Sync 1.0.0` publicado al canal stable de GitHub Packages, firmado + SBOM | Librería consumible y verificable |
| EP-09 | Release | Las 3 imágenes (front/backend/db) publicadas a GHCR con `:latest`, firmadas + SBOM atestado | Monolito desplegable y verificable |
| EP-09 | Proceso | Validación con tags preview `rc.1`–`rc.4` antes del stable; 4 bugs de pipeline corregidos | Release stable sin sorpresas |

## 3. Feedback recibido

- El primer release real ejercitó por primera vez ambos workflows: ninguno había corrido nunca sobre un tag, así que cuatro bugs latentes (pack, SBOM, concurrencia de firma, presupuesto del step) sólo aparecieron aquí.
- Validar con tags preview `rc.N` fue decisivo: cada bug se encontró y corrigió sin arriesgar el stable; recién con rc.4 verde se taggeó `v1.0.0`.
- Los fixes son quirúrgicos (workflows/script; el `.csproj` no cambia, el gate sigue verde) y dejan el pipeline reproducible para los próximos releases.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 4 bugs de pipeline (NU5026, SBOM `-j`, concurrencia de firma, presupuesto del step), todos corregidos en el sprint |

Pruebas: 315 verdes (278 unitarias + 37 de integración), sin cambio respecto del Sprint 28: sprint de release/DevOps sin lógica de dominio nueva. El gate de cobertura se mantiene; el test que inspecciona el `.nupkg` sigue verde tras los fixes (el `.csproj` no cambió). Verificación específica: cuatro corridas preview `rc.1`–`rc.4` hasta dejar ambos workflows verdes, y la corrida stable de `v1.0.0` (paquete + imágenes) en verde. Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-RELEASE-1.0.0 | Tarea | Aceptada (`v1.0.0` stable publicado y firmado: paquete a GitHub Packages + 3 imágenes a GHCR; 4 bugs de pipeline corregidos y validados con tags preview) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 29 se traslada; `v1.0.0` quedó publicado. |

Único frente de producto restante: el mapa interactivo (bloqueado por la clave de proveedor de mapas). El resto es mantenimiento post-release (Dependabot/CVE por SLA, PATCH si surge).

## 7. Decisiones tomadas durante el review

- Validar los workflows de tag con tags preview `rc.N` antes del stable: regla permanente para futuros releases (un workflow que sólo corre en tag no se ejercita de otra forma).
- Corregir NU5026 con `-p:GeneratePackageOnBuild=false` sólo en el workflow/script, conservando `GeneratePackageOnBuild` en el `.csproj` (lo necesita el test del gate).
- Serializar la firma de imágenes (`max-parallel: 1`) para no saturar el sigstore public-good; y firmar+atestar en el pipeline, difiriendo la verificación al promotor/consumidor.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 29 (release v1.0.0). Veredicto Parcial inicial (release iniciado, 2 bugs detectados). Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
| 1.1 | 2026-06-03 | `v1.0.0` stable publicado y firmado (paquete + 3 imágenes) tras validar el pipeline con tags preview `rc.1`–`rc.4`; cuatro bugs de pipeline corregidos. Veredicto a Cumplido. Por AG-07 |
