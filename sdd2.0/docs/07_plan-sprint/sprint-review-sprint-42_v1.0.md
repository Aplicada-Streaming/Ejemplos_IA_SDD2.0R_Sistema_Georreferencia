# Sprint Review — Sprint 42

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-42_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-42_v1.0.md`:

> Que una captura (foto + coordenada) se encole localmente y funcione sin conexión, y se suba al reconectar, conservándose sin pérdida durante la jornada.

Veredicto: Cumplido.

Explicación corta: se agregó el camino de **captura offline real**. Una `CapturaPendiente` (foto en bytes + coordenada EXIF/manual + relevamiento) se encola en `ColaCapturasSqlite` (SQLite, foto en BLOB, idempotente por `CapturaId`). `MotorCapturas` drena la cola subiendo cada captura vía `ICapturaBackendClient` (POST observación + binario), marcando como subidas las aceptadas y conservando lo pendiente ante un corte de conexión. `CapturaPage` ahora **encola** en vez de postear directo, e intenta drenar en el acto; "Sincronizar capturas" drena lo pendiente. El núcleo (cola + motor) vive en `GeoVial.Sync`, cubierto por pruebas en el gate; el cliente REST y la UI se verificaron on-device.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-16-OFFLINE | Funcionalidad | "Enviar captura" encola la captura (anda sin conexión) y la sube al sincronizar | La app de campo por fin captura offline |
| CU-06 | Robustez | Sin conexión, la captura queda en la cola y se sube al reconectar | No se pierde trabajo en terreno |
| BT-CAP-TESTS | Calidad | `MotorCapturas` + `ColaCapturasSqlite` cubiertos en el gate (8 casos) | La lógica queda protegida por la CI |

## 3. Feedback recibido

- Cierra la brecha más grave de la app de campo: la captura ya no depende de tener señal en el momento (NB-03). Antes, la cola offline sólo la usaba un *stub* de demo; ahora la alimenta la captura real.
- El núcleo (cola + motor) se modeló igual que el de comentarios (`MotorSincronizacion`), reutilizando el patrón ya probado; la diferencia es que la captura lleva el binario de la foto, por eso va por su propio camino y no por /sync.
- La verificación on-device confirmó el ciclo completo end-to-end (encolar → subir → observación creada en el backend), apoyada además en el fix de S41 (un agente asignado ya puede capturar).

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: **354** (314 unitarias + 40 de integración), +8 unitarias (`MotorCapturasTests` 4 + `ColaCapturasSqliteTests` 4): drena todo, orden por marca temporal, corte de conexión conserva lo pendiente, rechazo 4xx no cicla; persistencia de la foto BLOB, sin coordenada, idempotencia, marcar subida. El MAUI compila para `net10.0-android`. Gate de cobertura mantenido.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-16-OFFLINE | Historia | Aceptada (captura encola offline + drena al sincronizar; verificada on-device) |
| BT-CAP-TESTS | Tarea | Aceptada (núcleo cola + motor cubierto en el gate, 8 casos) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 42 se traslada. |

Backlog restante (revisión funcional §5/§6): selección de relevamiento asignado (S43, F-M-04/05), método de seguridad + reingreso offline (S44, RN-06), auto-sync al recuperar conectividad, y P2/P3.

## 7. Decisiones tomadas durante el review

- Encolar siempre la captura primero (offline-first) e intentar drenar en el acto, en vez de postear directo; así no se pierde trabajo sin señal.
- Modelar la captura como un camino propio (con binario) en paralelo a la sincronización de comentarios, reutilizando el patrón del motor.
- Recordar el relevamiento destino conocido para capturar sin conexión; la selección con varios relevamientos se difiere a S43.
- Aceptar la subida de binario best-effort (igual que el flujo online) para no duplicar observaciones; documentado como límite conocido.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Sprint review del Sprint 42 (captura offline real). Veredicto Cumplido, velocity 8, 0 carry-over, 354 pruebas (+8 unitarias); verificado on-device (captura encolada y subida; observación creada en el backend). Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
