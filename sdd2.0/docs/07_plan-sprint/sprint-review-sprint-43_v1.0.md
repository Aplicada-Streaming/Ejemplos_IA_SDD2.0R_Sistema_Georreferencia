# Sprint Review — Sprint 43

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-43_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-43_v1.0.md`:

> El agente debe poder ver los relevamientos asignados y elegir cuál trabajar; la elección la usan todas las solapas.

Veredicto: Cumplido.

Explicación corta: se reemplazó el "siempre el primer relevamiento" por una **selección real**. `LectorTokenJwt` lee el usuario (claim `sub`) del token; `SelectorRelevamientos` arma la lista marcando los relevamientos **asignados** al usuario (sus agentes vigentes lo incluyen) y los ordena (asignados primero), y resuelve el **relevamiento activo** (la selección del usuario, o por defecto el primer asignado abierto). `ServicioSesion` recuerda el activo (sirve sin conexión) y expone `ListarRelevamientosAsync`/`RelevamientoActivoAsync`. La `MainPage` suma un `Picker` para elegir; Captura/Revisión/Mapa y la sincronización usan el activo. El núcleo vive en `GeoVial.Sync` (en el gate); la UI se verificó on-device.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-RELEV-SELECT | Funcionalidad | La solapa Sync muestra "Relevamiento activo" con los relevamientos (obra · estado · ✓ asignado) y deja elegir | El agente por fin elige dónde trabajar |
| CU-02 | Funcionalidad | La sincronización y la captura usan el relevamiento elegido, no "el primero" | Coherente entre todas las solapas |
| BT-SELECT-TESTS | Calidad | `LectorTokenJwt` + `SelectorRelevamientos` cubiertos en el gate | La lógica de selección queda protegida |

## 3. Feedback recibido

- Cierra una brecha que hacía la app inusable con más de un relevamiento: antes siempre trabajaba contra el primero del backend.
- Marcar los asignados (✓) y ponerlos primero orienta al agente sin esconder el resto (el backend igual sólo permite capturar en los asignados).
- Leer el `sub` del token en el cliente (sin validar firma) es suficiente para la UI; la autorización real la sigue haciendo el backend.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: **369** (329 unitarias + 40 de integración), +15 unitarias (`LectorTokenJwtTests` 6, `SelectorRelevamientosTests` 6, `ServicioSesionTests` +3): lectura del `sub`, marcado de asignados + orden, resolución del activo (selección/por-defecto/lista vacía/selección inexistente), y listado/activo de `ServicioSesion` con handler falso. El MAUI compila para `net10.0-android`. Gate de cobertura mantenido.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-RELEV-SELECT | Historia | Aceptada (selección de relevamiento + uso del activo en todas las solapas; verificada on-device) |
| BT-SELECT-TESTS | Tarea | Aceptada (núcleo de selección cubierto en el gate, 15 casos) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 43 se traslada. |

Backlog restante (revisión funcional §5/§6): método de seguridad + reingreso offline (S44, RN-06, F-M-02/03), auto-sync al recuperar conectividad (F-M-14), bandeja sin georreferenciar, avisos de conflictos/pendientes, e idempotencia de la captura.

## 7. Decisiones tomadas durante el review

- Mostrar todos los relevamientos accesibles marcando los asignados, en vez de ocultar los no asignados (evita listas vacías para jefes/raíz y orienta al agente).
- Leer el usuario del JWT en el cliente sólo para la UI (marcar asignados); no se usa para autorizar.
- Recordar el relevamiento activo en la sesión para que la captura offline (S42) siga funcionando sin reconsultar el backend.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Sprint review del Sprint 43 (selección de relevamiento asignado). Veredicto Cumplido, velocity 8, 0 carry-over, 369 pruebas (+15 unitarias); verificado on-device. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
