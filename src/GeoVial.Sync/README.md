# GeoVial.Sync

Librería de **sincronización offline/online** extraída de GeoVial (EP-09, ADR-07). Permite a una app de
campo encolar cambios sin conexión y consolidarlos contra un backend cuando hay señal, con resolución de
conflictos por última escritura (RN-04) e idempotencia por identificador de cambio (RC-03).

La superficie pública (`Abstractions`) es **estable y versionada con SemVer 2.0.0**: todo cambio incompatible
bumpea MAJOR.

## Instalación

```bash
dotnet add package GeoVial.Sync --version <version> --source github
```

El feed es GitHub Packages del repositorio (canales **preview** y **stable**, ver
`estrategia-versionado`). La versión la calcula MinVer desde los tags Git.

## Superficie pública

- `IChangeQueue` — cola local de cambios pendientes (implementación incluida sobre SQLite: `ColaCambiosSqlite`).
- `ISyncEngine` — motor de sincronización (`MotorSincronizacion`): sube lo pendiente en orden, marca confirmados, reporta conflictos y reanuda sin duplicar.
- `ISyncBackendClient` — puerto hacia el backend (implementación REST `ClienteSyncHttp` incluida).
- `IConflictReporter`, `IConnectivityMonitor` — reporte de conflictos y disparo automático por conectividad.
- Tipos: `ChangeRecord`, `SyncResult`, `ConflictInfo`, `OperationType`, `ConflictKind`, `SyncOptions`.
- Captura offline: `ColectorOffline`, `ChangeRecordFactory`, `ObservacionCapturada`, `CoordinadorAutoSync`.

## Uso mínimo

```csharp
using GeoVial.Sync;

IChangeQueue cola = new ColaCambiosSqlite("Data Source=cola.db");
ISyncBackendClient backend = /* ClienteSyncHttp o un mock */;
ISyncEngine motor = new MotorSincronizacion(cola, backend);

// Encolar sin conexión
await cola.EnqueueAsync(ChangeRecordFactory.Comentario(observacion));

// Sincronizar al recuperar señal
SyncResult r = await motor.SynchronizeAsync(relevamientoId);
// r.Confirmed / r.Conflicts / r.Updates
```

## Demos

- `samples/01-sync-basico` — consola: alta local → sincronización → cola vacía contra un backend en memoria.
- `samples/02-sync-maui-demo` — demo MAUI autónoma con backend simulado y resolución básica de conflictos.

## Licencia

MIT.
