# Sprint Review — Sprint 49

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-49_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-49_v1.0.md`:

> Ambos se resuelven con un mismo cambio: que `CoordinadorAutoSync` emita un evento al terminar con un resumen del resultado, y que la app reaccione (refrescar el indicador en vivo + avisar de conflictos).

Veredicto: Cumplido.

Explicación corta: el indicador de S48 sólo se refrescaba en eventos de UI; un auto-sync en background no se reflejaba hasta volver a tocar la pantalla, y los conflictos sólo se logueaban. Se agregó el evento `CoordinadorAutoSync.SincronizacionCompletada` (payload `ResultadoAutoSync`: confirmados, conflictos, capturas subidas, error), disparado al terminar la sincronización con éxito o con error. `MainPage` se suscribe mientras está visible: refresca el indicador en vivo y muestra un aviso cuando hay conflictos. El cambio es aditivo (no toca el ctor ni el retorno de `SincronizarSiCorrespondeAsync`).

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-AUTOSYNC-OBS | Funcionalidad | Al recuperar señal, el auto-sync corre y el indicador pasa solo a "Todo sincronizado" sin tocar la pantalla | El agente ve en vivo que su trabajo se subió |
| Conflictos | Aviso | Si la sync detecta conflictos, aparece "⚠ N conflictos detectados — revisalos en la web/revisión" | El agente se entera de que algo necesita atención |

## 3. Feedback recibido

- Cierra la salvedad que el propio equipo anotó en la retro de S48 (el indicador no se actualizaba solo tras un auto-sync): ahora es realmente en vivo.
- De paso salda un ítem del backlog de la revisión funcional (avisos de conflictos en la UI), reutilizando el mismo evento — buena economía de diseño.
- Se aprovechó para **acotar las suscripciones** a la vida visible de la página (`OnAppearing`/`OnDisappearing`), corrigiendo el patrón de fuga que arrastraba la suscripción del indicador de S48.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: **403** (360 unitarias + 43 de integración), +3 unitarias en `AutoSyncCapturasTests`: el evento al éxito lleva los conteos (confirmados/conflictos) y las capturas subidas; al fallar marca `HuboError` y relanza; sin relevamiento no emite. Los tests de auto-sync previos (S12/S45) siguen verdes (cambio aditivo). Cobertura del gate: Domain 88,5 % / 79,8 %; Application 87,3 % / 76,2 % (umbral 80 % / 70 %); `GeoVial.Sync` a 94,7 % / 88,8 %. El MAUI compila para `net10.0-android`.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-AUTOSYNC-OBS | Historia | Aceptada (indicador en vivo + aviso de conflictos) |
| BT-AUTOSYNC-EVT | Tarea | Aceptada (evento + payload en el gate) |
| BT-AUTOSYNC-EVT-TESTS | Tarea | Aceptada (3 casos) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 49 se traslada. |

Pendiente menor (no compromiso): verificación on-device del refresco en vivo y del aviso (modo avión → reconectar). Backlog restante (revisión funcional §5/§6): bandeja sin georreferenciar navegable, biométrico nativo, y el sprint de limpieza del listado de área (retro S47).

## 7. Decisiones tomadas durante el review

- Un único evento (`SincronizacionCompletada`) sirve para dos objetivos (refresco en vivo + aviso de conflictos): economía de diseño.
- Cambio **aditivo** al coordinador (sin tocar ctor ni retorno) para no romper S12/S45.
- Suscripciones acotadas a la vida visible de la página, corrigiendo el patrón de fuga de S48.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Sprint review del Sprint 49 (auto-sincronización observable). Veredicto Cumplido, velocity 8, 0 carry-over, 403 pruebas (+3); cobertura del gate mantenida; `GeoVial.Sync` a 94,7/88,8. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
