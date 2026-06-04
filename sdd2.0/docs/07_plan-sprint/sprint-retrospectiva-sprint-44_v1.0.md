# Sprint Retrospectiva — Sprint 44

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-44_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-04
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Se cerró RN-06 (F-M-02/03): el login configura el método de seguridad + habilita el modo offline + recuerda el usuario, y el reingreso en terreno re-autentica sin reescribir la clave.
- El núcleo de auth quedó en `ServicioSesion` (en el gate), reusando el asentado de token del login; 5 pruebas cubren configurar/offline/reingreso (incluido el rechazo sin método).
- El flujo completo se verificó **de punta a punta contra el backend real** por API (409 sin método → configurar → offline → reingreso 200 → reingreso 409 sin método), lo que da confianza aun sin el dispositivo.

## 2. Qué no salió bien

- El **dispositivo se desconectó** a mitad del sprint, así que el check visual de la `LoginPage` (botón de reingreso + SecureStorage) quedó pendiente. Se mitigó verificando el flujo por API + gate, pero la acción de S40 ("verificación on-device de cambios de la app") quedó parcialmente cumplida.
- El "método de seguridad del teléfono" se modeló con `SecureStorage` (Keystore) como stand-in; no es un biométrico real. Es honesto pero no es la experiencia final.
- El reingreso necesita conexión (POST `/auth/reingreso`); el beneficio offline real lo da la cola de capturas (S42), no el reingreso en sí.

## 3. Qué probar

- Completar el check on-device cuando el dispositivo vuelva: login → reabrir la app → aparece "Reingreso en terreno" → reingresar sin clave.
- Evaluar el biométrico nativo (huella/PIN) como `MetodoPresenteAsync` real, con un plugin MAUI.
- Considerar un refresh de token automático (RefreshRequest con método presente) para sesiones largas en terreno.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Completar el check on-device del reingreso al reconectar el dispositivo | AG-05 (QA) | 2028-03-05 | Pendiente |
| Evaluar biométrico nativo (huella/PIN) como método de seguridad real | AG-08 (móvil) | 2028-03-05 | Pendiente |
| Auto-sync al recuperar conectividad (F-M-14) | AG-08 (móvil) | 2028-03-05 | Pendiente (S45) |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 43 | Estado actual |
| --- | --- |
| Método de seguridad + reingreso offline (RN-06, F-M-02/03) | Completada (entregada en este sprint) |
| Evaluar endpoint backend "relevamientos asignados a mí" | Pendiente (se reitera) |
| Script de pre-chequeo del entorno on-device (backend + adb reverse) | Pendiente (se reitera; el entorno volvió a fallar este sprint) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-04 | Retrospectiva del Sprint 44 (método de seguridad + reingreso): RN-06 cerrada con núcleo en el gate y flujo verificado por API; el check visual on-device quedó pendiente por desconexión del dispositivo. 3 acciones nuevas (check on-device, biométrico nativo, auto-sync). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
