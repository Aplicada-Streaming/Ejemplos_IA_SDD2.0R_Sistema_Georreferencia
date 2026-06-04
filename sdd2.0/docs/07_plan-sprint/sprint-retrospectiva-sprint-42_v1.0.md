# Sprint Retrospectiva — Sprint 42

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-42_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Se cerró la brecha P0 de la app de campo (F-M-12/13): la captura es ahora **offline real** (encola + drena), no un *stub*. NB-03 cumplido en el cliente móvil.
- El núcleo (cola + motor) quedó **en el gate** con 8 pruebas, reutilizando el patrón ya probado de `MotorSincronizacion`; la única diferencia (el binario de la foto) se aisló en su propio camino.
- Se respetó la acción de la retro de S40: se **verificó on-device** el ciclo completo (login agente → tomar foto → encolar → subir), confirmando contra el backend que la observación quedó creada.
- El fix de S41 (asignación de agentes) habilitó la verificación: un agente asignado pudo capturar de punta a punta.

## 2. Qué no salió bien

- El endpoint de captura del backend **no es idempotente** (no hay clave de idempotencia como en /sync): si la app cae entre subir la observación y marcarla, podría duplicarse. Se mitigó marcando apenas el backend responde 2xx, pero el flanco existe (documentado).
- La captura sin conexión **necesita conocer el relevamiento destino**, que hoy se obtiene online; se resolvió recordando el último conocido, pero la selección real (varios relevamientos) sigue pendiente (S43).
- El binario de la foto se sube best-effort (igual que el flujo online): si la observación se crea pero el binario falla, la captura se da por subida y el binario habría que resubirlo; no hay aún un reintento del binario.
- La automatización de la verificación on-device por `adb` sigue siendo frágil (coordenadas a ciegas); se confirmó el resultado de forma autoritativa consultando el backend.

## 3. Qué probar

- Probar el ciclo offline completo en el dispositivo en modo avión (capturar varias sin señal → reconectar → "Sincronizar capturas" sube todas) además del happy path verificado.
- Evaluar una clave de idempotencia para la captura en el backend (que `CapturaId` viaje y el backend deduplique), cerrando el flanco de duplicados.
- Agregar un reintento del binario de la foto cuando la observación ya existe pero el contenido no se subió.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Selección de relevamiento asignado en el móvil (F-M-04/05) | AG-08 (móvil) | 2028-02-05 | Pendiente (S43) |
| Probar el ciclo offline en modo avión en el dispositivo | AG-05 (QA) | 2028-02-05 | Pendiente |
| Evaluar idempotencia de la captura en el backend (dedup por `CapturaId`) | AG-06 (backend) | 2028-02-05 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 41 | Estado actual |
| --- | --- |
| Evaluar una pasada de integración contra proveedor relacional (SQLite/LocalDB) en CI | Pendiente (se reitera); este sprint sumó pruebas SQLite de la cola, pero la pasada de integración completa sigue pendiente |
| Auditar entidades de agregados que pre-asignen PK `ValueGeneratedOnAdd` | Pendiente (se reitera) |
| Continuar el backlog de la revisión funcional: captura offline real (S42) | Completada (entregada en este sprint) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Retrospectiva del Sprint 42 (captura offline real): cierra la brecha P0 de la app de campo, con núcleo en el gate y verificación on-device. 3 acciones nuevas (selección de relevamiento, ciclo offline en modo avión, idempotencia de la captura). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
