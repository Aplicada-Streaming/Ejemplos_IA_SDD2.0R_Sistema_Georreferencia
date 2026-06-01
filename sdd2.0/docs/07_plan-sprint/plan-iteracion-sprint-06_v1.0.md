# Plan de Iteración — Sprint 06

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-06_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2026-08-18
**Fecha fin:** 2026-08-29
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (2 dev backend, 1 dev frontend, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 21,7 SP; tope del 110 % = 24 SP. Se comprometen 13 SP. El margen respecto del tope se reserva por el manejo de archivo binario (ZIP) y la reconstrucción completa del grafo de 12 entidades con remapeo de identificadores, ambos con efectos estructurales sobre datos.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 2 | 120 | 0,72 | 86 h |
| Dev frontend | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Permitir que un jefe de área exporte un relevamiento completo (datos, marcadores, observaciones, fotos, comentarios y etiquetas) en un único archivo ZIP para resguardarlo o entregarlo al área central, y que pueda importar ese archivo para reconstruir el relevamiento en el sistema, validando la pertenencia de área y la coherencia del archivo, todo auditado.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-27 | Historia | Exportar el relevamiento completo en un único archivo ZIP | Alta (Must) | 5 | Dev backend A | Pendiente |
| US-28 | Historia | Importar el relevamiento completo desde un archivo ZIP | Alta (Must) | 8 | Dev backend B / Dev frontend | Pendiente |

Total de puntos comprometidos: 13 SP. US-27 y US-28 (Must de EP-07) están `Ready`: cierran el alcance Must de resguardo y traspaso (NB-04) y se apoyan en el grafo de 12 entidades ya persistido (BT-07, completado en el Sprint 04).

## 4. Alcance técnico

Componentes que se construyen o modifican (sobre la arquitectura de 05, sin redefinirla):

1. US-27 — Exportación (CU-08 §5.A): un command que arma un manifiesto JSON del relevamiento (datos, marcadores, observaciones, fotos con su referencia y fuente, comentarios y nombres de etiquetas), lo empaqueta en un único archivo ZIP (`manifiesto.json`) y lo entrega, respetando el acotamiento por rol y área (RN-08, RN-01) y auditando la exportación (RN-07). Opera sobre datos consolidados en cualquier estado, incluido cerrado (US-27 §7). El empaquetado físico (ZIP + JSON) vive en infraestructura tras un puerto, para no acoplar la capa de aplicación al formato.
2. US-28 — Importación (CU-08 §5.B): un command que desempaqueta el ZIP, valida la coherencia del manifiesto (versión, campos requeridos e integridad referencial interna), valida la pertenencia de área del jefe (RN-01) y reconstruye el relevamiento con identificadores nuevos —remapeando las referencias internas— preservando estado, marcadores, observaciones, fotos, comentarios y etiquetas. Audita la importación (RN-07). Ante un archivo incoherente responde `ARCHIVO_EXPORTACION_INVALIDO` (422) sin alterar datos existentes.

El formato del archivo importado es exactamente el que produce la exportación (US-28 §7). Los binarios de las fotos: el sistema persiste la referencia al archivo (no el binario, que vive en la librería de alojamiento configurable, ADR-08, fuera del alcance de este backend); el manifiesto transporta la referencia y la fuente de cada foto. La integración con la librería de alojamiento queda fuera de este sprint.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- La exportación entrega un único archivo ZIP con el relevamiento completo y registra la exportación; si no puede auditarse, responde `ACCION_NO_AUDITADA` y no entrega el archivo.
- La importación reconstruye el relevamiento con sus marcadores, fotos, comentarios y etiquetas, y registra la importación; un archivo incoherente responde `ARCHIVO_EXPORTACION_INVALIDO` sin alterar datos existentes.
- El ciclo exportar → importar es de ida y vuelta: lo importado conserva la estructura y las relaciones de lo exportado (round-trip verificado por pruebas).
- La exportación e importación respetan el acotamiento por rol y área (RN-01, RN-08).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El remapeo de identificadores al importar puede romper relaciones (foto↔observación, comentario↔marcador) | Media | Alto | Construir el grafo con claves locales del manifiesto y un mapa explícito old→new; pruebas de round-trip que verifican conteos y relaciones tras importar |
| Un ZIP corrupto o un manifiesto incompleto podría provocar una importación parcial | Media | Alto | Validar el manifiesto completo antes de tocar la base; reconstruir y persistir en una sola operación; ante invalidez, responder `ARCHIVO_EXPORTACION_INVALIDO` sin escribir |
| Acoplar la capa de aplicación al formato ZIP | Baja | Medio | Aislar el empaquetado (ZIP + JSON) tras el puerto `IEmpaquetadorRelevamiento` en infraestructura; la aplicación trabaja con el manifiesto y un `byte[]` |

## 7. Criterios de hecho del sprint

El Sprint 06 se considera completo cuando US-27 y US-28 están terminadas según la DoD con sus pruebas verdes; el ciclo exportar → importar queda demostrado de ida y vuelta en el sprint review; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU que avanzan | CU-08 (exportación §5.A: US-27; importación §5.B: US-28) |
| NB que avanzan | NB-04 (resguardo y traspaso del relevamiento completo) |
| ADRs que gobiernan | ADR-02 (API REST), ADR-08 (librería de alojamiento de binarios), ADR-01 (CQRS ligero), ADR-09 (persistencia EF Core), ADR-11 (Problem Details) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Plan inicial del Sprint 06 (exportación/importación). Compromete US-27 y US-28 (13 SP). Cierra el alcance Must de resguardo y traspaso de NB-04 con archivo ZIP; la integración con la librería de alojamiento de binarios (ADR-08) queda fuera. Generado por AG-07 |
