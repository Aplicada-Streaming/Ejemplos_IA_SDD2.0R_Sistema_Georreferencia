# Sprint Retrospectiva — Sprint 22

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-22_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- La firma keyless (cosign + OIDC de Actions) se integró sin gestionar secretos ni certificados de larga vida: el único requisito fue añadir `id-token: write` al workflow.
- Verificar la firma en el propio pipeline (no solo dejarla documentada) convierte la firma en un gate efectivo: si no valida, el publish no ocurre.
- La política ya estaba escrita (`supply-chain-seguridad §2`): implementarla fue traducir un diseño claro a pasos del workflow, sin reabrir decisiones.
- Adjuntar el bundle `.cosign.bundle` al release deja la verificación al alcance de cualquier consumidor con un comando.

## 2. Qué no salió bien

- La firma solo se ejercita al disparar el workflow por tag; no hay forma de probarla en el gate de pruebas local, así que la verificación efectiva ocurre recién en el primer release `v1.0.0`.
- Quedó pendiente el SBOM (CycloneDX) firmado del paquete, que `supply-chain-seguridad` también contempla; este sprint se acotó a la firma del artefacto.
- La firma de las imágenes Docker (los otros artefactos del §2) no se tocó; pertenece a los stages de imagen, no al de la librería.

## 3. Qué probar

- Generar y firmar el SBOM (CycloneDX) del paquete junto al `.nupkg` en el mismo stage.
- Tras taggear `v1.0.0`, verificar de punta a punta la firma del paquete publicado desde un consumidor limpio.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| SBOM CycloneDX firmado del paquete junto al `.nupkg` | AG-09 (DevOps) | 2027-05-01 | Pendiente |
| Publicar v1.0.0 (tag) y verificar la firma del paquete publicado | AG-09 (Release manager) | 2027-05-01 | Pendiente |
| Pulido móvil pendiente (mapa interactivo, caché de fotos) y E2E del ciclo de conflictos | AG-08 / AG-05 | 2027-05-01 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 21 | Estado actual |
| --- | --- |
| Firmar el paquete en el workflow de publicación y verificar la firma | Completada (cosign keyless + verificación en el workflow) |
| Taggear v1.0.0 y validar la instalación post-publish en un consumidor limpio | Pendiente (release del Release manager) |
| Pulido móvil pendiente y E2E del ciclo de conflictos | Pendiente (se reitera) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 22 con 3 acciones nuevas y seguimiento de las del Sprint 21 (firma del paquete completada). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
