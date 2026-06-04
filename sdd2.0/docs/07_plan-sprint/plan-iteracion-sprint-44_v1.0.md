# Plan de Iteración — Sprint 44

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-44_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-02-08
**Fecha fin:** 2028-02-20
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S41–S43); capacidad sugerida estricta 9 SP. Se compromete el **método de seguridad del teléfono + reingreso en terreno** (RN-06, 8 SP), brecha de la revisión funcional. Núcleo en `ServicioSesion` (en el gate); UI y almacén seguro fuera de CI.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Cerrar la brecha **F-M-02/F-M-03** de la revisión funcional (RN-06): la app no configuraba el **método de seguridad del teléfono** (precondición del modo sin conexión) ni ofrecía el **reingreso en terreno** (re-autenticarse al reabrir la app sin reescribir la clave). El backend ya tenía los endpoints (`/auth/metodo-seguridad`, `/auth/offline`, `/auth/reingreso`); faltaba el cliente.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-04-METODO | Historia | Tras el login con conexión, configurar el método de seguridad + habilitar el modo sin conexión; recordar el usuario | Alta | 4 | Dev móvil (AG-08) | Cerrada |
| US-05-REINGRESO | Historia | Reingreso en terreno con el método de seguridad presente, sin reescribir la clave | Alta | 4 | Dev móvil (AG-08) | Cerrada |

Total de puntos comprometidos: 8 SP. Cliente móvil; el núcleo de auth (`ServicioSesion`) vive en `GeoVial.Sync` (en el gate).

## 4. Alcance técnico

1. **Núcleo (`GeoVial.Sync`, en el gate):** `ServicioSesion` suma:
   - `ConfigurarMetodoSeguridadAsync()` → POST `/auth/metodo-seguridad` (true si 204).
   - `HabilitarOfflineAsync()` → POST `/auth/offline` (true si 204; false si 409 "falta método").
   - `ReingresarAsync(usuario, metodoPresente)` → POST `/auth/reingreso`; si el método no está presente ni siquiera llama al backend; traduce 409/errores a un mensaje y, si va bien, asienta el token (reusa el helper común con el login). No lanza.
2. **Almacén seguro del dispositivo (`SeguridadDispositivo`, fuera de CI):** sobre `SecureStorage` (Keystore) recuerda el usuario y un marcador de "método configurado"; `MetodoPresenteAsync` representa "el teléfono tiene su seguridad configurada".
3. **UI (`LoginPage`, fuera de CI):** el login con conexión configura el método + habilita offline + recuerda el usuario; si hay un usuario recordado (al reabrir la app), ofrece **"Reingreso en terreno (sin clave)"**. `MainPage` olvida el método al cerrar sesión explícitamente (el reingreso es para reabrir, no para logout).

## 5. Definition of Done aplicada

- Tras el login, el método de seguridad queda configurado y el modo offline habilitado; al reabrir la app, el reingreso en terreno re-autentica sin reescribir la clave; sin el método presente, el reingreso se rechaza.
- `ServicioSesion` (configurar/offline/reingreso) cubierto por pruebas en el gate. La suite .NET pasa a **374** y sigue verde.
- El MAUI compila para `net10.0-android`.
- **Verificación del flujo RN-06 contra el backend real (API):** reingreso 409 sin método → configurar (204) → offline (204) → reingreso con método 200 + token → reingreso sin método 409. *(El check visual de la `LoginPage` on-device quedó pendiente por desconexión del dispositivo; el flujo está verificado por API + gate.)*

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El "método de seguridad del teléfono" no es un biométrico real | Alta | Bajo | Se modela con `SecureStorage` (Keystore), que en la práctica exige un dispositivo seguro; el biométrico nativo queda como mejora futura |
| Configurar el método tras el login falla y bloquea el ingreso | Media | Medio | Best-effort: si falla, igual se ingresa; el reingreso requerirá conexión una vez más |
| El reingreso necesita conexión (no es 100 % offline) | Media | Bajo | Las capturas ya se encolan offline (S42); el reingreso es la re-autenticación rápida al reabrir |

## 7. Criterios de hecho del sprint

El Sprint 44 se considera completo cuando: el login configura el método + habilita offline + recuerda el usuario, el reingreso en terreno re-autentica con el método presente (y se rechaza sin él), el núcleo está cubierto en el gate, la suite .NET sigue verde (374), el MAUI compila para android, el flujo RN-06 está verificado contra el backend real, y se facilitan el sprint review y la retrospectiva.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | `revision-funcional-app_v1.0.md` §3 (F-M-02/03), RN-06 |
| CU | CU-02 §5.A/§5.B (reingreso en terreno / configurar método); US-04, US-05 |
| Componentes | `GeoVial.Sync` (`ServicioSesion`); `GeoVial.Mobile` (`SeguridadDispositivo`, `LoginPage`, `MainPage`, `MauiProgram`) |
| Calidad | definition-of-done §1.4 |
| Tests previstos | `ServicioSesionTests` (+5 RN-06); verificación del flujo por API |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-04 | Plan del Sprint 44 (método de seguridad + reingreso en terreno, RN-06 / F-M-02/03): `ServicioSesion` (configurar/offline/reingreso) en el gate + `SeguridadDispositivo` (SecureStorage) + `LoginPage`. Suite a 374. Flujo verificado por API; check visual on-device pendiente por desconexión del dispositivo. Compromete 8 SP. Generado por AG-07 |
