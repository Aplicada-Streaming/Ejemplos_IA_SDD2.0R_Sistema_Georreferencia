# Plan de Iteración — Sprint 19

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-19_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-02-23
**Fecha fin:** 2027-03-06
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,7 SP (S16–S18); capacidad sugerida estricta 10 SP. Se compromete US-15 (frente cliente, 8 SP). El valor testeable —la validación y el armado de las ediciones del marcador— entra al gate; la pantalla MAUI queda fuera de CI.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Permitir que el agente enriquezca un marcador desde la app móvil durante la revisión: agregar un comentario al marcador y etiquetar sus fotos y comentarios (US-15, CU-09), consumiendo los endpoints ya existentes (Sprint 04). El bloqueo de solo lectura tras el cierre lo aplica el backend (RN-05).

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-15 | Historia | Gestionar comentarios y etiquetas del marcador (frente cliente móvil) | Alta (Must) | 8 | Dev móvil / Dev fullstack | Pendiente |

Total de puntos comprometidos: 8 SP. El backend de comentarios y etiquetas (CU-09) está entregado desde el Sprint 04; este sprint construye el frente cliente: el cliente de edición y la integración en la pantalla de revisión. El alta/baja de la foto en sí se cubre por la captura (US-11) y queda fuera del alcance de este sprint, centrado en comentarios y etiquetas.

## 4. Alcance técnico

1. **`GeoVial.Revision`** (en la solución y el gate): `ClienteEdicionMarcador` agrega un comentario al marcador (`POST /marcadores/{id}/comentarios`) y etiqueta una foto (`POST /fotos/{id}/etiquetas`) o un comentario (`POST /comentarios/{id}/etiquetas`). Valida localmente que el texto y la etiqueta no estén vacíos antes de llamar al backend, devolviendo un resultado uniforme (éxito o mensaje de error).
2. **Pantalla de revisión en `GeoVial.Mobile`** (fuera de CI): sobre el marcador en foco del carrusel, permite escribir un comentario y agregarlo, y etiquetar la foto en foco; tras una edición exitosa recarga la revisión para reflejar el cambio.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- El cliente agrega un comentario al marcador y etiqueta una foto/comentario contra los endpoints existentes.
- Un comentario o una etiqueta vacíos se rechazan en el cliente antes de llamar al backend.
- El backend que rechaza la edición (por ejemplo, relevamiento cerrado → `RELEVAMIENTO_SOLO_LECTURA`) se refleja como un resultado de error en el cliente.
- El núcleo `GeoVial.Revision` respeta el gate de cobertura (líneas ≥ 80 %, branches ≥ 70 %); la pantalla MAUI queda fuera del gate.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| La edición sobre un relevamiento cerrado debe bloquearse | Baja | Bajo | El backend aplica RN-05 (`RELEVAMIENTO_SOLO_LECTURA`); el cliente refleja el error sin asumir el estado |
| La pantalla MAUI no compila en CI (Android SDK) | Alta | Bajo | `GeoVial.Mobile` queda fuera de la solución/CI; el núcleo testeable vive en `GeoVial.Revision` |
| El alcance de US-15 incluye alta/baja de fotos | Media | Bajo | El alta de foto es captura (US-11, ya entregada); este sprint se acota a comentarios y etiquetas |

## 7. Criterios de hecho del sprint

El Sprint 19 se considera completo cuando US-15 (frente cliente, comentarios y etiquetas) está terminada según la DoD con sus pruebas verdes: el cliente agrega comentarios y etiquetas validando localmente y reflejando los rechazos del backend; la pantalla de revisión permite editar el marcador en foco; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US que avanzan | US-15 (gestionar comentarios y etiquetas del marcador, frente cliente) |
| CU que avanzan | CU-09 (gestión del marcador) |
| EP | EP-03 (Captura y georreferenciación) / EP-04 (revisión) |
| NB que avanzan | NB-04 (marcadores ricos para la revisión) |
| RN aplicadas | RN-05 (solo lectura), RN-01 (autorización), RC-04 (etiquetas muchos a muchos) |
| BT derivadas | BT-05, BT-07, BT-09 |
| Tests previstos | acceptance/AT-09-gestion-marcador |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 19 (edición sobre el marcador desde el móvil: comentarios y etiquetas, US-15 frente cliente). Compromete 8 SP. El núcleo `GeoVial.Revision` entra al gate; la pantalla MAUI queda fuera de CI. Generado por AG-07 |
