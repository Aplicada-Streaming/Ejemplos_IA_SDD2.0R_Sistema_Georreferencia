# Plan de Iteración — Sprint 49

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-49_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-04-17
**Fecha fin:** 2028-04-28
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S46–S48); capacidad sugerida estricta 9 SP. Se compromete la **auto-sincronización observable** (8 SP): un evento de fin de auto-sync que actualiza el indicador en vivo (cierra la salvedad de S48) y avisa de conflictos en la app (ítem del backlog de la revisión funcional). Núcleo (evento + payload) en el gate.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

El indicador de S48 sólo se refrescaba en eventos de UI (aparecer la pantalla, capturar, sync manual): un **auto-sync en background** (al recuperar señal, S45) no se reflejaba hasta volver a tocar la pantalla. Además, los **conflictos** detectados al sincronizar sólo se logueaban, sin avisar al agente (gap de la revisión funcional). Ambos se resuelven con un mismo cambio: que `CoordinadorAutoSync` **emita un evento al terminar** con un resumen del resultado, y que la app reaccione (refrescar el indicador en vivo + avisar de conflictos).

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-AUTOSYNC-OBS | Historia | El indicador se actualiza en vivo al terminar el auto-sync; la app avisa de conflictos detectados | Alta | 5 | Dev móvil (AG-08) | Cerrada |
| BT-AUTOSYNC-EVT | Tarea | `CoordinadorAutoSync.SincronizacionCompletada` + payload `ResultadoAutoSync` (confirmados, conflictos, capturas subidas, error) | Alta | 2 | Dev fullstack (AG-09) | Cerrada |
| BT-AUTOSYNC-EVT-TESTS | Tarea | Cubrir en el gate el evento (éxito con conteos + capturas; error que relanza; sin relevamiento no emite) | Alta | 1 | QA (AG-05) | Cerrada |

Total de puntos comprometidos: 8 SP. Núcleo en `GeoVial.Sync` (en el gate); cableado y aviso en `GeoVial.Mobile`.

## 4. Alcance técnico

1. **Núcleo (en el gate):** `CoordinadorAutoSync` gana un evento `SincronizacionCompletada` (payload `ResultadoAutoSync(Confirmados, Conflictos, CapturasSubidas, HuboError)`), que se dispara al terminar `SincronizarSiCorrespondeAsync` — con éxito (conteos del motor + capturas subidas del drenado S42) o con error (relanza, marcando `HuboError`). Cambio **aditivo**: no toca el ctor ni el tipo de retorno; los tests de S12/S45 siguen verdes.
2. **Cableado móvil (`MainPage`):** se suscribe al evento mientras la pantalla está visible (suscripción en `OnAppearing`, baja en `OnDisappearing`, para evitar fugas/duplicados al navegar entre solapas). Al recibirlo: refresca el indicador (S48) en vivo y muestra/oculta un **aviso de conflictos**. La suscripción del indicador de S48 también se migra a este patrón acotado.
3. **Aviso de conflictos:** un label visible sólo cuando hay conflictos; también se setea tras la sincronización manual (DRY con un helper).

## 5. Definition of Done aplicada

- Al terminar un auto-sync (al recuperar señal), el indicador se actualiza **sin tocar la pantalla** y, si hubo conflictos, la app los avisa.
- Núcleo (evento + payload) cubierto por pruebas en el gate; los tests de auto-sync previos (S12/S45) siguen verdes (cambio aditivo).
- Cobertura DoD respetada (Domain/Application líneas ≥80 %, ramas ≥70 %); el núcleo (`GeoVial.Sync`) queda bien cubierto.
- El MAUI compila para `net10.0-android` con el cableado.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Suscribir una página transitoria a un singleton fuga la página | Media | Medio | Suscripción acotada a `OnAppearing`/`OnDisappearing` (se migra también la de S48) |
| Cambiar `SincronizarSiCorrespondeAsync` rompe los tests de S12/S45 | Media | Medio | Cambio aditivo: mismo ctor y retorno; el evento sin suscriptores no afecta; suite previa verde |
| El evento corre fuera del hilo de UI | Media | Medio | El handler móvil usa `MainThread.BeginInvokeOnMainThread`; el núcleo sólo notifica |

## 7. Criterios de hecho del sprint

El Sprint 49 se considera completo cuando: el auto-sync emite el evento de fin con su resumen, el indicador se actualiza en vivo y la app avisa de conflictos, el núcleo está cubierto en el gate sin romper lo previo, la cobertura DoD se mantiene, el MAUI compila, y se facilitan el sprint review y la retrospectiva.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Acción de retro S48 (auto-refresco al terminar el auto-sync) + backlog de la revisión funcional (avisos de conflictos en la UI) |
| CU | CU-07 (sincronización), CU-12 (conflictos); apoya US-19 (auto-sync) y el indicador (S48) |
| Componentes | `GeoVial.Sync` (`CoordinadorAutoSync`, `ResultadoAutoSync`); `GeoVial.Mobile` (`MainPage`) |
| Calidad | definition-of-done §1.4 (cobertura) |
| Tests previstos | `AutoSyncCapturasTests` (+3): evento al éxito con conteos + capturas; error que relanza con `HuboError`; sin relevamiento no emite |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Plan del Sprint 49 (auto-sincronización observable): evento `SincronizacionCompletada` + payload en el gate; indicador en vivo + aviso de conflictos en `MainPage`. Cierra la salvedad de S48 y un gap de la revisión funcional. Compromete 8 SP. Generado por AG-07 |
