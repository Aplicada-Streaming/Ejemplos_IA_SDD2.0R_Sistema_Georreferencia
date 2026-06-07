# Sprint Review — Sprint 61

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-61_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-06
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita del sprint goal de `plan-iteracion-sprint-61_v1.0.md`:

> Agregar el swipe sobre la foto (avanzar/retroceder con cruce de marcador), conservando los botones ("además de los controles visibles", §6).

Veredicto: Cumplido.

Explicación corta: el carrusel de revisión ahora admite **gesto lateral**. Se agregó al núcleo `NavegadorRevision` (gate) los métodos `Avanzar()`/`Retroceder()` que recorren las fotos y, al terminar las de un marcador, **cruzan al marcador siguiente/anterior** (circular sobre todo el relevamiento). En `RevisionPage`, dos `SwipeGestureRecognizer` (izquierda/derecha) sobre la foto invocan esos métodos; los botones de marcador/foto se conservan. Cierra **H-04** de la auditoría UX. Verificado on-device con un relevamiento sembrado (2 marcadores, el primero con 2 fotos): swipe a la izquierda avanzó dentro del marcador y luego **cruzó a "Marcador 2/2"**; swipe a la derecha **volvió a "Marcador 1/2"**.

## 2. Demos realizadas

| ID | Tipo | Descripción | Feedback |
| --- | --- | --- | --- |
| H-04-SWIPE | Funcionalidad | Swipe izquierda: foto 1→2 (Marcador 1/2) → cruza a Marcador 2/2 (`21`→`22`) | Navegación natural, como una galería |
| H-04-SWIPE | Funcionalidad | Swipe derecha: vuelve a Marcador 1/2 (cruce inverso a la última foto) | Correcto en ambos sentidos |
| BT-NAV-CRUCE | Núcleo | `Avanzar/Retroceder` con cruce de marcador, circular, marcador sin fotos | Bien cubierto en el gate |

## 3. Feedback recibido

- El gesto **complementa** los botones (no los reemplaza): el agente elige; respeta el wireframe §6.
- El cruce de marcador hace que el carrusel se sienta continuo (al terminar un marcador, sigue con el siguiente), como en apps de galería conocidas (Ley de Jakob).
- La lógica de cruce quedó en el núcleo testeable; el gesto es glue delgado.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 5 |
| Puntos completados | 5 |
| Velocity efectiva | 5 |
| Ratio de completitud | 100 % |
| Defectos detectados | 0 |

Pruebas: **489** (445 unitarias + 44 de integración), **+5** del núcleo (`Avanzar`/`Retroceder`: dentro del marcador, cruce a la primera/última, circular, marcador sin fotos). Integración sin cambios (sin backend). Cobertura del gate sin regresión. El MAUI compila (`net10.0-android`, arm64) y se redeployó al moto g42; verificación on-device con datos sembrados por API (2 marcadores, uno con 2 fotos).

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| H-04-SWIPE | Historia | Aceptada (swipe con cruce de marcador) |
| BT-NAV-CRUCE | Tarea | Aceptada (`Avanzar/Retroceder` + tests) |

## 6. Carry-over

| ID | Puntos | Motivo |
| --- | --- | --- |
| — | 0 | Sin traslados. |

Pendiente de la auditoría: el último P1 estructural **H-01/H-03** (captura sobre el mapa, con posición + pines + disparador) — la rearquitectura más grande, candidata a un sprint de alcance pleno. Luego, los P2 (carga al entrar sin "Recargar", disparador central, logout en overflow, contraste de placeholders).

## 7. Decisiones tomadas

- Cruce de marcador **circular** sobre todo el relevamiento (al terminar el último, vuelve al primero), coherente con la navegación de marcadores ya existente.
- Al cruzar hacia atrás, posicionar en la **última** foto del marcador anterior (no la primera): es lo natural al "volver".
- Conservar los botones (accesibilidad y descubribilidad).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-06 | Review del Sprint 61 (H-04: carrusel deslizable). Cumplido, velocity 5, 0 carry-over, 489 pruebas (+5 del núcleo); verificado on-device (swipe con cruce de marcador en ambos sentidos). Generado por AG-07 |
