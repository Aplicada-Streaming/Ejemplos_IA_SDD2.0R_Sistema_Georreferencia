# Sprint Retrospectiva — Sprint 25

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-25_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Reusar el mismo flujo de firma keyless para el SBOM (además del `.nupkg`) fue trivial: un solo bucle firma y verifica ambos artefactos.
- La herramienta CycloneDX para .NET genera el SBOM con una invocación directa; encajó en el workflow sin fricción.
- El endurecimiento de supply-chain de la librería queda completo y coherente con `supply-chain-seguridad` (SBOM §1 + firma §2), cerrando una acción que venía de las retros S22 y S24.
- Adjuntar el SBOM firmado al release deja el inventario de dependencias verificable por cualquier consumidor.

## 2. Qué no salió bien

- Como los sprints de release previos, el SBOM solo se ejercita al disparar el workflow por tag; no hay verificación en el gate de pruebas local.
- El SBOM cubre el paquete de la librería, no las imágenes Docker del monolito (esos stages de imagen siguen pendientes).
- Tres sprints seguidos de supply-chain/DevOps (S22 firma, S25 SBOM, con S21 release prep) sin lógica de dominio: conviene volver a un frente de producto (pulido móvil) para equilibrar.

## 3. Qué probar

- Tras taggear `v1.0.0`, verificar el SBOM y su firma desde un consumidor, junto con la instalación del paquete.
- Volver al frente de producto con el pulido móvil (caché de fotos, mapa interactivo) en el próximo sprint.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Pulido móvil (caché de fotos del carrusel, conservar índice al recargar) | AG-08 (móvil) | 2027-06-12 | Pendiente |
| Publicar v1.0.0 (tag) y verificar paquete + firma + SBOM post-publish | AG-09 (Release manager) | 2027-06-12 | Pendiente |
| Migrar `CapturaE2ETests` al helper `EscenarioE2E` compartido | AG-05 | 2027-06-12 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 24 | Estado actual |
| --- | --- |
| SBOM CycloneDX firmado del paquete junto al `.nupkg` | Completada (entregada en este sprint) |
| Migrar `CapturaE2ETests` al helper `EscenarioE2E` compartido | Pendiente (se reitera) |
| Pulido móvil pendiente (mapa interactivo, caché de fotos) | Pendiente (se reitera; priorizada para el próximo sprint) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 25 con 3 acciones nuevas y seguimiento de las del Sprint 24 (SBOM firmado completado). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
