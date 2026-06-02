using GeoVial.Sync;

namespace GeoVial.SyncDemo;

/// <summary>
/// Fachada de la demo autónoma (US-32): orquesta el alta de registros locales, la sincronización contra el
/// backend simulado y la resolución básica de conflictos, sobre la superficie pública de <c>GeoVial.Sync</c>.
/// La consumen tanto la cáscara MAUI como las pruebas de aceptación; no contiene lógica de UI.
/// </summary>
public sealed class CoordinadorDemo
{
    private readonly IChangeQueue _cola;
    private readonly ISyncEngine _motor;
    private readonly ResolutorConflictos _resolutor;
    private readonly Guid _relevamiento;
    private bool _yaSincronizo;

    public CoordinadorDemo(IChangeQueue cola, ISyncEngine motor, ResolutorConflictos resolutor, Guid relevamiento)
    {
        _cola = cola;
        _motor = motor;
        _resolutor = resolutor;
        _relevamiento = relevamiento;
    }

    /// <summary>Cantidad de cambios pendientes en la cola local (estado «pendiente» de la demo).</summary>
    public Task<int> PendientesAsync(CancellationToken ct = default) => _cola.PendingCountAsync(ct);

    /// <summary>
    /// Da de alta una observación local encolándola como cambio (US-32: alta de registros locales). Devuelve el
    /// <see cref="ChangeRecord"/> encolado para que la UI/los tests conozcan su <c>ChangeId</c> y su recurso.
    /// </summary>
    public async Task<ChangeRecord> CapturarAsync(ObservacionCapturada observacion, CancellationToken ct = default)
    {
        var cambio = ChangeRecordFactory.Comentario(observacion);
        await _cola.EnqueueAsync(cambio, ct);
        return cambio;
    }

    /// <summary>
    /// Sincroniza la cola contra el backend simulado. La primera vez sube sin filtro; en adelante envía un
    /// <c>since</c> para que el backend ofrezca también las actualizaciones acumuladas (US-19 CA-02).
    /// </summary>
    public Task<SyncResult> SincronizarAsync(CancellationToken ct = default)
    {
        DateTime? desde = _yaSincronizo ? DateTime.UnixEpoch : null;
        _yaSincronizo = true;
        return _motor.SynchronizeAsync(_relevamiento, desde, ct);
    }

    /// <summary>Resolución básica «mantener lo local»: el cambio local prevalece y la próxima sincronización lo confirma.</summary>
    public void ResolverManteniendoLocal(Guid recurso) => _resolutor.MantenerLocal(recurso);

    /// <summary>Resolución básica «aceptar lo remoto»: descarta el cambio local de la cola.</summary>
    public Task ResolverAceptandoRemotoAsync(Guid changeId, CancellationToken ct = default) =>
        _resolutor.AceptarRemotoAsync(changeId, ct);
}
