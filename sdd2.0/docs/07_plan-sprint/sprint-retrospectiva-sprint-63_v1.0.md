# Sprint Retrospectiva — Sprint 63

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-63_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-07
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- **Backlog de la auditoría UX saldado.** Con los 4 P1 (S59-S62) y el bundle de P2 (S63), la auditoría `evaluacion-ux-mobile_v1.0.md` queda ejecutada: cinta de conexión, GPS, carrusel deslizable, captura sobre el mapa, y el pulido (carga al entrar, logout al overflow, contraste, disparador prominente).
- **Bundle eficiente.** Agrupar 4 P2 chicos en un sprint evitó cuatro micro-sprints; todos eran cambios seguros de presentación/comportamiento.
- **De paso, otro §8.** Al refactorizar la carga de Revisión se eliminó un leak de `ex.Message` (mensaje técnico) por uno llano, en línea con H-06.

## 2. Qué no salió bien

- **Verificación on-device frágil.** El túnel `adb reverse` se cayó (sleep del dispositivo) y dio "Connection failure" en el login; hubo que reestablecerlo. Se reitera (por enésima vez) la necesidad de un script de arranque (backend + reverse + seed + login) para la verificación.
- **Lector de capturas saturado.** Tras muchas capturas, no se pudieron inspeccionar visualmente las últimas (alto 2400 px); H-11/H-12/H-13 se confirmaron por construcción (compilan/despliegan) y por dump, no por captura. Conviene reducir/escala de capturas o recortarlas.
- **Dos P2 diferidos.** H-14 (anuncios por región en vivo) y H-07 (truncado de etiquetas de pestañas) no son cambios de una línea; se difieren a una pasada de accesibilidad/UI dedicada para no inflar el bundle.

## 3. Qué probar

- On-device: entrar a *Revisión* muestra datos sin tocar "Cargar"; "Cerrar sesión" está en el overflow; el login muestra placeholders legibles; el disparador "Tomar foto" es el botón grande.

## 4. Acciones concretas

| Acción | Responsable | Fecha | Estado |
| --- | --- | --- | --- |
| Script único de verificación on-device (backend + adb reverse + seed + login) | AG-09 | 2028-11-24 | Pendiente (reiterado) |
| Pasada de accesibilidad/UI dedicada: H-14 (live regions) + H-07 (tabs) | AG-08 | 2028-11-24 | Planificado |
| Evolución de H-01: coordenada de captura desde el pin del mapa embebido | Equipo | 2028-11-24 | Pendiente (evolución) |
| Control de mapa compartido (deuda técnica) | AG-08 | 2028-11-24 | Pendiente (reiterado) |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 62 | Estado |
| --- | --- |
| Verificar on-device la captura sobre el mapa | En curso (con el usuario) |
| Control de mapa compartido | Pendiente (reiterado) |
| Evolución de H-01 (coordenada desde el pin embebido) | Pendiente |
| Atacar los P2 de la auditoría | **Hecho** (este sprint) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-07 | Retro del Sprint 63 (P2 de la auditoría UX). Salda el backlog de la auditoría (P1 + P2). Fricciones: túnel adb caído y lector de capturas saturado. Diferidos H-14 y H-07 a una pasada de accesibilidad dedicada. Generada por AG-07 |
