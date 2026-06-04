# Sprint Review — Sprint 44

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-44_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-04
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-44_v1.0.md`:

> La app no configuraba el método de seguridad del teléfono ni ofrecía el reingreso en terreno. Cerrar F-M-02/03 (RN-06): configurar el método tras el login y reingresar en terreno sin reescribir la clave.

Veredicto: Cumplido.

Explicación corta: `ServicioSesion` suma `ConfigurarMetodoSeguridadAsync` (POST `/auth/metodo-seguridad`), `HabilitarOfflineAsync` (POST `/auth/offline`) y `ReingresarAsync(usuario, metodoPresente)` (POST `/auth/reingreso`), reusando el mismo asentado de token que el login. La `LoginPage`, tras un login con conexión, configura el método + habilita offline + recuerda el usuario en `SeguridadDispositivo` (SecureStorage); al reabrir la app ofrece **"Reingreso en terreno (sin clave)"** que re-autentica con el método presente. El flujo se verificó de punta a punta contra el backend real (API).

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-04-METODO | Funcionalidad | El login configura el método de seguridad y habilita el modo sin conexión | El agente queda listo para trabajar offline |
| US-05-REINGRESO | Funcionalidad | Al reabrir la app, "Reingreso en terreno" re-autentica sin reescribir la clave | Re-entrada rápida en el campo |
| RN-06 | Robustez | Sin el método presente/configurado, el reingreso se rechaza (409) | La regla de seguridad se respeta |

## 3. Feedback recibido

- Cierra la precondición del trabajo sin conexión (RN-06): el método de seguridad habilita el modo offline, y el reingreso evita reescribir la clave en terreno.
- El núcleo de auth quedó en `ServicioSesion` (en el gate), reusando el asentado de token del login; el almacén seguro (`SecureStorage`) y la UI quedaron como glue móvil.
- El "método de seguridad del teléfono" se modeló con `SecureStorage` (Keystore) como stand-in pragmático; el biométrico nativo queda como mejora futura.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: **374** (334 unitarias + 40 de integración), +5 unitarias (`ServicioSesionTests`): reingreso con método asienta el token, reingreso sin método no llama al backend, reingreso 409 da mensaje claro, configurar método + habilitar offline (204), offline sin método (409 → false). El MAUI compila para `net10.0-android`. Flujo RN-06 verificado contra el backend real (API): `reingreso 409 sin método → metodo-seguridad 204 → offline 204 → reingreso con método 200+token → reingreso sin método 409`. El gate de cobertura se mantiene.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-04-METODO | Historia | Aceptada (login configura método + habilita offline + recuerda usuario) |
| US-05-REINGRESO | Historia | Aceptada (reingreso en terreno con método presente; rechazo sin él) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 44 se traslada. |

Pendiente menor (no compromiso): el **check visual de la `LoginPage` on-device** (botón de reingreso + SecureStorage) quedó pendiente por desconexión del dispositivo; el flujo está verificado por API + gate. Backlog restante (revisión funcional §5/§6): auto-sync al recuperar conectividad (F-M-14), bandeja sin georreferenciar, avisos de conflictos/pendientes, idempotencia de la captura, y el biométrico nativo.

## 7. Decisiones tomadas durante el review

- Configurar el método + habilitar offline como best-effort tras el login (no bloquear el ingreso si falla).
- Reusar el asentado de token del login para el reingreso (un solo camino de sesión).
- Olvidar el método al cerrar sesión explícitamente; conservarlo ante un cierre del proceso (para el reingreso al reabrir).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-04 | Sprint review del Sprint 44 (método de seguridad + reingreso en terreno, RN-06). Veredicto Cumplido, velocity 8, 0 carry-over, 374 pruebas (+5 unitarias); flujo verificado por API. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
