# Plan de Iteración — Sprint 45

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-45_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-02-22
**Fecha fin:** 2028-03-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S42–S44); capacidad sugerida estricta 9 SP. Se compromete la **auto-sincronización al recuperar conectividad** (US-19, F-M-14, 8 SP), brecha de la revisión funcional. Núcleo en `CoordinadorAutoSync` (en el gate); cableado y arranque fuera de CI.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Cerrar la brecha **F-M-14** de la revisión funcional (US-19): la auto-sincronización **no estaba activada** — `CoordinadorAutoSync` existía pero nunca recibía el relevamiento activo y sólo sincronizaba comentarios (no las capturas de S42). Al recuperar señal, la app debe sincronizar **automáticamente** el relevamiento activo (comentarios + capturas encoladas), sin acción del usuario.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-19-AUTOSYNC | Historia | Activar la auto-sincronización al recuperar conectividad: relevamiento activo (de la sesión) + drenar comentarios y capturas | Alta | 6 | Dev móvil (AG-08) | Cerrada |
| BT-AUTOSYNC-TESTS | Tarea | Cubrir en el gate el disparo (proveedor del relevamiento + drenado de capturas) sin romper los tests existentes | Alta | 2 | QA (AG-05) | Cerrada |

Total de puntos comprometidos: 8 SP. Cliente móvil; el núcleo (`CoordinadorAutoSync`) vive en `GeoVial.Sync` (en el gate).

## 4. Alcance técnico

1. **Núcleo (`GeoVial.Sync`, en el gate):** `CoordinadorAutoSync` se extiende de forma **aditiva** (sin romper los tests de S12):
   - Un **proveedor del relevamiento activo** (`Func<Guid?>`) además del `RelevamientoActivo` settable: toma el relevamiento de la sesión, así no hace falta fijarlo a mano.
   - Un `MotorCapturas` opcional: al sincronizar, además de los comentarios, **drena la cola de capturas** (S42).
2. **Cableado (`MauiProgram`):** registra el `CoordinadorAutoSync` con el proveedor `() => ServicioSesion.RelevamientoActivoId` y el `MotorCapturas`.
3. **Arranque (`App.xaml.cs`):** resuelve el `CoordinadorAutoSync` al crear la ventana para que se **suscriba** al monitor de conectividad (antes se registraba pero nunca se instanciaba, por eso no operaba).
4. **Endurecimiento (`LoginPage`):** se atrapan los fallos inesperados (p. ej. `SecureStorage` en un dispositivo sin bloqueo) para que no tumben el acceso; el método de seguridad queda best-effort (hallado al verificar on-device).

## 5. Definition of Done aplicada

- Al recuperar conexión, la app sincroniza automáticamente el relevamiento activo (comentarios + capturas), sin acción del usuario.
- `CoordinadorAutoSync` (proveedor + drenado de capturas) cubierto por pruebas en el gate; los tests de auto-sync de S12 siguen verdes. La suite .NET pasa a **378** y sigue verde.
- El MAUI compila para `net10.0-android` y arranca sin crash con el coordinador suscrito.
- **Verificación on-device (moto g42):** la app arranca con el auto-sync cableado; además se completó el check pendiente de S44 (reingreso en terreno funciona end-to-end).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Cambiar el ctor de `CoordinadorAutoSync` rompe los tests de S12 | Media | Medio | Cambio aditivo (parámetros opcionales); los tests de S12 (ctor de 3 args) siguen pasando |
| El coordinador no se suscribe si nadie lo instancia | Alta | Alto | Se resuelve en `App.CreateWindow` al arrancar; verificado que la app no crashea |
| El ciclo offline completo (modo avión) no se prueba on-device | Media | Bajo | El disparo está cubierto en el gate; la prueba en modo avión queda como acción de retro (se reitera de S42) |

## 7. Criterios de hecho del sprint

El Sprint 45 se considera completo cuando: la auto-sincronización toma el relevamiento activo y drena comentarios + capturas al recuperar señal, el núcleo está cubierto en el gate (sin romper S12), la suite .NET sigue verde (378), el MAUI compila y arranca con el coordinador suscrito, y se facilitan el sprint review y la retrospectiva.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | `revision-funcional-app_v1.0.md` §3 (F-M-14: auto-sync no activada) |
| CU | US-19 (sincronización automática por conectividad), CU-07 |
| Componentes | `GeoVial.Sync` (`CoordinadorAutoSync`); `GeoVial.Mobile` (`MauiProgram`, `App`, `LoginPage`) |
| Calidad | definition-of-done §1.4 |
| Tests previstos | `AutoSyncCapturasTests` (4 casos) + los de S12; verificación de arranque on-device |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-04 | Plan del Sprint 45 (auto-sincronización al recuperar conectividad, US-19/F-M-14): `CoordinadorAutoSync` con proveedor del relevamiento activo + drenado de capturas (en el gate) + cableado y arranque; endurecimiento de `LoginPage`. Suite a 378. Verificado el arranque on-device + reingreso de S44. Compromete 8 SP. Generado por AG-07 |
