namespace GeoVial.Sync;

/// <summary>
/// Motor de sincronización (ISyncEngine, CU-07): lee los cambios pendientes de la cola en lotes, los sube
/// al backend en orden por marca temporal, marca como confirmados los que el backend acepta (vaciándolos
/// de la cola) y reporta los conflictos. Reanuda sin duplicar: lo no confirmado queda en la cola y la
/// idempotencia por ChangeId evita aplicar dos veces ante un reintento.
/// </summary>
public sealed class MotorSincronizacion : ISyncEngine
{
    private readonly IChangeQueue _cola;
    private readonly ISyncBackendClient _backend;
    private readonly IConflictReporter? _reporter;
    private readonly SyncOptions _opciones;

    public MotorSincronizacion(IChangeQueue cola, ISyncBackendClient backend, IConflictReporter? reporter = null, SyncOptions? opciones = null)
    {
        _cola = cola;
        _backend = backend;
        _reporter = reporter;
        _opciones = opciones ?? new SyncOptions();
    }

    public async Task<SyncResult> SynchronizeAsync(Guid relevamientoId, DateTime? since = null, CancellationToken ct = default)
    {
        var confirmados = new List<Guid>();
        var conflictos = new List<ConflictInfo>();
        var actualizaciones = new List<string>();

        while (true)
        {
            var lote = await _cola.ReadPendingAsync(_opciones.BatchSize, ct);
            if (lote.Count == 0)
            {
                break;
            }

            SyncResult resultado;
            try
            {
                resultado = await _backend.UploadAsync(relevamientoId, lote, since, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // CU-07 §5.A: corte de conexión; lo no confirmado queda en la cola para reanudar sin duplicar.
                throw new SyncInterruptedException("La sincronización se interrumpió; los cambios pendientes se conservan.", ex);
            }

            await _cola.MarkConfirmedAsync(resultado.Confirmed, ct);
            confirmados.AddRange(resultado.Confirmed);
            conflictos.AddRange(resultado.Conflicts);
            actualizaciones.AddRange(resultado.Updates);

            // Si el backend no confirmó nada de este lote, se corta para no ciclar indefinidamente.
            if (resultado.Confirmed.Count == 0)
            {
                break;
            }
        }

        if (conflictos.Count > 0 && _reporter is not null)
        {
            await _reporter.ReportAsync(conflictos, ct);
        }

        return new SyncResult(confirmados, conflictos, actualizaciones);
    }
}
