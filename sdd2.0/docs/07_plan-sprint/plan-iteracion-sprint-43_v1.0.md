# Plan de Iteración — Sprint 43

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-43_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-01-25
**Fecha fin:** 2028-02-06
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S40–S42); capacidad sugerida estricta 9 SP. Se compromete la **selección de relevamiento asignado** del cliente móvil (8 SP), brecha de la revisión funcional. Núcleo de selección en el gate; UI fuera de CI, verificada on-device.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Cerrar la brecha **F-M-04/F-M-05** de la revisión funcional (`revision-funcional-app_v1.0.md` §3/§5): la app **siempre usaba "el primer relevamiento"** (`PrimerRelevamientoAsync`), sin listar ni dejar elegir. El agente debe poder **ver los relevamientos asignados** y **elegir cuál trabajar**; la elección la usan todas las solapas (Sync/Captura/Revisión/Mapa).

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-RELEV-SELECT | Historia | Listar relevamientos accesibles marcando los asignados + elegir el activo; las páginas usan el activo | Alta | 6 | Dev móvil (AG-08) | Cerrada |
| BT-SELECT-TESTS | Tarea | Núcleo testeable: `LectorTokenJwt` (usuario del token) + `SelectorRelevamientos` (marcar/ordenar/resolver activo) con pruebas | Alta | 2 | QA (AG-05) | Cerrada |

Total de puntos comprometidos: 8 SP. Cliente móvil; el núcleo de selección vive en `GeoVial.Sync` (en el gate).

## 4. Alcance técnico

1. **Núcleo (`GeoVial.Sync`, en el gate):**
   - `LectorTokenJwt`: lee el claim `sub` (usuario) del JWT (sin validar la firma; la valida el backend) para poder marcar los relevamientos asignados al usuario.
   - `SelectorRelevamientos` + `RelevamientoDatos`/`RelevamientoResumen`: `Listar` marca como asignado el relevamiento cuyos agentes vigentes incluyen al usuario y ordena (asignados primero, luego por obra); `Activo` resuelve el relevamiento activo (la selección del usuario si sigue disponible; si no, el primer asignado abierto; si no, el primero).
   - `ServicioSesion`: guarda el token, expone `UsuarioId`, `RelevamientoActivoId` + `SeleccionarRelevamiento`, `ListarRelevamientosAsync` y `RelevamientoActivoAsync` (recuerda el activo para usarlo sin conexión). Reemplaza `PrimerRelevamientoAsync`.
2. **UI (`MainPage`, fuera de CI):** un `Picker` "Relevamiento activo" lista los relevamientos (obra · estado · ✓ asignado), deja elegir y fija el activo. Las solapas Captura/Revisión/Mapa y la sincronización usan `RelevamientoActivoAsync()` en vez del "primero".

## 5. Definition of Done aplicada

- El agente ve la lista de relevamientos con los asignados marcados y primero; elegir uno lo fija como activo; las solapas usan el activo.
- `LectorTokenJwt` y `SelectorRelevamientos` cubiertos por pruebas en el gate; `ServicioSesion` con pruebas de listado/activo (fake handler). La suite .NET pasa a **369** y sigue verde.
- El MAUI compila para `net10.0-android`.
- **Verificación on-device (moto g42):** login como agente → la solapa Sync muestra el selector de relevamiento poblado y un activo; la sincronización usa el activo elegido.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El backend lista relevamientos del área, no sólo los asignados | Media | Bajo | Se marcan los asignados (✓) y van primero; el agente igual sólo puede capturar en los asignados (lo enforcea el backend) |
| Sin conexión no se puede listar | Media | Bajo | El activo elegido se recuerda y sirve sin conexión; el listado requiere conexión una vez |
| Leer el JWT sin validar firma | Baja | Bajo | Sólo se usa para marcar asignados en la UI; toda autorización la hace el backend |

## 7. Criterios de hecho del sprint

El Sprint 43 se considera completo cuando: el agente puede ver y elegir el relevamiento (los asignados marcados/primeros) y la elección la usan todas las solapas, el núcleo de selección está cubierto en el gate, la suite .NET sigue verde (369), el MAUI compila para android, se verifica on-device, y se facilitan el sprint review y la retrospectiva.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | `revision-funcional-app_v1.0.md` §3/§5 (F-M-04/05) |
| CU | CU-02 (iniciar sesión y seleccionar relevamiento asignado), RN-01 |
| Componentes | `GeoVial.Sync` (`LectorTokenJwt`, `SelectorRelevamientos`, `ServicioSesion`); `GeoVial.Mobile` (`MainPage` + las solapas) |
| Calidad | definition-of-done §1.4 |
| Tests previstos | `LectorTokenJwtTests` + `SelectorRelevamientosTests` + `ServicioSesionTests` (15 casos); verificación on-device |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Plan del Sprint 43 (selección de relevamiento asignado, F-M-04/05): `LectorTokenJwt` + `SelectorRelevamientos` (núcleo en el gate) + Picker en `MainPage` y uso del relevamiento activo en todas las solapas. Suite a 369. Verificado on-device. Compromete 8 SP. Generado por AG-07 |
