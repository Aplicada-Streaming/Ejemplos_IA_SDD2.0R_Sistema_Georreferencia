# Referencia de API — Librería de sincronización GeoVial

**Proyecto:** GeoVial
**Documento:** referencia-api_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Technical Writer / Developer Advocate Senior (AG-10), Equipo SDD 2.0
**Tipo Diátaxis:** Reference
**Audiencia:** Developer integrador que reutiliza la librería de sincronización en otros proyectos
**Nivel:** Avanzado
**Tiempo estimado de lectura:** 16 min

---

Esta referencia documenta la superficie pública (capa Abstractions) de `GeoVial.Sync` con paridad uno a uno con [contratos-abstractions-sync_v1.0.md](../05_arquitectura_tecnica/contratos-abstractions-sync_v1.0.md) de 05. Toda firma deriva de la responsabilidad declarada en ese contrato; no se documenta ningún tipo ni método ausente del contrato. La superficie interna (implementaciones concretas) no forma parte del contrato y puede cambiar sin bump MAJOR.

## 1. Tipos públicos

Cuatro tipos de datos públicos, en paridad con §4 del contrato de 05.

### `ChangeRecord`

Unidad de cambio encolada. Propósito: representar una operación local pendiente de sincronizar, identificada de forma única para idempotencia.

| Propiedad | Tipo | Descripción | Invariante |
| --- | --- | --- | --- |
| `ChangeId` | `string` | Identificador único de idempotencia (RC-03). | No se repite en la cola; un reintento con el mismo `ChangeId` no aplica el cambio dos veces. |
| `OperationType` | `OperationType` | Tipo de operación: `Create` o `Update`. | Uno de los valores del enum. |
| `Entity` | `string` | Nombre lógico de la entidad afectada. | No vacío. |
| `EntityRef` | `string` | Referencia al recurso concreto dentro de la entidad. | No vacío. |
| `Timestamp` | `DateTimeOffset` | Marca temporal en UTC; ordena la cola y dirime last-write-wins (RN-04). | En UTC. |
| `Payload` | `string` | Contenido serializado del cambio. | Agnóstico del dominio de la librería. |

`OperationType` es un enum público con los valores `Create` y `Update` (los únicos declarados en §4 del contrato).

### `SyncResult`

Resultado de un ciclo de sincronización. Propósito: reportar qué se confirmó, qué quedó en conflicto y qué actualizaciones se bajaron.

| Propiedad | Tipo | Descripción |
| --- | --- | --- |
| `Confirmed` | `IReadOnlyList<string>` | `ChangeId` de los cambios subidos y confirmados (vaciados de la cola). |
| `Conflicts` | `IReadOnlyList<ConflictInfo>` | Conflictos detectados durante la consolidación. |
| `Updates` | `IReadOnlyList<ChangeRecord>` | Actualizaciones remotas bajadas del backend. |

### `ConflictInfo`

Conflicto detectado, expuesto para resolución posterior. Propósito: señalar lo que la consolidación no dirime sola.

| Propiedad | Tipo | Descripción |
| --- | --- | --- |
| `ConflictId` | `string` | Identificador del conflicto. |
| `Kind` | `ConflictKind` | `FieldConflict` (choque de campo, last-write-wins) o `MarkersWithinRadius` (marcadores en un mismo radio, RN-02). |
| `InvolvedResources` | `IReadOnlyList<string>` | Recursos involucrados en el conflicto. |

`ConflictKind` es un enum público con los valores `FieldConflict` y `MarkersWithinRadius` (§4 del contrato).

### `SyncOptions`

Opciones del ciclo de sincronización.

| Propiedad | Tipo | Descripción | Default |
| --- | --- | --- | --- |
| `BatchSize` | `int` | Cantidad de cambios por lote de subida. | Definido por la implementación. |
| `RetryPolicy` | `RetryPolicy` | Política de reintento ante fallos transitorios. | Definido por la implementación. |

## 2. Métodos

Los métodos derivan de las responsabilidades declaradas en §3 del contrato de 05 para cada interfaz pública.

### `IChangeQueue`

Responsabilidad (contrato §3): encolar un cambio local, leer pendientes ordenados por marca temporal, marcar confirmado, vaciar lo sincronizado.

| Método | Firma | Parámetros | Retorno | Excepciones |
| --- | --- | --- | --- | --- |
| Encolar | `Task EnqueueAsync(ChangeRecord change, CancellationToken ct = default)` | `change` (obligatorio): cambio a encolar. `ct` (opcional). | `Task` | — |
| Leer pendientes | `Task<IReadOnlyList<ChangeRecord>> GetPendingAsync(CancellationToken ct = default)` | `ct` (opcional). | Lista ordenada por `Timestamp`. | — |
| Marcar confirmado | `Task MarkConfirmedAsync(string changeId, CancellationToken ct = default)` | `changeId` (obligatorio). `ct` (opcional). | `Task` | — |
| Vaciar sincronizados | `Task DrainConfirmedAsync(CancellationToken ct = default)` | `ct` (opcional). | `Task` | — |

### `ISyncEngine`

Responsabilidad (contrato §3): orquestar el pipeline (subir locales, bajar actualizaciones, reportar conflictos); idempotencia por identificador de cambio.

| Método | Firma | Parámetros | Retorno | Excepciones |
| --- | --- | --- | --- | --- |
| Sincronizar | `Task<SyncResult> SynchronizeAsync(SyncOptions options, CancellationToken ct = default)` | `options` (obligatorio). `ct` (opcional). | `SyncResult` | `ConsolidationException`, `ConflictNotMarkedException`, `SyncInterruptedException` |

### `ISyncBackendClient`

Responsabilidad (contrato §3): puerto hacia el backend. La implementación concreta (por ejemplo REST contra `/api/v1/.../sync`) vive fuera de Abstractions y la provee el consumidor.

| Método | Firma | Parámetros | Retorno | Excepciones |
| --- | --- | --- | --- | --- |
| Subir | `Task<SyncResult> PushAsync(IReadOnlyList<ChangeRecord> changes, CancellationToken ct = default)` | `changes` (obligatorio). `ct` (opcional). | `SyncResult` con confirmados y conflictos (Updates vacío en la subida). | `SyncInterruptedException` |
| Bajar | `Task<IReadOnlyList<ChangeRecord>> PullAsync(CancellationToken ct = default)` | `ct` (opcional). | Actualizaciones remotas. | `SyncInterruptedException` |

`PushAsync` retorna `SyncResult` (tipo público contractualizado en `contratos-abstractions-sync_v1.0.md` §4): en la subida pobla `Confirmed` (identificadores aceptados) y `Conflicts` (conflictos detectados), y deja `Updates` vacío.

### `IConflictReporter`

Responsabilidad (contrato §3): exponer los conflictos detectados (last-write-wins y marcadores en un mismo radio) para resolución posterior.

| Método | Firma | Parámetros | Retorno | Excepciones |
| --- | --- | --- | --- | --- |
| Publicar conflictos | `Task PublishAsync(IReadOnlyList<ConflictInfo> conflicts, CancellationToken ct = default)` | `conflicts` (obligatorio). `ct` (opcional). | `Task` | — |
| Listar pendientes | `Task<IReadOnlyList<ConflictInfo>> GetUnresolvedAsync(CancellationToken ct = default)` | `ct` (opcional). | Conflictos sin resolver. | — |

### `IConnectivityMonitor`

Responsabilidad (contrato §3): notificar la recuperación de conexión para disparar la sincronización automática.

| Miembro | Firma | Descripción |
| --- | --- | --- |
| Estado | `bool IsConnected { get; }` | Indica si hay conexión disponible. |
| Evento | `event EventHandler ConnectivityRestored` | Se publica al recuperar conexión (ver §3 Eventos). |

## 3. Eventos

| Evento | Publicado por | Payload | Semántica de orden y entrega |
| --- | --- | --- | --- |
| `ConnectivityRestored` | `IConnectivityMonitor` | `EventArgs` (sin datos adicionales). | Se publica al detectar el paso de sin conexión a con conexión. Es la señal para invocar `ISyncEngine.SynchronizeAsync`. La detección de conectividad es automática (NFR de GeoVial). |

## 4. Excepciones

Paridad uno a uno con §5 del contrato de 05. Cada excepción mapea a un código del catálogo de dominio.

| Excepción | Código de dominio | Cuándo se lanza |
| --- | --- | --- |
| `ConsolidationException` | `CONSOLIDACION_INVALIDA` (RN-04) | La consolidación no pudo aplicar last-write-wins. |
| `ConflictNotMarkedException` | `CONFLICTO_NO_MARCADO` (RN-04) | Un recurso consolidado quedó sin la marca de conflicto requerida. |
| `SyncInterruptedException` | `SINCRONIZACION_INTERRUMPIDA` (CU-07) | Se cortó la conexión durante la sincronización. Los cambios no confirmados permanecen en la cola para reanudar sin duplicar. |

Los consumidores que usen un cliente REST contra GeoVial reciben además las respuestas Problem Details del contrato REST (ver `contratos-rest_v1.0.md` §5 en 05).

## 5. Ejemplos breves por método

### `ISyncEngine.SynchronizeAsync` (método más complejo del pipeline)

```csharp
SyncResult result = await engine.SynchronizeAsync(new SyncOptions(batchSize: 50));
Console.WriteLine($"Confirmados {result.Confirmed.Count}, conflictos {result.Conflicts.Count}");
```

### `IChangeQueue.EnqueueAsync` / `GetPendingAsync`

```csharp
await queue.EnqueueAsync(new ChangeRecord(
    Guid.NewGuid().ToString(), OperationType.Update, "Comentario", "com-77",
    DateTimeOffset.UtcNow, payloadJson));
var pendientes = await queue.GetPendingAsync(); // ordenados por Timestamp
```

### `ISyncBackendClient.PushAsync`

```csharp
SyncResult resp = await backend.PushAsync(await queue.GetPendingAsync());
foreach (var id in resp.Confirmed) await queue.MarkConfirmedAsync(id);
```

### `IConflictReporter.PublishAsync`

```csharp
if (result.Conflicts.Count > 0)
    await conflictReporter.PublishAsync(result.Conflicts);
```

### `IConnectivityMonitor.ConnectivityRestored`

```csharp
monitor.ConnectivityRestored += async (_, _) =>
    await engine.SynchronizeAsync(new SyncOptions(batchSize: 50));
```

---

## Referencias cruzadas

- [contratos-abstractions-sync_v1.0.md](../05_arquitectura_tecnica/contratos-abstractions-sync_v1.0.md) — fuente normativa con paridad uno a uno (05).
- [ADR-06-conflictos-last-write-wins-override-manual_v1.0.md](../05_arquitectura_tecnica/adrs/ADR-06-conflictos-last-write-wins-override-manual_v1.0.md) — semántica de conflictos y last-write-wins (05).
- [ADR-07-libreria-sincronizacion-github-packages_v1.0.md](../05_arquitectura_tecnica/adrs/ADR-07-libreria-sincronizacion-github-packages_v1.0.md) — versionado SemVer de la superficie (05).
- [CU-07-sincronizar-cambios-locales_v1.0.md](../02_especificacion_funcional/casos-de-uso/CU-07-sincronizar-cambios-locales_v1.0.md) — códigos de error del pipeline (02).
- [conceptos-fundamentales_v1.0.md](conceptos-fundamentales_v1.0.md) y [glosario-tecnico_v1.0.md](glosario-tecnico_v1.0.md) (10).

## Control de cambios

| Versión | Fecha | Cambio | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. Referencia de tipos públicos, métodos, eventos, excepciones y ejemplos, en paridad uno a uno con contratos-abstractions-sync_v1.0.md de 05. | AG-10 |
