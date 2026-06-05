# Plan de Iteración — Sprint 54

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-54_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-06-26
**Fecha fin:** 2028-07-07
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 7,0 SP (S51–S53, arrastra el S52=5 acotado); se compromete **8 SP**. Sprint de **fixes hallados en la verificación on-device** (sesión QA con el moto g42), análogo a S40. Núcleo (persistencia de sesión) en el gate.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Al probar en el dispositivo real surgieron dos cosas a corregir:

1. **La app se "resetea" al volver de la cámara.** En equipos con poca RAM, Android **mata el proceso** mientras la cámara está en primer plano; al volver, la app arranca en frío y la sesión —que vivía sólo en memoria— se perdía, rebotando al **login**. Hay que **persistir la sesión** para que sobreviva a la recreación del proceso.
2. **No se podía ver la clave al tipearla** en el login (errores de tipeo en el teléfono). Falta un **"ojito"** para mostrar/ocultar la clave.

Además se corrigió, durante la sesión, que el **biométrico** (S53) sólo aceptaba huella/rostro: en un teléfono con **patrón/PIN** (sin huella) el reingreso fallaba. Se habilitó la **credencial del dispositivo** (patrón/PIN), más fiel a RN-06.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BUG-CAMARA-RESET | Bug | Persistir la sesión (token) para que sobreviva a que el SO mate el proceso al usar la cámara; no rebotar al login | Alta | 5 | Dev móvil (AG-08) | Cerrada |
| US-LOGIN-OJITO | Historia | Mostrar/ocultar la clave en el login | Baja | 1 | Dev móvil (AG-08) | Cerrada |
| BUG-BIO-PATRON | Bug | El reingreso biométrico debe aceptar la credencial del dispositivo (patrón/PIN), no sólo huella (RN-06) | Alta | 2 | Dev fullstack (AG-09) | Cerrada |

Total de puntos comprometidos: 8 SP. Núcleo en `GeoVial.Sync`; implementación y UI en `GeoVial.Mobile`.

## 4. Alcance técnico

1. **Persistencia de sesión (núcleo en el gate):** `IAlmacenTokenSesion` (guardar/leer/limpiar) + `ServicioSesion` que persiste el token al asentarlo, lo **restaura** (`RestaurarAsync`) y lo limpia al cerrar sesión. Parámetro **opcional** (los tests previos no se tocan). Implementación móvil `AlmacenTokenSecureStorage` (SecureStorage/Keystore). `App.CreateWindow` restaura al arrancar (en un hilo del pool, sin bloquear la UI) → vuelve a las solapas en vez del login.
2. **Ojito (`LoginPage`):** botón que alterna `Entry.IsPassword` y su glifo (👁/🙈).
3. **Biométrico con credencial de dispositivo (`AutenticadorBiometricoAndroid`):** `SetAllowedAuthenticators(BIOMETRIC_WEAK | DEVICE_CREDENTIAL)` en API 30+ (o `SetDeviceCredentialAllowed` en 29); piso API 29. Permiso `USE_BIOMETRIC` ya estaba.

## 5. Definition of Done aplicada

- Tras una recreación de proceso, la app **restaura la sesión** y vuelve a las solapas (no al login); cerrar sesión limpia el token persistido.
- El login permite mostrar/ocultar la clave.
- El reingreso biométrico acepta patrón/PIN del dispositivo.
- Núcleo (persistencia/restauración) cubierto en el gate; los tests previos siguen verdes.
- Cobertura DoD respetada (Domain/Application líneas ≥80 %, ramas ≥70 %); `GeoVial.Sync` bien cubierto.
- El MAUI compila y se **verifica on-device** (arranque + restauración).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Bloquear la UI o deadlock al restaurar en el arranque | Media | Medio | La restauración corre en un hilo del pool (`Task.Run(...).GetAwaiter().GetResult()`) con try/catch; SecureStorage es de lectura rápida |
| La foto en curso igual se pierde si el proceso muere | Media | Bajo | El objetivo es **no perder la sesión** (no rebotar al login); re-tomar la foto funciona y el agente sigue logueado. Persistir la captura en vuelo queda fuera de alcance |
| El token persistido queda tras desinstalar/compartir equipo | Baja | Medio | Se limpia al cerrar sesión; SecureStorage es por-app y cifrado por el Keystore |

## 7. Criterios de hecho del sprint

El Sprint 54 se considera completo cuando: la sesión se persiste y restaura tras la recreación de proceso (no rebota al login), el login muestra/oculta la clave, el biométrico acepta patrón/PIN, el núcleo está cubierto en el gate sin romper lo previo, la cobertura DoD se mantiene, el MAUI compila y se verifica el arranque/restauración on-device, y se facilitan el sprint review y la retrospectiva.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Sesión de verificación on-device (moto g42): reseteo al volver de la cámara, falta de "ojito", biométrico que no aceptaba patrón |
| CU | CU-02 (acceso/sesión), RN-06 (método de seguridad del teléfono) |
| Componentes | `GeoVial.Sync` (`IAlmacenTokenSesion`, `ServicioSesion`); `GeoVial.Mobile` (`AlmacenTokenSecureStorage`, `App`, `LoginPage`, `AutenticadorBiometricoAndroid`) |
| Calidad | definition-of-done §1.4 (cobertura) |
| Tests previstos | `ServicioSesionTests` (+3): sesión persistida se restaura; cerrar sesión limpia la persistencia; sin almacén no rompe |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Plan del Sprint 54 (fixes on-device): persistencia/restauración de sesión (fix reseteo al volver de la cámara) + ojito en el login + biométrico con credencial de dispositivo (patrón/PIN). Compromete 8 SP. Generado por AG-07 |
