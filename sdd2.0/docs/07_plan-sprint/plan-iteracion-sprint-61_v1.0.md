# Plan de Iteración — Sprint 61

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-61_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-10-02
**Fecha fin:** 2028-10-13
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas. Equipo: 4 (`equipo_n: 4`). Estimación en story points.
- Capacidad: promedio móvil 6,0 SP (S58–S60). Se compromete una **mejora de UX acotada (5 SP)** que reusa el núcleo de navegación del carrusel (S26); sin backend.

## 2. Objetivo del sprint

Cerrar el hallazgo **H-04** de la auditoría UX móvil: el carrusel de revisión se navega **sólo con botones**, mientras que wireframes-marcador-carrusel §6 pide un **gesto lateral** que, al terminar las fotos de un marcador, **cruce al marcador siguiente/anterior**. El objetivo es agregar el **swipe** sobre la foto (avanzar/retroceder con cruce de marcador), **conservando los botones** ("además de los controles visibles", §6).

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| H-04-SWIPE | Historia | Carrusel deslizable: swipe lateral sobre la foto, con cruce de marcador al terminar | Alta | 3 | Dev móvil (AG-08) | Cerrada |
| BT-NAV-CRUCE | Tarea | Núcleo `NavegadorRevision.Avanzar/Retroceder` (cruce de marcador) + tests | Media | 2 | Dev móvil (AG-08) | Cerrada |

Total: 5 SP (UX; reusa el `NavegadorRevision` del gate, sin backend).

## 4. Alcance técnico

- **Núcleo (gate, `GeoVial.Revision`):** `NavegadorRevision.Avanzar()` y `Retroceder()`. `Avanzar` pasa a la foto siguiente y, si era la última (o el marcador no tiene fotos), **cruza al marcador siguiente** (su primera foto). `Retroceder` va a la anterior y, si era la primera, **cruza al marcador anterior** posicionándose en su **última** foto. Circular sobre todo el relevamiento. Complementan (no reemplazan) `SiguienteFoto/AnteriorFoto/SiguienteMarcador/AnteriorMarcador`.
- **UI (`RevisionPage`):** dos `SwipeGestureRecognizer` (Left/Right) sobre la foto (`FotoImg`); `OnSwipeFoto` llama `Avanzar`/`Retroceder` y `RenderAsync`. Los botones de marcador/foto **se conservan**.
- **Sin cambios de backend/Application/Domain.**

## 5. Definition of Done aplicada

- En la revisión, deslizar la foto a la izquierda avanza (y cruza al marcador siguiente al terminar las fotos); a la derecha retrocede (y cruza al marcador anterior, a su última foto). Los botones siguen funcionando.
- Suite del gate verde con el núcleo de cruce cubierto; cobertura DoD sin regresión.
- El MAUI compila y se verifica on-device (swipe que cruza de marcador en ambos sentidos).

## 6. Riesgos y mitigaciones

| Riesgo | Prob. | Impacto | Mitigación |
| --- | --- | --- | --- |
| El swipe horizontal choca con el scroll vertical | Media | Bajo | `SwipeGestureRecognizer` Left/Right sobre la imagen captura el gesto lateral; el scroll es vertical |
| Cruce de marcador con marcador sin fotos | Media | Bajo | `Avanzar/Retroceder` contemplan el marcador sin fotos (cruzan directo); cubierto por test |
| Índice de foto fuera de rango al cruzar | Baja | Medio | El núcleo fija el índice a la última foto del marcador destino; cubierto por test |

## 7. Criterios de hecho del sprint

Completo cuando: el carrusel admite swipe lateral con cruce de marcador en ambos sentidos (y conserva los botones); la suite del gate verde con el núcleo cubierto; el MAUI compila y se verifica on-device; y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Auditoría UX móvil `evaluacion-ux-mobile_v1.0.md`, hallazgo **H-04** (P1) |
| CU/UX | wireframes-marcador-carrusel §4-§6 (carrusel deslizable, cruce de marcador), CU-09 |
| Componentes | `GeoVial.Revision` (`NavegadorRevision`); `GeoVial.Mobile` (`RevisionPage`) |
| Calidad | definition-of-done §1.4; gate Domain/Application líneas ≥80 % / ramas ≥70 % |
| Tests | `NavegadorRevisionTests` (avanzar dentro/cruce, retroceder cruce a la última, circular, marcador sin fotos) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-06 | Plan del Sprint 61 (H-04: carrusel deslizable). Núcleo `NavegadorRevision.Avanzar/Retroceder` (cruce de marcador, gate) + `SwipeGestureRecognizer` en `RevisionPage`, conservando los botones. Sin backend. 5 SP. Generado por AG-07 |
