namespace GeoVial.Sync;

/// <summary>
/// Coordina la sincronización automática al recuperar conexión (US-19, CU-07 §1). Se suscribe al monitor
/// de conectividad; cuando se restaura la señal y hay un relevamiento activo, dispara la sincronización por
/// el motor de comentarios y drena la cola de capturas (S42). Si había cambios pendientes lo notifica
/// (<see cref="CambiosPendientesDetectados"/>); si no, la sincronización solo baja actualizaciones.
/// El relevamiento activo se toma de <see cref="RelevamientoActivo"/> o, si no, de un proveedor (la sesión),
/// así no hace falta fijarlo a mano cada vez que el usuario elige un relevamiento (F-M-14).
/// </summary>
public sealed class CoordinadorAutoSync : IDisposable
{
    private readonly IConnectivityMonitor _conectividad;
    private readonly ISyncEngine _motor;
    private readonly IChangeQueue _cola;
    private readonly MotorCapturas? _capturas;
    private readonly Func<Guid?>? _proveedorRelevamiento;

    public CoordinadorAutoSync(
        IConnectivityMonitor conectividad,
        ISyncEngine motor,
        IChangeQueue cola,
        MotorCapturas? capturas = null,
        Func<Guid?>? proveedorRelevamiento = null)
    {
        _conectividad = conectividad;
        _motor = motor;
        _cola = cola;
        _capturas = capturas;
        _proveedorRelevamiento = proveedorRelevamiento;
        _conectividad.ConnectivityRestored += AlRecuperarConexion;
    }

    /// <summary>Relevamiento que se sincroniza automáticamente; si es <c>null</c> se usa el del proveedor (la sesión).</summary>
    public Guid? RelevamientoActivo { get; set; }

    /// <summary>Se dispara con la cantidad de cambios que había pendientes al recuperar conexión.</summary>
    public event EventHandler<int>? CambiosPendientesDetectados;

    /// <summary>
    /// Se dispara al terminar una sincronización (con éxito o con error), con un resumen del resultado.
    /// Permite que la UI actualice el indicador en vivo y avise de conflictos sin que el agente toque la
    /// pantalla (S49: cierra la salvedad de S48, donde el indicador sólo se refrescaba en eventos de UI).
    /// </summary>
    public event EventHandler<ResultadoAutoSync>? SincronizacionCompletada;

    /// <summary>Sincroniza si hay conexión y un relevamiento activo. Núcleo testeable del disparo automático.</summary>
    public async Task<SyncResult> SincronizarSiCorrespondeAsync(CancellationToken ct = default)
    {
        var relevamiento = RelevamientoActivo ?? _proveedorRelevamiento?.Invoke();
        if (!_conectividad.IsOnline || relevamiento is not { } id)
        {
            return SyncResult.Empty;
        }

        var pendientes = await _cola.PendingCountAsync(ct);
        if (pendientes > 0)
        {
            CambiosPendientesDetectados?.Invoke(this, pendientes);
        }

        var capturasSubidas = 0;
        try
        {
            // F-M-14: al recuperar señal, también se suben las capturas encoladas sin conexión (S42).
            if (_capturas is not null)
            {
                capturasSubidas = (await _capturas.SincronizarAsync(ct)).Subidas.Count;
            }

            var resultado = await _motor.SynchronizeAsync(id, null, ct);
            SincronizacionCompletada?.Invoke(this,
                new ResultadoAutoSync(resultado.Confirmed.Count, resultado.Conflicts.Count, capturasSubidas, HuboError: false));
            return resultado;
        }
        catch
        {
            // Un corte a mitad (capturas o comentarios) se reporta como error: lo subido hasta acá queda contado.
            SincronizacionCompletada?.Invoke(this, new ResultadoAutoSync(0, 0, capturasSubidas, HuboError: true));
            throw;
        }
    }

    private async void AlRecuperarConexion(object? sender, EventArgs e)
    {
        try
        {
            await SincronizarSiCorrespondeAsync();
        }
        catch (Exception)
        {
            // Una sincronización fallida no debe tumbar la app; se reintenta en la próxima recuperación de señal.
        }
    }

    public void Dispose() => _conectividad.ConnectivityRestored -= AlRecuperarConexion;
}

/// <summary>
/// Resumen del resultado de una sincronización automática (S49), para que la UI actualice el indicador y
/// avise de conflictos. <paramref name="Confirmados"/> y <paramref name="Conflictos"/> vienen del motor de
/// comentarios; <paramref name="CapturasSubidas"/>, del drenado de la cola de capturas (S42).
/// <paramref name="HuboError"/> indica que el intento se interrumpió (se reintentará en la próxima señal).
/// </summary>
public sealed record ResultadoAutoSync(int Confirmados, int Conflictos, int CapturasSubidas, bool HuboError);
