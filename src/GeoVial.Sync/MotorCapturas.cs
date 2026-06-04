namespace GeoVial.Sync;

/// <summary>
/// Motor de subida de capturas de campo encoladas sin conexión (US-16, CU-06/CU-07). Lee las capturas
/// pendientes de la cola en lotes (orden por marca temporal), sube cada una al backend (observación +
/// binario) y marca como subidas las que el backend acepta, vaciándolas de la cola. Reanuda sin perder:
/// lo no subido queda en la cola. Si la conexión se corta, conserva lo pendiente y señala la interrupción.
/// Es el camino de captura, paralelo a <see cref="MotorSincronizacion"/> (que sincroniza comentarios por /sync).
/// </summary>
public sealed class MotorCapturas
{
    private readonly IColaCapturas _cola;
    private readonly ICapturaBackendClient _backend;
    private readonly SyncOptions _opciones;

    public MotorCapturas(IColaCapturas cola, ICapturaBackendClient backend, SyncOptions? opciones = null)
    {
        _cola = cola;
        _backend = backend;
        _opciones = opciones ?? new SyncOptions();
    }

    /// <summary>Drena la cola de capturas subiéndolas al backend. Devuelve las capturas subidas con éxito.</summary>
    public async Task<ResultadoCapturas> SincronizarAsync(CancellationToken ct = default)
    {
        var subidas = new List<Guid>();

        while (true)
        {
            var lote = await _cola.LeerPendientesAsync(_opciones.BatchSize, ct);
            if (lote.Count == 0)
            {
                break;
            }

            var confirmadas = new List<Guid>();
            foreach (var captura in lote)
            {
                bool subida;
                try
                {
                    subida = await _backend.SubirAsync(captura, ct);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    // CU-07 §5.A: corte de conexión. Se marca lo ya subido y se conserva el resto para reanudar.
                    await _cola.MarcarSubidasAsync(confirmadas, ct);
                    throw new SyncInterruptedException("La subida de capturas se interrumpió; las pendientes se conservan.", ex);
                }

                if (!subida)
                {
                    // El backend rechazó la captura (no es un corte): se corta el lote para no ciclar; queda en la cola.
                    break;
                }

                confirmadas.Add(captura.CapturaId);
            }

            await _cola.MarcarSubidasAsync(confirmadas, ct);
            subidas.AddRange(confirmadas);

            // Si no se subió nada de este lote, se corta para no ciclar indefinidamente.
            if (confirmadas.Count == 0)
            {
                break;
            }
        }

        return new ResultadoCapturas(subidas);
    }
}
