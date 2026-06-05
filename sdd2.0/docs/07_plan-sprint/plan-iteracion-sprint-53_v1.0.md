# Plan de Iteración — Sprint 53

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-53_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-06-12
**Fecha fin:** 2028-06-23
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 7,0 SP (S50–S52, deprimido por el sprint de limpieza S52=5); se vuelve al rango habitual con **8 SP**. Se compromete el **biométrico nativo** (último ítem de feature del backlog de la revisión funcional). Núcleo (abstracción + coordinador de reingreso) en el gate; la implementación `BiometricPrompt` es de plataforma.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Hasta S44 el "método de seguridad del teléfono" (RN-06) era un **marcador blando** en SecureStorage: su sola presencia habilitaba el reingreso en terreno sin clave, sin verificar realmente la identidad. El objetivo es hacerlo **real**: antes de reingresar, el teléfono debe **verificar al usuario con su método nativo** (huella/rostro/PIN, Android `BiometricPrompt`). Sólo si la verificación tiene éxito se reingresa contra el backend.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-BIOMETRICO | Historia | Reingreso en terreno con verificación biométrica nativa (huella/rostro/PIN) en vez del marcador blando | Alta | 5 | Dev móvil (AG-08) | Cerrada |
| BT-BIO-NUCLEO | Tarea | Núcleo en el gate: `IAutenticadorBiometrico` + `CoordinadorReingreso` (verifica → reingresa); decisión sin depender del SO | Alta | 2 | Dev fullstack (AG-09) | Cerrada |
| BT-BIO-ANDROID | Tarea | Implementación `BiometricPrompt` (API 28+) + permiso `USE_BIOMETRIC` + cableado | Media | 1 | Dev móvil (AG-08) | Cerrada |

Total de puntos comprometidos: 8 SP. Núcleo en `GeoVial.Sync` (en el gate); implementación en `GeoVial.Mobile` (Android).

## 4. Alcance técnico

1. **Núcleo (en el gate):** `IAutenticadorBiometrico` (`HayMetodoDisponibleAsync`, `AutenticarAsync(motivo)` → `ResultadoBiometrico`: Exito/Fallo/Cancelado/NoDisponible) + `CoordinadorReingreso`, que verifica con el método nativo y **sólo ante éxito** llama a `ServicioSesion.ReingresarAsync(usuario, metodoPresente: true)`. La verificación exitosa **es** el método presente (RN-06), no un marcador blando. Mensajes de UI por cada resultado.
2. **Implementación Android (plataforma):** `AutenticadorBiometricoAndroid` usa el `BiometricPrompt` de la plataforma (API 28+; `Android.Hardware.Biometrics`, **sin agregar NuGet**) y `KeyguardManager.IsDeviceSecure` para la disponibilidad. En API < 28 o sin método configurado → `NoDisponible`.
3. **Manifiesto:** permiso `USE_BIOMETRIC`.
4. **UI (`LoginPage`):** el reingreso usa el coordinador (pide el biométrico). El login sólo configura el método + habilita offline + recuerda el usuario **si hay método nativo disponible**, para que el reingreso sea coherente.
5. **Sin cambios de backend:** el contrato `/auth/reingreso` ya recibe `metodoSeguridadPresente`; ahora ese `true` lo respalda una verificación real.

## 5. Definition of Done aplicada

- El reingreso en terreno exige verificación biométrica nativa exitosa; si falla/cancela/no hay método, no reingresa y lo explica.
- Núcleo (abstracción + coordinador) cubierto por pruebas en el gate; los tests de reingreso previos (S44) siguen verdes.
- Cobertura DoD respetada (Domain/Application líneas ≥80 %, ramas ≥70 %); `GeoVial.Sync` bien cubierto.
- El MAUI compila para `net10.0-android` con el `BiometricPrompt` y el permiso.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| La API nativa `BiometricPrompt` requiere paquete/binding extra | Media | Medio | Se usa el API de **plataforma** (`Android.Hardware.Biometrics`, API 28+), sin NuGet; el build valida las firmas |
| El flujo real no se puede verificar sin dispositivo | Alta | Medio | La decisión (verificar→reingresar) está cubierta en el gate con un autenticador falso; la verificación on-device queda como acción de retro |
| Dispositivos sin huella/PIN quedarían sin reingreso | Media | Bajo | `NoDisponible` con mensaje claro: reingresar con usuario y clave; el login no configura el método si no hay biométrico |

## 7. Criterios de hecho del sprint

El Sprint 53 se considera completo cuando: el reingreso exige verificación biométrica nativa (núcleo en el gate), la implementación Android compila con el permiso, la cobertura DoD se mantiene, los tests de reingreso previos siguen verdes, y se facilitan el sprint review y la retrospectiva.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | `revision-funcional-app_v1.0.md` (backlog: biométrico nativo) |
| CU | CU-02 (acceso/reingreso); RN-06 (método de seguridad del teléfono) |
| Componentes | `GeoVial.Sync` (`IAutenticadorBiometrico`, `CoordinadorReingreso`); `GeoVial.Mobile` (`AutenticadorBiometricoAndroid`, `LoginPage`, manifiesto) |
| Calidad | definition-of-done §1.4 (cobertura) |
| Tests previstos | `CoordinadorReingresoTests` (biométrico OK → reingresa; fallo/cancelado/no-disponible no reingresa; sin usuario no pide; disponibilidad delega) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Plan del Sprint 53 (biométrico nativo, RN-06): `IAutenticadorBiometrico` + `CoordinadorReingreso` en el gate + `BiometricPrompt` de plataforma (API 28+, sin NuGet) + permiso + `LoginPage`. Sin cambios de backend. Compromete 8 SP. Generado por AG-07 |
