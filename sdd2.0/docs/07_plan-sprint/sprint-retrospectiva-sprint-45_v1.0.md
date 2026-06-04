# Sprint Retrospectiva — Sprint 45

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-45_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-04
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Se activó la auto-sincronización (F-M-14): el coordinador que ya existía pero estaba inerte ahora toma el relevamiento activo de la sesión y, al recuperar señal, drena comentarios + capturas (S42), cerrando el ciclo de campo sin conexión (S42→S43→S44→S45).
- El cambio fue **aditivo** (proveedor + capturas opcionales), así que los tests de auto-sync de S12 siguieron verdes sin tocarlos; +4 pruebas nuevas cubren el disparo.
- El dispositivo volvió a estar disponible y se aprovechó para **cerrar el pendiente de S44**: se verificó on-device el reingreso en terreno (login → matar proceso → reabrir → "Reingreso en terreno como campo" → entra sin clave).
- Verificar on-device sacó a la luz un endurecimiento necesario de la `LoginPage` (un fallo de `SecureStorage` no debe tumbar el acceso); se corrigió en el sprint.

## 2. Qué no salió bien

- La causa del bug era doble y silenciosa: el coordinador **nunca se instanciaba** (registrado pero no resuelto, así que no se suscribía) **y** nunca recibía el relevamiento. Un servicio "registrado" no es un servicio "activo"; conviene resolver explícitamente los singletons que se suscriben a eventos.
- El **ciclo offline completo en modo avión** (capturar sin señal → recuperar → auto-sync sube todo) no se probó on-device; la automatización de modo avión es frágil y el disparo ya está cubierto en el gate. Sigue pendiente (se reitera de S42).
- El endurecimiento de la `LoginPage` es un arreglo de código de S44 que se coló en S45; está documentado, pero idealmente la verificación on-device de S44 lo habría detectado antes (no se pudo, por la desconexión del dispositivo en su momento).

## 3. Qué probar

- Probar el ciclo offline completo en el dispositivo en modo avión: capturar/comentar sin señal → activar el wifi → confirmar que la cola se vacía sola.
- Considerar un patrón explícito para "servicios de arranque" (los que se suscriben a eventos) que se resuelvan siempre al iniciar, para no repetir el caso del coordinador inerte.
- Mostrar en la UI un indicador de "sincronizando…" / "sincronizado hace X" para que el agente vea que el auto-sync ocurrió.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Probar el ciclo offline completo en modo avión on-device | AG-05 (QA) | 2028-03-19 | Pendiente (se reitera de S42) |
| Indicador de estado de sincronización en la UI | AG-08 (móvil) | 2028-03-19 | Pendiente |
| Patrón de "servicios de arranque" que se resuelvan al iniciar | AG-08 (móvil) | 2028-03-19 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 44 | Estado actual |
| --- | --- |
| Completar el check on-device del reingreso al reconectar el dispositivo | Completada (verificada en este sprint: reingreso en terreno end-to-end) |
| Evaluar biométrico nativo (huella/PIN) como método de seguridad real | Pendiente (se reitera) |
| Auto-sync al recuperar conectividad (F-M-14) | Completada (entregada en este sprint) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-04 | Retrospectiva del Sprint 45 (auto-sincronización): se activó el coordinador inerte (registrado pero no resuelto + sin relevamiento); se cerró el check de reingreso de S44 on-device y se endureció la `LoginPage`. 3 acciones nuevas (modo avión, indicador de sync, servicios de arranque). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
