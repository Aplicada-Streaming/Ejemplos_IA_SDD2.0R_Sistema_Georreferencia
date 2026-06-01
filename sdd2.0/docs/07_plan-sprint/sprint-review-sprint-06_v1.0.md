# Sprint Review — Sprint 06

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-06_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-06_v1.0.md`:

> Permitir que un jefe de área exporte un relevamiento completo (datos, marcadores, observaciones, fotos, comentarios y etiquetas) en un único archivo ZIP para resguardarlo o entregarlo al área central, y que pueda importar ese archivo para reconstruir el relevamiento en el sistema, validando la pertenencia de área y la coherencia del archivo, todo auditado.

Veredicto: Cumplido.

Explicación corta: la exportación arma el manifiesto del relevamiento completo y lo entrega en un único archivo ZIP, autorizada por área (RN-01, RN-08) y auditada (RN-07); la importación desempaqueta y valida la coherencia del archivo, autoriza por área y reconstruye el relevamiento con identificadores nuevos remapeando las referencias internas, preservando estado, marcadores, observaciones, fotos, comentarios y etiquetas, auditando la operación; un archivo incoherente responde `ARCHIVO_EXPORTACION_INVALIDO` sin alterar datos existentes.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-27 | Historia | Exportar un relevamiento de la "Zona Norte" a un único archivo ZIP; descarga desde la web | Archivo único, autocontenido |
| US-28 | Historia | Importar el ZIP exportado: el relevamiento se reconstruye con sus marcadores, fotos, comentarios y etiquetas | Round-trip íntegro |
| US-28 | Historia | Importar un archivo que no es un relevamiento coherente responde `ARCHIVO_EXPORTACION_INVALIDO` y no altera datos | Rechazo seguro |
| US-27/US-28 | Historia | Un jefe de otra área no exporta ni importa (RN-01); toda operación auditada (RN-07) | Acotamiento por área respetado |

## 3. Feedback recibido

- El ciclo exportar → importar cierra el alcance Must de resguardo y traspaso (NB-04): el área central puede recibir el relevamiento como unidad y reconstruirlo en otra instancia.
- Se confirma que la integración con la librería de alojamiento de binarios de fotos (ADR-08) y la sincronización del cliente móvil (CU-07, EP-04) son los próximos focos.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 13 |
| Puntos completados | 13 |
| Velocity efectiva | 13 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 176 verdes (158 unitarias + 18 de integración), +17 respecto del Sprint 05. Cobertura: dominio 89,8 % líneas / 80,4 % branches; aplicación 88,6 % / 79,8 % (gate ≥ 80 % / ≥ 70 % cumplido). Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-27 | Historia | Aceptada |
| US-28 | Historia | Aceptada |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 06 se traslada. |

El manifiesto transporta la referencia y la fuente de cada foto; el binario de la foto vive en la librería de alojamiento configurable (ADR-08), cuya integración queda fuera de este sprint. El filtrado por etiquetas (US-23, CU-08 §5.C), el visor a pantalla completa (US-24) y la sincronización del cliente móvil (EP-04) continúan en el backlog.

## 7. Decisiones tomadas durante el review

- Dar por cerrado el alcance Must de exportación/importación de NB-04.
- Priorizar para los próximos sprints la integración de la librería de alojamiento (ADR-08) y la sincronización (EP-04).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Sprint review del Sprint 06 (exportación/importación del relevamiento completo en ZIP). Veredicto Cumplido, velocity 13, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
