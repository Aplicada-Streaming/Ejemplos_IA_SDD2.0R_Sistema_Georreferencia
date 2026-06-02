# Sprint Retrospectiva — Sprint 26

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-26_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Mantener el valor en el núcleo `GeoVial.Revision` (caché LRU + `IrAlMarcador`) permitió cubrirlo con pruebas unitarias deterministas y dejarlo en el gate, con la pantalla MAUI como cáscara fuera de CI.
- La caché LRU quedó cubierta al 100 % (líneas y branches) con pocos casos: ausencia, recencia, descarte y actualización in situ.
- `IrAlMarcador` resolvió de raíz la fricción de "volver al primer marcador" tras editar: la pantalla recarga conservando el contexto con un cambio mínimo en `RecargarAsync`.
- Tras tres sprints de cierre/supply-chain, volver al frente de producto reequilibró el trabajo como se había acordado en la retro S25.

## 2. Qué no salió bien

- La caché es en memoria y por sesión de pantalla; no persiste entre cargas ni comparte con la captura de campo (alcance acotado, aceptable para el pulido).
- El mapa interactivo sigue bloqueado por la clave de proveedor de mapas: el pulido cubre carrusel/posición pero no la visualización geográfica.
- La pantalla MAUI sólo se ejercita por compilación android-arm64 fuera de CI; la lógica de caché/posición se valida en el núcleo, no la UI en sí.

## 3. Qué probar

- Evaluar persistir la caché de fotos en disco (cuota acotada) si el volumen de fotos por relevamiento lo justifica.
- Retomar el mapa interactivo cuando se disponga de la clave de proveedor de mapas.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Publicar v1.0.0 (tag) y verificar paquete + firma + SBOM post-publish | AG-09 (Release manager) | 2027-06-26 | Pendiente |
| Migrar `CapturaE2ETests` al helper `EscenarioE2E` compartido | AG-05 | 2027-06-26 | Pendiente |
| Gestionar la clave de proveedor de mapas para habilitar el mapa interactivo | AG-08 (móvil) | 2027-06-26 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 25 | Estado actual |
| --- | --- |
| Pulido móvil (caché de fotos del carrusel, conservar índice al recargar) | Completada (entregada en este sprint) |
| Publicar v1.0.0 (tag) y verificar paquete + firma + SBOM post-publish | Pendiente (se reitera; acto del Release manager) |
| Migrar `CapturaE2ETests` al helper `EscenarioE2E` compartido | Pendiente (se reitera) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 26 (pulido móvil) con 3 acciones nuevas y seguimiento de las del Sprint 25 (pulido móvil completado). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
