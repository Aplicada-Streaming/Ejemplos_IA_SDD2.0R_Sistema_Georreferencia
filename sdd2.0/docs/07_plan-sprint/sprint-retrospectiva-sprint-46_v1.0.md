# Sprint Retrospectiva — Sprint 46

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-46_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Se cerró la grieta de **doble captura** que abrieron S42 + S45: el ciclo offline ahora es seguro ante reintentos, no sólo automático. La clave de idempotencia `CapturaId` ya existía en el cliente; sólo faltaba enviarla y deduplicar en el backend.
- Se reutilizó el **patrón de idempotencia del sync** (`CambioAplicado`) en vez de inventar uno nuevo: misma semántica ("ya aplicado → se devuelve sin reaplicar"), código coherente y fácil de revisar.
- Cambio **compatible hacia atrás**: `CapturaId` opcional al final del DTO/command/factories, así ningún llamador previo se rompió y el comportamiento sin clave quedó intacto (cubierto por un test explícito).
- El núcleo quedó **todo en el gate** (dominio + handler), con unit + E2E; la cobertura del gate se mantuvo por encima del umbral sin esfuerzo extra.

## 2. Qué no salió bien

- La grieta era **invisible en CI**: InMemory (tests/dev) no aplica índices únicos ni la verificación de filas afectadas, así que un test sobre InMemory no "ve" el duplicado por índice. Se mitigó haciendo que la dedup la resuelva el **handler** (no el índice), de modo que el E2E sobre InMemory sí la ejercita; pero el índice único filtrado sólo se valida de verdad sobre SqlServer/SQLite. Es la enésima vez que InMemory esconde un comportamiento relacional.
- La idempotencia se diseñó para el reintento **secuencial** (el escenario realista del auto-sync). La **carrera concurrente** (dos POST simultáneos con la misma clave) sólo la atrapa el índice único en SqlServer; no hay test que lo demuestre porque InMemory no lo soporta. Queda documentado y como riesgo conocido.
- No se verificó on-device: el cambio es de backend + un ajuste menor del cliente, y la suite + E2E cubren el comportamiento. La prueba on-device del ciclo offline completo sigue pendiente (se arrastra).

## 3. Qué probar

- Migrar la suite de integración del flujo de captura/sync a un proveedor **relacional** (SQLite, como ya se hizo para `AsignacionAgente` en la revisión funcional) para que el índice único y la concurrencia sí se ejerciten en CI.
- Probar el ciclo offline completo en modo avión on-device: capturar sin señal → recuperar → confirmar que la cola se vacía sola y **sin duplicar** (cierra esta historia end-to-end).
- Un test de concurrencia (dos altas simultáneas con la misma `CapturaId`) una vez que el flujo corra sobre relacional en CI.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Migrar la integración de captura/sync a SQLite relacional para ejercitar índices únicos en CI | AG-05 (QA) | 2028-03-31 | Pendiente |
| Probar el ciclo offline completo en modo avión on-device (sin duplicar) | AG-05 (QA) | 2028-03-31 | Pendiente (se reitera de S42/S45) |
| Indicador de estado de sincronización en la UI | AG-08 (móvil) | 2028-03-31 | Pendiente (se reitera de S45) |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 45 | Estado actual |
| --- | --- |
| Probar el ciclo offline completo en modo avión on-device | Pendiente (se reitera) |
| Indicador de estado de sincronización en la UI | Pendiente (se reitera) |
| Patrón de "servicios de arranque" que se resuelvan al iniciar | Pendiente (no aplicó a este sprint, de backend) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Retrospectiva del Sprint 46 (idempotencia de la captura): se cerró la grieta de doble captura reutilizando el patrón del sync, compatible hacia atrás y todo en el gate. Se reitera la limitación de InMemory (no aplica índices únicos) y se propone migrar la integración de captura a SQLite relacional. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
