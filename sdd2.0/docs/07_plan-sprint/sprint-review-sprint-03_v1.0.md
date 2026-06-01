# Sprint Review — Sprint 03

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-03_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-03_v1.0.md`:

> Entregar la captura georreferenciada de observaciones, de modo que una observación tome su coordenada de los metadatos de la foto como fuente primaria, se agrupe en un marcador del relevamiento cuando cae dentro del radio configurado y, si la foto no trae metadatos, se ubique manualmente o quede en la bandeja sin georreferenciar, todo sobre relevamientos en recolección, verificado por rol y área y auditado.

Veredicto: Cumplido.

Explicación corta: la captura resuelve la coordenada por prioridad de metadatos (RN-03), agrupa por radio (RN-02) y deriva a la bandeja sin georreferenciar cuando falta la coordenada; la ubicación manual la rescata. El cálculo de distancia (haversine) está encapsulado y probado.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-11 | Historia | Captura con metadatos: crea marcador y georreferencia la observación | Coordenada confiable desde la foto |
| US-12 | Historia | Segunda captura dentro del radio: reutiliza el marcador existente | Agrupación por radio correcta |
| US-13 | Historia | Captura sin metadatos: bandeja sin georreferenciar y ubicación manual posterior | Recuperación clara del punto |
| US-14 | Historia | Carga priorizando metadatos: rechazo de ubicación manual sobre foto con metadatos (FUENTE_UBICACION_INCORRECTA) | Prioridad de fuentes respetada |

## 3. Feedback recibido

- Persiste la limitación de demo por UI con roles distintos del raíz (falta provisión de credenciales para un agente); se confirma la prioridad de esa tarea para el próximo sprint.
- Se sugiere, para la revisión sobre mapa (EP-05), mostrar la bandeja sin georreferenciar como una vista propia del relevamiento.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 28 |
| Puntos completados | 28 |
| Velocity efectiva | 28 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 107 verdes (97 unitarias + 10 de integración), +16 respecto del Sprint 02. Cobertura: dominio 90,7 % líneas / 78,9 % branches; aplicación 80,5 % / 71,7 % (gate ≥ 80 % / ≥ 70 % cumplido). Build Release sin warnings tratados como error. Se agregó una prueba de integración del flujo de captura contra EF Core (marcador + observación + foto).

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-11 | Historia | Aceptada |
| US-12 | Historia | Aceptada |
| US-13 | Historia | Aceptada |
| US-14 | Historia | Aceptada |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 03 se traslada. |

BT-05 y BT-06 quedaron materializados como el backend de este slice. BT-07 avanzó con las entidades Marcador, Observacion y Foto mapeadas y migradas; la gestión de fotos/comentarios/etiquetas del marcador (US-15) y la integración con Testcontainers quedan para la épica de revisión.

## 7. Decisiones tomadas durante el review

- Priorizar en el backlog la provisión de credenciales para habilitar las demos por UI de agentes y jefes de área.
- Tratar la bandeja sin georreferenciar como una vista del relevamiento al construir la revisión sobre mapa (EP-05).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Sprint review del Sprint 03 (captura y georreferenciación). Veredicto Cumplido, velocity 28, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
