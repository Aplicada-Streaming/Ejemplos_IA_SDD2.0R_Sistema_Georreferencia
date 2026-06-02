# Sprint Review — Sprint 25

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-25_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-25_v1.0.md`:

> Generar el SBOM (CycloneDX, JSON) del paquete `GeoVial.Sync` y firmarlo con cosign junto al `.nupkg` en el pipeline de publicación (supply-chain-seguridad §1, STAGE-09 → STAGE-10), adjuntándolo al release para que el consumidor verifique el inventario de dependencias y su integridad.

Veredicto: Cumplido.

Explicación corta: el workflow `publish-sync.yml` ahora genera el SBOM CycloneDX (JSON) del paquete con la herramienta CycloneDX para .NET (STAGE-09) y lo firma con cosign keyless junto al `.nupkg` (STAGE-10), verificando su firma antes de publicar y adjuntándolo —con su bundle— como artefacto del release. Se documentó la verificación del SBOM para el consumidor; `supply-chain-seguridad §1` quedó marcado como implementado para la librería y el CHANGELOG lo registra. Con esto, el endurecimiento de supply-chain de la librería (SBOM + firma) está completo.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| EP-09 | Supply-chain | El workflow genera el SBOM CycloneDX del paquete (STAGE-09) | Inventario de dependencias verificable |
| EP-09 | Supply-chain | El SBOM se firma con cosign y se verifica antes de publicar (STAGE-10) | Integridad del SBOM garantizada |
| EP-09 | Documentación | Procedimiento `cosign verify-blob` del SBOM para consumidores | Verificable por terceros |

## 3. Feedback recibido

- El SBOM cierra el anti-patrón "falta de SBOM" para la librería: el consumidor obtiene el inventario de dependencias junto al paquete, firmado.
- El endurecimiento de supply-chain de la librería queda completo: empaquetado verificado (S21), firmado (S22) y con SBOM firmado (S25).
- La generación de SBOM de las imágenes Docker sigue pendiente de los stages de imagen, fuera del alcance de la librería.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 307 verdes (270 unitarias + 37 de integración), sin cambio respecto del Sprint 24: es un sprint de supply-chain/DevOps sin lógica de dominio nueva, por lo que el gate de cobertura se mantiene. La verificación es la generación y firma del SBOM dentro del workflow de publicación. Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-SBOM | Tarea | Aceptada (SBOM CycloneDX generado + firmado con cosign + verificado + adjunto al release) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 25 se traslada. |

La publicación efectiva de `v1.0.0` (tag por el Release manager), el SBOM y la firma de las imágenes Docker, y el pulido móvil (mapa interactivo, caché) siguen en el backlog.

## 7. Decisiones tomadas durante el review

- Generar el SBOM con la herramienta CycloneDX para .NET (formato estándar) y firmarlo en el mismo flujo keyless que el paquete.
- Verificar la firma del SBOM dentro del propio workflow antes de publicar, igual que el `.nupkg`.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 25 (SBOM CycloneDX firmado del paquete). Veredicto Cumplido, velocity 8, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
