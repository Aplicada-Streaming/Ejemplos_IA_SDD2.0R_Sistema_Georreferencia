# Sprint Review — Sprint 22

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-22_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-22_v1.0.md`:

> Firmar el paquete `GeoVial.Sync` en el pipeline de publicación con cosign (sigstore) en modo keyless (OIDC de GitHub Actions, sin llaves privadas de larga vida), verificando la firma en el propio workflow antes de publicar y documentando la verificación para los consumidores.

Veredicto: Cumplido.

Explicación corta: el workflow `publish-sync.yml` ahora instala cosign y firma cada `.nupkg` en modo keyless con el OIDC de Actions (`id-token: write`), produciendo un bundle detached (`.cosign.bundle`) con certificado efímero y registro en Rekor; acto seguido lo verifica con `cosign verify-blob` contra la identidad del workflow y el emisor OIDC, de modo que una firma inválida aborta el publish (STAGE-10 → STAGE-13). El bundle se adjunta como artefacto del release. La verificación para consumidores quedó documentada en las notas de release y la guía de publicación; `supply-chain-seguridad §2` se marcó como implementado para el paquete de la librería y el CHANGELOG lo registra.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| EP-09 | Supply-chain | El workflow firma el `.nupkg` con cosign keyless (sin gestión de llaves) | Firma sin secretos que rotar |
| EP-09 | Supply-chain | El workflow verifica la firma antes de publicar; una firma inválida aborta el publish | Red de seguridad en el release |
| EP-09 | Documentación | Procedimiento `cosign verify-blob` para consumidores en notas de release y guía | Verificable por terceros |

## 3. Feedback recibido

- La firma keyless (sigstore) evita gestionar certificados de larga vida: el OIDC de Actions emite un certificado efímero por ejecución, registrado en el transparency log público.
- El paquete queda listo para declararse plenamente consumible en stable: empaquetado verificado (S21) + firmado y verificado (S22).
- La firma de las imágenes Docker y los SBOM sigue pendiente (stages de imagen), fuera del alcance de la librería.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 303 verdes (270 unitarias + 33 de integración), sin cambio respecto del Sprint 21: es un sprint de supply-chain/DevOps sin lógica de dominio nueva, por lo que el gate de cobertura se mantiene. La verificación es la firma y su validación dentro del workflow de publicación. Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-SIGN | Tarea | Aceptada (firma cosign keyless + verificación en el workflow + documentación de verificación) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 22 se traslada. |

La publicación efectiva de `v1.0.0` (tag por el Release manager), la firma de las imágenes Docker y los SBOM, y el pulido móvil (mapa interactivo, caché) siguen en el backlog.

## 7. Decisiones tomadas durante el review

- Firma keyless con cosign (sin llaves privadas), conforme a `supply-chain-seguridad §2`, en lugar de la firma NuGet con certificado X.509.
- Verificar la firma dentro del propio workflow antes de publicar (autovalidación), no solo dejarla para el consumidor.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 22 (firma del paquete de la librería con cosign keyless + verificación en el workflow). Veredicto Cumplido, velocity 8, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
