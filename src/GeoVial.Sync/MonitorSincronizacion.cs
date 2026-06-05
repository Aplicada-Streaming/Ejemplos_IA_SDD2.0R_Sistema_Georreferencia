namespace GeoVial.Sync;

/// <summary>Estado de sincronización que ve el agente (F-M-?? / acción de retro S45-S47).</summary>
public enum EstadoSync
{
    /// <summary>No hay nada pendiente y hay conexión: el trabajo está a salvo en el backend.</summary>
    AlDia,

    /// <summary>Hay conexión pero quedan cambios/capturas por subir.</summary>
    Pendiente,

    /// <summary>Una sincronización está en curso.</summary>
    Sincronizando,

    /// <summary>Sin conexión: lo capturado queda en cola local hasta recuperar señal (S42/S45).</summary>
    SinConexion,

    /// <summary>El último intento con conexión falló y aún quedan pendientes (se reintentará).</summary>
    Error,
}

/// <summary>
/// Resumen del estado de sincronización para mostrar al agente: estado, cantidad de pendientes y un texto
/// apto para la UI. El cálculo es una función pura (testeable en el gate) sobre conectividad + pendientes +
/// si hay una sync en curso + si el último intento falló. Núcleo del indicador de sincronización (S48).
/// </summary>
public sealed record ResumenSincronizacion(EstadoSync Estado, int Pendientes, string Texto)
{
    /// <summary>Estado inicial neutro hasta el primer refresco.</summary>
    public static readonly ResumenSincronizacion Inicial = new(EstadoSync.AlDia, 0, "Todo sincronizado");

    /// <summary>
    /// Deriva el resumen. Prioridad: sincronizando &gt; sin conexión &gt; error (con pendientes) &gt; pendiente &gt; al día.
    /// Un valor negativo de <paramref name="pendientes"/> se normaliza a 0 (defensivo).
    /// </summary>
    public static ResumenSincronizacion Calcular(bool online, bool sincronizando, int pendientes, bool huboError = false)
    {
        if (pendientes < 0)
        {
            pendientes = 0;
        }

        if (sincronizando)
        {
            return new ResumenSincronizacion(EstadoSync.Sincronizando, pendientes, "Sincronizando…");
        }

        if (!online)
        {
            return new ResumenSincronizacion(EstadoSync.SinConexion, pendientes,
                pendientes > 0 ? $"Sin conexión · {Cuenta(pendientes)} sin sincronizar" : "Sin conexión");
        }

        if (huboError && pendientes > 0)
        {
            return new ResumenSincronizacion(EstadoSync.Error, pendientes,
                $"No se pudo sincronizar · {Cuenta(pendientes)} (se reintentará)");
        }

        if (pendientes > 0)
        {
            return new ResumenSincronizacion(EstadoSync.Pendiente, pendientes,
                $"{Cuenta(pendientes)} por sincronizar");
        }

        return new ResumenSincronizacion(EstadoSync.AlDia, 0, "Todo sincronizado");
    }

    private static string Cuenta(int n) => n == 1 ? "1 pendiente" : $"{n} pendientes";
}

/// <summary>
/// Mantiene el estado de sincronización vivo a partir de las fuentes reales (cola de comentarios S11, cola de
/// capturas S42 y monitor de conectividad) y notifica cuando cambia, para que la UI muestre el indicador sin
/// recalcular a mano. El cómputo se delega en <see cref="ResumenSincronizacion.Calcular"/> (gate). El cableado
/// de cuándo refrescar (al aparecer la pantalla, al capturar, alrededor de una sync) vive en la app móvil.
/// </summary>
public sealed class MonitorSincronizacion
{
    private readonly IConnectivityMonitor _conectividad;
    private readonly IChangeQueue _cola;
    private readonly IColaCapturas _capturas;

    public MonitorSincronizacion(IConnectivityMonitor conectividad, IChangeQueue cola, IColaCapturas capturas)
    {
        _conectividad = conectividad;
        _cola = cola;
        _capturas = capturas;
    }

    /// <summary>Último resumen calculado; arranca neutro hasta el primer <see cref="RefrescarAsync"/>.</summary>
    public ResumenSincronizacion Actual { get; private set; } = ResumenSincronizacion.Inicial;

    /// <summary>Se dispara cada vez que el resumen cambia (la UI se suscribe para actualizar el indicador).</summary>
    public event EventHandler<ResumenSincronizacion>? Cambiado;

    /// <summary>Marca que hay una sincronización en curso, conservando el conteo de pendientes conocido.</summary>
    public void MarcarSincronizando() =>
        Publicar(ResumenSincronizacion.Calcular(_conectividad.IsOnline, sincronizando: true, Actual.Pendientes));

    /// <summary>
    /// Relee los pendientes (comentarios + capturas) y la conectividad y recalcula el resumen.
    /// <paramref name="huboError"/> marca que el último intento con conexión falló (se reintentará).
    /// </summary>
    public async Task<ResumenSincronizacion> RefrescarAsync(bool huboError = false, CancellationToken ct = default)
    {
        var pendientesComentarios = await _cola.PendingCountAsync(ct);
        var pendientesCapturas = await _capturas.PendientesAsync(ct);
        var resumen = ResumenSincronizacion.Calcular(
            _conectividad.IsOnline, sincronizando: false, pendientesComentarios + pendientesCapturas, huboError);
        Publicar(resumen);
        return resumen;
    }

    private void Publicar(ResumenSincronizacion resumen)
    {
        Actual = resumen;
        Cambiado?.Invoke(this, resumen);
    }
}
