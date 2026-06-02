# Sprint Review — Sprint 19

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-19_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-19_v1.0.md`:

> Permitir que el agente enriquezca un marcador desde la app móvil durante la revisión: agregar un comentario al marcador y etiquetar sus fotos y comentarios (US-15, CU-09), consumiendo los endpoints ya existentes (Sprint 04).

Veredicto: Cumplido.

Explicación corta: el nuevo `ClienteEdicionMarcador` agrega un comentario al marcador (`POST /marcadores/{id}/comentarios`) y etiqueta fotos (`POST /fotos/{id}/etiquetas`) o comentarios (`POST /comentarios/{id}/etiquetas`), validando localmente que el texto y la etiqueta no estén vacíos antes de llamar al backend, y reflejando los rechazos del backend (por ejemplo, relevamiento cerrado → solo lectura) como un resultado de error. La pantalla de revisión en `GeoVial.Mobile` permite, sobre el marcador en foco del carrusel, escribir un comentario y etiquetar la foto, recargando la revisión tras cada edición.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-15 | Historia | Agregar un comentario al marcador en foco: se postea al backend y aparece al recargar | Marcadores enriquecibles en campo |
| US-15 | Historia | Etiquetar la foto en foco del marcador | Clasificación rápida |
| US-15 | Historia | Un comentario o etiqueta vacíos se rechazan en el cliente sin llamar al backend | Validación temprana |
| US-15 | Historia | El backend que rechaza la edición (cerrado → solo lectura) se refleja como error | RN-05 respetada |

## 3. Feedback recibido

- La edición sobre el marcador cierra el ciclo de la revisión en el cliente móvil: ya no es solo lectura.
- Reusar los endpoints del Sprint 04 dejó el sprint enteramente de frente cliente, sin reabrir backend.
- La validación local evita viajes innecesarios al backend para entradas vacías.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 299 verdes (269 unitarias + 30 de integración), +6 respecto del Sprint 18 (agregar comentario, etiquetar foto/comentario, validaciones de vacío y reflejo del rechazo del backend). Cobertura del núcleo `GeoVial.Revision`: 97,7 % líneas / 100 % branches (gate ≥ 80 % / ≥ 70 % cumplido). Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-15 | Historia | Aceptada en su frente cliente (comentarios y etiquetas; la pantalla MAUI compila fuera de CI). El alta/baja de fotos se cubre por la captura (US-11) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 19 se traslada. |

El mapa interactivo (control de mapas), la caché de fotos del carrusel y el alta/baja explícita de fotos desde la revisión quedan en el backlog.

## 7. Decisiones tomadas durante el review

- Acotar US-15 cliente a comentarios y etiquetas; el alta de fotos es la captura (US-11), ya entregada.
- Recargar la revisión tras cada edición para reflejar el cambio (simplicidad sobre actualización incremental).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 19 (edición sobre el marcador desde el móvil: comentarios y etiquetas, US-15 frente cliente). Veredicto Cumplido, velocity 8, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
