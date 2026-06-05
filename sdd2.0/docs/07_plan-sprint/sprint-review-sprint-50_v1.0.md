# Sprint Review — Sprint 50

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-50_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-50_v1.0.md`:

> El objetivo es enriquecer la bandeja en la revisión (momento + referencia de foto) y mostrarla en una solapa de la app, ordenada y legible. La colocación efectiva en el mapa desde la app se planifica aparte.

Veredicto: Cumplido.

Explicación corta: las fotos sin GPS van a la bandeja sin georreferenciar (RN-03) a la espera de ubicación manual (CU-05), pero la app no las mostraba y el backend sólo exponía los IDs. Se enriqueció la revisión con un campo `Bandeja` (id + momento + referencia de foto), de forma **aditiva** (se conserva `ObservacionesSinGeorreferenciar`), y se agregó una solapa "Bandeja" en la app que la presenta ordenada (más recientes primero) y legible, vía un `PresentadorBandeja` en el gate. La colocación en el mapa desde la app queda explícitamente para el próximo sprint.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-BANDEJA-VER | Funcionalidad | La solapa "Bandeja" lista las observaciones sin ubicar del relevamiento activo, con momento y nombre de foto | El agente por fin ve qué quedó sin ubicar |
| BT-BANDEJA-DTO | Robustez | La revisión expone la bandeja enriquecida (E2E: poblada antes de ubicar, vacía después) | Coherente con el flujo de ubicación manual |

## 3. Feedback recibido

- Cierra (la parte de visibilidad de) un gap concreto de la revisión funcional: las observaciones sin GPS eran "invisibles" en la app.
- Diseño **aditivo**: el campo `Bandeja` es opcional y se conservó `ObservacionesSinGeorreferenciar`, así no se rompió ningún consumidor (web/tests).
- Honesto sobre el alcance: por ahora el agente **ve** la bandeja; la **ubicación en el mapa desde la app** es el sprint siguiente (hoy se resuelve en la web/revisión, y la UI lo aclara).

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 1 menor (uso de `ViewCell` obsoleto rompía el build MAUI con warnings-as-errors) — corregido en el sprint |

Pruebas: **408** (365 unitarias + 43 de integración), +5 unitarias (4 de `PresentadorBandejaTests`: orden por momento, etiqueta con/sin foto, vacía; 1 de `RevisionTests`: la revisión enriquece la bandeja) y se reforzó el E2E `Ubicacion_manual_completo` (bandeja poblada con foto antes de ubicar, vacía después). Cobertura del gate: Domain 88,5 % / 79,8 %; Application 87,4 % / 76,4 % (umbral 80 % / 70 %); `GeoVial.Revision` a 97,2 % / 93,5 %. El MAUI compila para `net10.0-android`.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-BANDEJA-VER | Historia | Aceptada (solapa "Bandeja" muestra las observaciones sin ubicar) |
| BT-BANDEJA-DTO | Tarea | Aceptada (revisión enriquecida, aditiva) |
| BT-BANDEJA-NUCLEO | Tarea | Aceptada (`PresentadorBandeja` cubierto en el gate) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 50 se traslada. |

Pendiente planificado (no carry-over): **ubicar observaciones de la bandeja desde la app** (tap en el mapa → coordenada → CU-05), candidato a S51. Backlog restante (revisión funcional §5/§6): biométrico nativo; sprint de limpieza del listado de área (retro S47).

## 7. Decisiones tomadas durante el review

- Enriquecer la bandeja de forma **aditiva** (campo nuevo opcional + conservar el de IDs) para no romper consumidores.
- Separar **ver** (este sprint) de **ubicar en el mapa** (sprint siguiente), para entregar valor incremental y no mezclar la interacción WebView compleja.
- Núcleo de presentación (`PresentadorBandeja`) en `GeoVial.Revision` para máxima cobertura en el gate, al estilo de `NavegadorRevision`.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Sprint review del Sprint 50 (bandeja sin georreferenciar visible y navegable). Veredicto Cumplido, velocity 8, 0 carry-over, 408 pruebas (+5); cobertura del gate mantenida, `GeoVial.Revision` a 97,2/93,5. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
