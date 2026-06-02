namespace GeoVial.Sync;

/// <summary>
/// Coordina la sincronización automática al recuperar conexión (US-19, CU-07 §1). Se suscribe al monitor
/// de conectividad; cuando se restaura la señal y hay un relevamiento activo, dispara la sincronización por
/// el motor. Si había cambios pendientes lo notifica (<see cref="CambiosPendientesDetectados"/>); si no,
/// la sincronización solo baja actualizaciones sin subir nada.
/// </summary>
public sealed class CoordinadorAutoSync : IDisposable
{
    private readonly IConnectivityMonitor _conectividad;
    private readonly ISyncEngine _motor;
    private readonly IChangeQueue _cola;

    public CoordinadorAutoSync(IConnectivityMonitor conectividad, ISyncEngine motor, IChangeQueue cola)
    {
        _conectividad = conectividad;
        _motor = motor;
        _cola = cola;
        _conectividad.ConnectivityRestored += AlRecuperarConexion;
    }

    /// <summary>Relevamiento que se sincroniza automáticamente; lo fija la app al abrir un relevamiento.</summary>
    public Guid? RelevamientoActivo { get; set; }

    /// <summary>Se dispara con la cantidad de cambios que había pendientes al recuperar conexión.</summary>
    public event EventHandler<int>? CambiosPendientesDetectados;

    /// <summary>Sincroniza si hay conexión y un relevamiento activo. Núcleo testeable del disparo automático.</summary>
    public async Task<SyncResult> SincronizarSiCorrespondeAsync(CancellationToken ct = default)
    {
        if (!_conectividad.IsOnline || RelevamientoActivo is not { } relevamiento)
        {
            return SyncResult.Empty;
        }

        var pendientes = await _cola.PendingCountAsync(ct);
        if (pendientes > 0)
        {
            CambiosPendientesDetectados?.Invoke(this, pendientes);
        }

        return await _motor.SynchronizeAsync(relevamiento, null, ct);
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
