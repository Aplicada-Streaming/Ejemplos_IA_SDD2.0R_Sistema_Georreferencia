# Sprint Review — Sprint 53

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-53_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-53_v1.0.md`:

> El objetivo es hacerlo real: antes de reingresar, el teléfono debe verificar al usuario con su método nativo (huella/rostro/PIN, Android BiometricPrompt). Sólo si la verificación tiene éxito se reingresa contra el backend.

Veredicto: Cumplido.

Explicación corta: el "método de seguridad del teléfono" (RN-06) era un marcador blando en SecureStorage. Ahora el reingreso pasa por `CoordinadorReingreso`, que pide al teléfono verificar la identidad con su método nativo (`IAutenticadorBiometrico`) y **sólo ante éxito** llama a `ServicioSesion.ReingresarAsync(usuario, metodoPresente: true)`. La implementación Android usa el `BiometricPrompt` de la plataforma (API 28+, sin agregar NuGet). Sin cambios de backend: el `metodoSeguridadPresente=true` ahora lo respalda una verificación real.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-BIOMETRICO | Funcionalidad | Reingreso en terreno: el teléfono pide huella/rostro/PIN; sólo si verifica, entra sin clave | La seguridad del reingreso ahora es real, no un marcador |
| BT-BIO-NUCLEO | Robustez | El coordinador verifica → reingresa; si falla/cancela/no hay método, no toca el backend y lo explica | Cubierto en el gate con autenticador falso |

## 3. Feedback recibido

- Convierte RN-06 de un marcador blando en una verificación real: cierra el último ítem de feature del backlog de la revisión funcional.
- El núcleo (decisión verificar→reingresar) quedó en el gate; la dependencia de plataforma (`BiometricPrompt`) se aisló detrás de `IAutenticadorBiometrico`. Buen reparto gate/glue.
- Se evitó agregar un binding NuGet usando el `BiometricPrompt` de plataforma (API 28+); el moto g42 (Android 12) lo soporta de sobra.
- Honesto: la verificación biométrica **real** no se pudo probar on-device en este sprint; el build valida las firmas de la API y el núcleo cubre la decisión. Queda como acción de retro (junto con la verificación on-device acumulada).

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: **434** (391 unitarias + 43 de integración), +6 unitarias en `CoordinadorReingresoTests`: biométrico OK → reingresa y asienta el token; fallo/cancelado/no-disponible no llaman al backend y explican; sin usuario no pide verificación; la disponibilidad delega en el autenticador. Los tests de reingreso de S44 siguen verdes (el contrato de `ServicioSesion.ReingresarAsync` no cambió). Cobertura del gate: Domain 88,5 % / 80,5 %; Application 87,6 % / 76,3 % (umbral 80 % / 70 %); `GeoVial.Sync` a 94,9 % / 88,4 %. El MAUI compila para `net10.0-android` (a la primera; las firmas de la API nativa eran correctas).

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-BIOMETRICO | Historia | Aceptada (reingreso con verificación biométrica nativa) |
| BT-BIO-NUCLEO | Tarea | Aceptada (abstracción + coordinador en el gate) |
| BT-BIO-ANDROID | Tarea | Aceptada (`BiometricPrompt` de plataforma + permiso; on-device pendiente) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 53 se traslada. |

Con esto, el **backlog de la revisión funcional queda cerrado por completo** (incluida su única feature de plataforma). Pendiente acumulado: **verificación on-device** de varios sprints (indicador S49, bandeja S50, ubicar S51, biométrico S53). Próximos rumbos posibles (a definir con el Product Owner): sesión de verificación on-device, endurecimiento/escala (paginación, observabilidad), o nuevas historias fuera del backlog.

## 7. Decisiones tomadas durante el review

- Aislar el biométrico tras `IAutenticadorBiometrico` y poner la decisión en `CoordinadorReingreso` (gate).
- Usar el `BiometricPrompt` de **plataforma** (API 28+) en vez de un binding NuGet, para no asumir riesgo de dependencia.
- La verificación nativa exitosa **reemplaza** al marcador blando como "método presente" (RN-06).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Sprint review del Sprint 53 (biométrico nativo, RN-06). Veredicto Cumplido, velocity 8, 0 carry-over, 434 pruebas (+6); cobertura del gate mantenida, `GeoVial.Sync` a 94,9/88,4. Cierra el backlog de la revisión funcional. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
