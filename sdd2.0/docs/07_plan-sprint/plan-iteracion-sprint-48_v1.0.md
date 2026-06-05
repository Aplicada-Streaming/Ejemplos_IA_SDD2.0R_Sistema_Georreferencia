# Plan de Iteración — Sprint 48

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-48_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-04-03
**Fecha fin:** 2028-04-14
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S45–S47); capacidad sugerida estricta 9 SP. Se compromete el **indicador de estado de sincronización** (8 SP), acción de retro reiterada en S45, S46 y S47. Núcleo (cálculo del estado + monitor) en el gate; el label es glue móvil.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

El agente no tiene forma de saber si su trabajo quedó **a salvo** (sincronizado) o sigue **en la cola local**. Con captura offline (S42) y auto-sync (S45), esa ambigüedad es riesgosa: puede creer que subió cuando no hay señal, o no saber que un intento falló. El objetivo es un **indicador de estado de sincronización** que muestre, de un vistazo: al día / pendientes / sincronizando / sin conexión / error, combinando las dos colas (comentarios S11 + capturas S42) con la conectividad.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-SYNC-IND | Historia | Indicador de estado de sincronización en la app: estado + pendientes + texto para el agente | Alta | 5 | Dev móvil (AG-08) | Cerrada |
| BT-SYNC-NUCLEO | Tarea | Núcleo testeable: `ResumenSincronizacion.Calcular` (puro) + `MonitorSincronizacion` (lee colas + conectividad, notifica) | Alta | 2 | Dev fullstack (AG-09) | Cerrada |
| BT-SYNC-TESTS | Tarea | Cubrir en el gate el cálculo (todos los estados, singular/plural, defensivo) y el monitor (refresco, sincronizando) | Alta | 1 | QA (AG-05) | Cerrada |

Total de puntos comprometidos: 8 SP. Núcleo en `GeoVial.Sync` (en el gate); binding del label en `GeoVial.Mobile`.

## 4. Alcance técnico

1. **Núcleo puro (en el gate):** `ResumenSincronizacion.Calcular(online, sincronizando, pendientes, huboError)` → `(EstadoSync, Pendientes, Texto)`. Prioridad: sincronizando &gt; sin conexión &gt; error (con pendientes) &gt; pendiente &gt; al día. Normaliza conteos negativos. Función pura, sin dependencias → testeable a fondo.
2. **Monitor (en el gate):** `MonitorSincronizacion` toma `IConnectivityMonitor` + `IChangeQueue` (comentarios) + `IColaCapturas` (capturas); `RefrescarAsync(huboError)` relee ambos conteos + conectividad y recalcula; `MarcarSincronizando()` marca el estado en curso conservando el conteo conocido; expone `Actual` y un evento `Cambiado`.
3. **Cableado móvil (`MauiProgram`):** registra `MonitorSincronizacion` (singleton) sobre los servicios ya existentes.
4. **Binding (`MainPage`):** un `Label` nuevo (`SyncEstadoLbl`) refleja `Cambiado`; se refresca al aparecer la pantalla, después de capturar (suma un pendiente) y alrededor de la sincronización (marca "sincronizando" antes, refresca con/ sin error después).

## 5. Definition of Done aplicada

- El indicador muestra correctamente los cinco estados según conectividad + pendientes + sync en curso + último error.
- Núcleo (cálculo + monitor) cubierto por pruebas en el gate; nada existente se rompe.
- Cobertura DoD respetada (Domain/Application líneas ≥80 %, ramas ≥70 %); el núcleo nuevo (`GeoVial.Sync`) queda bien cubierto.
- El MAUI compila para `net10.0-android` con el indicador cableado.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El evento `Cambiado` actualiza la UI fuera del hilo principal | Media | Medio | El handler móvil hace `MainThread.BeginInvokeOnMainThread`; el núcleo sólo notifica |
| El indicador queda "viejo" tras un auto-sync en background | Media | Bajo | Se refresca al aparecer la pantalla y alrededor de la sync manual; el auto-sync silencioso se reflejará al volver a la pantalla (mejora futura: suscribir al fin del auto-sync) |
| Mezclar dos conteos (comentarios + capturas) confunde el total | Baja | Bajo | El total es la suma; el texto habla de "pendientes" en general, no por tipo (suficiente para el agente) |

## 7. Criterios de hecho del sprint

El Sprint 48 se considera completo cuando: el indicador refleja los cinco estados, el núcleo está cubierto en el gate, la cobertura DoD se mantiene, el MAUI compila con el indicador, y se facilitan el sprint review y la retrospectiva.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Acción de retro reiterada (S45 §3, S46 §4, S47 §4): indicador de estado de sincronización en la UI |
| CU | CU-07 (sincronización); apoya US-16 (captura offline) y US-19 (auto-sync) |
| Componentes | `GeoVial.Sync` (`ResumenSincronizacion`, `MonitorSincronizacion`); `GeoVial.Mobile` (`MauiProgram`, `MainPage`) |
| Calidad | definition-of-done §1.4 (cobertura) |
| Tests previstos | `ResumenSincronizacionTests` (estados, singular/plural, defensivo) + `MonitorSincronizacionTests` (refresco suma colas, sin conexión, error, sincronizando) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Plan del Sprint 48 (indicador de estado de sincronización): núcleo `ResumenSincronizacion.Calcular` (puro) + `MonitorSincronizacion` en el gate + binding en `MainPage`. Acción de retro reiterada S45-S47. Compromete 8 SP. Generado por AG-07 |
