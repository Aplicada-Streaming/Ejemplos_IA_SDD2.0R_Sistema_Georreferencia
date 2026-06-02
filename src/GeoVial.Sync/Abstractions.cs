namespace GeoVial.Sync;

// Superficie pública de la librería de sincronización (contratos-abstractions-sync, ADR-07).
// Estable y versionada con SemVer 2.0.0: un cambio incompatible de esta superficie bumpea MAJOR.

/// <summary>Tipo de operación de un cambio local (Create/Update).</summary>
public enum OperationType
{
    Create = 1,
    Update = 2,
}

/// <summary>Clase de conflicto reportado por el backend (RN-04 / RN-02).</summary>
public enum ConflictKind
{
    FieldConflict = 1,
    MarkersWithinRadius = 2,
}

/// <summary>
/// Un cambio local encolado. <see cref="ChangeId"/> es la clave de idempotencia (RC-03): un reintento con
/// el mismo identificador no aplica el cambio dos veces. <see cref="Timestamp"/> gobierna la consolidación
/// last-write-wins (RN-04). <see cref="Payload"/> lleva los datos de la entidad en JSON.
/// </summary>
public sealed record ChangeRecord(
    Guid ChangeId,
    OperationType OperationType,
    string Entity,
    Guid EntityRef,
    DateTime Timestamp,
    string Payload);

/// <summary>Conflicto detectado por el backend, expuesto para resolución posterior (CU-12).</summary>
public sealed record ConflictInfo(Guid ConflictId, ConflictKind Kind, IReadOnlyList<string> InvolvedResources);

/// <summary>Resultado de una sincronización: cambios confirmados, conflictos y actualizaciones bajadas.</summary>
public sealed record SyncResult(
    IReadOnlyList<Guid> Confirmed,
    IReadOnlyList<ConflictInfo> Conflicts,
    IReadOnlyList<string> Updates)
{
    public static SyncResult Empty { get; } = new(Array.Empty<Guid>(), Array.Empty<ConflictInfo>(), Array.Empty<string>());
}

/// <summary>Opciones de sincronización.</summary>
public sealed record SyncOptions(int BatchSize = 50);

/// <summary>
/// Cola local de cambios pendientes de sincronizar (CU-06, US-17). Encola un cambio, lee los pendientes
/// ordenados por marca temporal, marca confirmado lo sincronizado y reporta cuántos quedan. Idempotente
/// por <see cref="ChangeRecord.ChangeId"/>.
/// </summary>
public interface IChangeQueue
{
    Task EnqueueAsync(ChangeRecord change, CancellationToken ct = default);
    Task<IReadOnlyList<ChangeRecord>> ReadPendingAsync(int max, CancellationToken ct = default);
    Task MarkConfirmedAsync(IEnumerable<Guid> changeIds, CancellationToken ct = default);
    Task<int> PendingCountAsync(CancellationToken ct = default);
}

/// <summary>Puerto hacia el backend de sincronización (la implementación REST contra /sync vive fuera de Abstractions).</summary>
public interface ISyncBackendClient
{
    Task<SyncResult> UploadAsync(Guid relevamientoId, IReadOnlyList<ChangeRecord> changes, DateTime? since, CancellationToken ct = default);
}

/// <summary>Expone los conflictos detectados (last-write-wins y marcadores en un mismo radio) para resolución posterior.</summary>
public interface IConflictReporter
{
    Task ReportAsync(IReadOnlyList<ConflictInfo> conflicts, CancellationToken ct = default);
}

/// <summary>Notifica la recuperación de conexión para disparar la sincronización automática (US-19).</summary>
public interface IConnectivityMonitor
{
    bool IsOnline { get; }

    event EventHandler ConnectivityRestored;
}

/// <summary>
/// Orquesta el pipeline de sincronización: sube los cambios encolados en orden, marca los confirmados,
/// reporta los conflictos y reanuda sin duplicar (idempotencia por ChangeId, CU-07).
/// </summary>
public interface ISyncEngine
{
    Task<SyncResult> SynchronizeAsync(Guid relevamientoId, DateTime? since = null, CancellationToken ct = default);
}

/// <summary>No se pudo aplicar la última escritura al consolidar (CONSOLIDACION_INVALIDA, RN-04).</summary>
public sealed class ConsolidationException : Exception
{
    public ConsolidationException(string message) : base(message)
    {
    }
}

/// <summary>Un recurso consolidado por última escritura no quedó marcado como conflicto (CONFLICTO_NO_MARCADO, RN-04).</summary>
public sealed class ConflictNotMarkedException : Exception
{
    public ConflictNotMarkedException(string message) : base(message)
    {
    }
}

/// <summary>Se perdió la conexión durante la sincronización; los cambios no confirmados se conservan (CU-07 §5.A).</summary>
public sealed class SyncInterruptedException : Exception
{
    public SyncInterruptedException(string message, Exception inner) : base(message, inner)
    {
    }
}

/// <summary>No hay espacio de almacenamiento local para encolar el cambio (ALMACENAMIENTO_LOCAL_INSUFICIENTE, US-16 CA-03).</summary>
public sealed class AlmacenamientoLocalInsuficienteException : Exception
{
    public AlmacenamientoLocalInsuficienteException(string message, Exception? inner = null) : base(message, inner)
    {
    }
}
