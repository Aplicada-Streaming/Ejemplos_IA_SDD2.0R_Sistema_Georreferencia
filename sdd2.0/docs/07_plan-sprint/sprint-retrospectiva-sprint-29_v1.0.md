# Sprint Retrospectiva — Sprint 29

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-29_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- El disparo por tag funcionó como diseño: un solo `git push` de `v1.0.0` lanzó ambos workflows de publicación (paquete e imágenes).
- Los Dockerfiles de S28 pasaron su primera construcción real: las tres imágenes (front/backend/db) se compilaron y publicaron a GHCR, y se generó su SBOM CycloneDX.
- El diagnóstico de NU5026 fue rápido: el log de Actions dio el error y se reprodujo localmente borrando `bin/obj` de la librería, confirmando la causa raíz antes de tocar nada.
- El fix de NU5026 quedó quirúrgico (sólo el workflow/script) y no rompió el test del gate que inspecciona el `.nupkg`.

## 2. Qué no salió bien

- **Dos** bugs de publicación llegaron hasta el release stable porque **ningún tag había ejercitado nunca los workflows** (el repo no tenía tags): `publish-sync.yml` y `publish-images.yml` corrieron por primera vez justo en `v1.0.0`.
- `publish-sync.yml` falló con NU5026 (interacción `GeneratePackageOnBuild` + `dotnet pack` en checkout limpio); la diferencia "pasa local / falla en CI" se debió a estado de build previo en local.
- `publish-images.yml` **publicó las imágenes pero la firma cosign keyless se colgó** (>10 min) y hubo que cancelar: las imágenes quedaron sin firmar y el step no tenía `timeout`, por lo que el cuelgue no fallaba solo.
- El release no se completó en un solo intento: requiere re-disparar el tag con los fixes para obtener artefactos firmados.

## 3. Qué probar

- Validar los workflows que sólo corren en tag con un tag **preview** (`-rc.N`) antes del stable: habría detectado NU5026 y el cuelgue de la firma sin afectar el release stable.
- Poner `timeout-minutes` en todo step de red de larga cola (firma/verificación) para que un cuelgue falle rápido con log, no indefinidamente (ya aplicado a ambos workflows).
- Para "pasa local / falla en CI", reproducir siempre sobre un árbol limpio (borrar `bin/obj`) antes de concluir.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Resolver la causa raíz del cuelgue de la firma cosign keyless (revisar log de la corrida cancelada) | AG-09 (DevOps) | 2027-07-24 | En curso |
| Re-disparar `v1.0.0` con los fixes mergeados y verificar paquete + imágenes (firma + SBOM) post-publish | AG-09 (Release manager) | 2027-07-24 | En curso |
| Adoptar un tag preview `-rc` para validar los workflows de tag antes de cada stable | AG-09 (DevOps) | 2027-08-07 | Pendiente |
| Gestionar la clave de proveedor de mapas para habilitar el mapa interactivo | AG-08 (móvil) | 2027-08-07 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 28 | Estado actual |
| --- | --- |
| Publicar v1.0.0 (tag) y validar paquete + imágenes (firma + SBOM) post-publish | En curso (tag creado; imágenes publicadas; paquete re-disparado tras corregir NU5026) |
| Gestionar la clave de proveedor de mapas para el mapa interactivo | Pendiente (se reitera) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 29 (release v1.0.0): release por tag funcional, imágenes OK en su 1ª corrida, NU5026 del paquete diagnosticado y corregido; 3 acciones nuevas (re-release, tag preview de validación, clave de mapas). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
