# Sprint Review — Sprint 26

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-26_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-26_v1.0.md`:

> Pulir la experiencia de revisión en la app móvil: cachear en memoria las fotos del carrusel para no re-descargarlas al navegar y conservar la posición del carrusel (marcador en foco) al recargar la revisión tras una edición.

Veredicto: Cumplido.

Explicación corta: el núcleo testeable `GeoVial.Revision` incorpora `CacheFotos`, una caché LRU en memoria (clave `FotoId` → bytes) con capacidad máxima configurable que descarta la entrada usada menos recientemente al excederse, y `NavegadorRevision.IrAlMarcador(Guid)`, que reposiciona el carrusel en un marcador por su identificador reiniciando la foto. La pantalla de revisión MAUI (`GeoVial.Mobile`, fuera de CI) ahora sirve la foto en foco desde la caché sin re-descargarla y, tras agregar un comentario o etiquetar una foto, recarga la revisión conservando el marcador en foco con `IrAlMarcador`. Con esto se atiende el pulido móvil que venía reiterándose en las retros S17/S19/S23/S25 y el equipo vuelve al frente de producto tras los sprints de cierre E2E y supply-chain.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-22 | Pulido móvil | Navegar el carrusel reusa la foto cacheada en memoria (sin re-descarga) | Navegación más fluida |
| US-22 | Pulido móvil | Al exceder la capacidad, la caché descarta la foto menos usada (LRU) | Memoria acotada |
| US-21 | Pulido móvil | Tras editar (comentario/etiqueta), la revisión recarga y conserva el marcador en foco | No se pierde el contexto al editar |

## 3. Feedback recibido

- La caché en memoria mejora la fluidez del carrusel sin crecer sin límite (descarte LRU), cerrando el anti-patrón "re-descarga al navegar".
- Conservar el marcador en foco al recargar elimina la fricción de volver al primer marcador tras cada edición.
- El mapa interactivo sigue pendiente por requerir una clave de proveedor de mapas, fuera del alcance; la revisión continúa por coordenada/carrusel.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 315 verdes (278 unitarias + 37 de integración), +8 unitarias respecto del Sprint 25 (caché LRU + `IrAlMarcador`). Cobertura del gate: Domain líneas 89,7 % / branches 79,8 %; Application 90,0 % / 82,0 %; Revision 98,7 % / 100 % (`CacheFotos` 100 % / 100 %, `NavegadorRevision` 96,5 % / 100 %). La pantalla MAUI queda fuera del gate y compila para `net10.0-android` (android-arm64). Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-UX-MOVIL | Tarea | Aceptada (caché LRU de fotos + `IrAlMarcador` con pruebas verdes; pantalla MAUI usando ambos) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 26 se traslada. |

Siguen en el backlog: la publicación efectiva de `v1.0.0` (tag por el Release manager), el SBOM y la firma de las imágenes Docker del monolito, el mapa interactivo (requiere clave de proveedor) y la migración de `CapturaE2ETests` al helper `EscenarioE2E`.

## 7. Decisiones tomadas durante el review

- Mantener el valor testeable en `GeoVial.Revision` (caché LRU + navegación) dentro del gate y dejar la pantalla MAUI fuera de CI, igual que en S17/S19.
- Capacidad por defecto de la caché en 8 fotos (suficiente para el carrusel típico), configurable por constructor.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 26 (pulido móvil: caché LRU de fotos del carrusel + conservar la posición al recargar). Veredicto Cumplido, velocity 8, 0 carry-over, 315 pruebas verdes. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
