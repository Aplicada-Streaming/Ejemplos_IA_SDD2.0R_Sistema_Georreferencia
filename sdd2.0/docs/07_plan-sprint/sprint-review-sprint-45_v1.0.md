# Sprint Review — Sprint 45

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-45_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-04
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-45_v1.0.md`:

> Al recuperar señal, la app debe sincronizar automáticamente el relevamiento activo (comentarios + capturas encoladas), sin acción del usuario.

Veredicto: Cumplido.

Explicación corta: `CoordinadorAutoSync` existía pero estaba **inerte** (nunca recibía el relevamiento activo, sólo sincronizaba comentarios, y nadie lo instanciaba para que se suscribiera a la conectividad). Se lo extendió de forma aditiva: toma el relevamiento activo de la sesión (proveedor `Func<Guid?>`) y, al sincronizar, **también drena la cola de capturas** (S42). Se lo cableó en `MauiProgram` con el proveedor `() => ServicioSesion.RelevamientoActivoId` + `MotorCapturas`, y se lo **resuelve al arrancar** (`App.CreateWindow`) para que se suscriba al monitor de conectividad. Así, al recuperar señal, la app sincroniza sola.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-19-AUTOSYNC | Funcionalidad | Al recuperar conexión, la app sincroniza el relevamiento activo (comentarios + capturas) sin tocar nada | El agente no tiene que acordarse de sincronizar |
| F-M-14 | Robustez | El coordinador toma el relevamiento de la sesión y drena la cola de capturas de S42 | Coherente con la selección y la captura offline |
| RN-06 (cierre) | Funcionalidad | Bonus: se verificó on-device el reingreso en terreno de S44 (pendiente del sprint anterior) | El reingreso sin clave funciona |

## 3. Feedback recibido

- Cierra la última pieza del ciclo de campo sin conexión: capturar offline (S42) + elegir relevamiento (S43) + reingresar en terreno (S44) + **sincronizar solo al volver la señal (S45)**.
- El coordinador ya era testeable; faltaba activarlo. El cambio aditivo (proveedor + capturas opcionales) mantuvo verdes los tests de auto-sync de S12.
- Al verificar on-device se halló y corrigió un endurecimiento de la `LoginPage` (un fallo de `SecureStorage` no debe tumbar el acceso).

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 1 menor (LoginPage no atrapaba fallos de SecureStorage) — corregido en el sprint |

Pruebas: **378** (338 unitarias + 40 de integración), +4 unitarias (`AutoSyncCapturasTests`): usa el relevamiento del proveedor, drena las capturas al sincronizar, sin relevamiento no hace nada, sin conexión no sincroniza. Los tests de auto-sync de S12 siguen verdes. El MAUI compila para `net10.0-android` y arranca con el coordinador suscrito. El gate de cobertura se mantiene.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-19-AUTOSYNC | Historia | Aceptada (auto-sync activado: relevamiento activo + comentarios + capturas; arranque verificado on-device) |
| BT-AUTOSYNC-TESTS | Tarea | Aceptada (4 casos en el gate; sin romper S12) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 45 se traslada. |

Pendiente menor (no compromiso): la **prueba del ciclo offline completo en modo avión** on-device (capturar sin señal → recuperar → auto-sync sube todo) sigue como acción de retro. Backlog restante (revisión funcional §5/§6): bandeja sin georreferenciar, avisos de conflictos/pendientes, idempotencia de la captura, biométrico nativo, endpoint "asignados a mí".

## 7. Decisiones tomadas durante el review

- Extender `CoordinadorAutoSync` de forma aditiva (proveedor + capturas opcionales) para no romper los tests existentes.
- Tomar el relevamiento activo de la sesión (proveedor) en vez de fijarlo a mano, así el auto-sync usa siempre la selección vigente (S43).
- Resolver el coordinador al arrancar la app para que se suscriba a la conectividad.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-04 | Sprint review del Sprint 45 (auto-sincronización al recuperar conectividad). Veredicto Cumplido, velocity 8, 0 carry-over, 378 pruebas (+4 unitarias); arranque y reingreso (S44) verificados on-device. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
