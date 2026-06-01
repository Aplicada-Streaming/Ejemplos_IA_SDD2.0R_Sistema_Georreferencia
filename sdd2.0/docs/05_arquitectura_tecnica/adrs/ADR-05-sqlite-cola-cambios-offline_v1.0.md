# ADR-05 — SQLite + cola de cambios para soporte sin conexión en móvil

**Proyecto:** GeoVial
**Documento:** ADR-05-sqlite-cola-cambios-offline_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Categoría:** Persistencia

## 1. Contexto

El agente de campo recolecta sin señal y debe poder operar al menos una jornada laboral completa (8 h) guardando observaciones localmente y encolando los cambios para sincronizar al recuperar conexión (CU-06, CU-07). Cada cambio encolado debe tener un identificador único para garantizar idempotencia ante reintentos o sincronizaciones parciales (RC-03, RN-04). Hay riesgo de pérdida de observaciones antes de sincronizar (R-02 de negocio). El cliente fijó SQLite (sqlite-net-pcl) como persistencia local móvil (PROJECT-README §7). NFR: operación offline ≥ 8 h; sincronización ≤ 5 min para ≈100 observaciones; detección de conectividad automática.

## 2. Decisión

Se adopta SQLite como almacenamiento local durable de la app móvil, con una tabla de cola de cambios (RegistroCambioSync) que registra cada operación con tipo, entidad afectada, referencia, marca temporal e identificador único de idempotencia, y un estado de sincronización. La captura sin conexión persiste en SQLite y encola el cambio; al recuperar señal, el motor de sincronización drena la cola.

## 3. Estado

Aceptado el 2026-06-01. Renumeración de `ADR-005` (PROJECT-README §15, estado original Propuesto) a `ADR-05`.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| SQLite + cola de cambios (elegido) | Durable, embebido, sin servidor; soporta offline ≥ 8 h; cola idempotente por identificador único | Requiere gestionar el esquema local y la convergencia con el backend |
| Almacenamiento en archivos planos / preferencias | Trivial de implementar | Sin consultas ni integridad; frágil ante volumen de una jornada |
| Almacenamiento solo en memoria con sync inmediata | Sin esquema local | Incumple offline ≥ 8 h; pierde datos ante cierre de la app o falta de señal |

## 5. Consecuencias positivas

1. Persistencia durable que sostiene una jornada completa sin conexión (NFR offline ≥ 8 h).
2. La cola con identificador único garantiza idempotencia (RC-03) y evita duplicar cambios ante reintentos.
3. Mitiga el riesgo de pérdida de observaciones (R-02): los datos sobreviven a cortes de señal y cierres de la app.

## 6. Consecuencias negativas y trade-offs

1. Hay que mantener el esquema SQLite local en paralelo al esquema SQL Server del backend; aceptado y documentado en el modelo lógico.
2. La convergencia con el backend es eventual; los conflictos se resuelven por last-write-wins y marca de conflicto (ADR-06).

## 7. Implementación

`GeoVial.Mobile` persiste en SQLite vía sqlite-net-pcl. El esquema de la cola local se detalla en `modelo-datos-logico_v1.0.md` (tabla aparte para SQLite). El módulo de captura local (CU-06) encola cada cambio; el módulo de sincronización (`GeoVial.Sync`, ADR-07) lo drena (CU-07). La detección de conectividad dispara la sincronización automáticamente. Las fotos se comprimen/redimensionan para acotar el payload de sincronización.

## 8. Métricas de validación

- Captura continua durante 8 h sin red conserva todos los cambios en la cola (NFR offline).
- Sincronización de 100 cambios en ≤ 5 min (NFR de sincronización, CA-01 de CU-07).
- Un reintento no aplica el mismo cambio dos veces (idempotencia, RC-03).

## 9. Referencias

- PROJECT-README §7 (persistencia móvil), §13 (NFR).
- RN-04, RN-06; RC-03; CU-06, CU-07.
- ADR-06 (conflictos), ADR-07 (librería de sync), `flujo-ejecucion_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. Decisión aceptada. Renumeración de ADR-005 a ADR-05 |
