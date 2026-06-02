using GeoVial.Sync;

namespace GeoVial.SyncDemo;

/// <summary>
/// Resolución básica de conflictos para la demo (US-32 CA-02). Ofrece las dos decisiones mínimas que un
/// integrador necesita evaluar sobre la librería:
/// <list type="bullet">
/// <item><b>Mantener lo local</b> (RN-04, prevalece la última escritura): despeja el conflicto del recurso en
/// el backend, de modo que la próxima sincronización confirme el cambio local que había quedado pendiente.</item>
/// <item><b>Aceptar lo remoto</b>: descarta el cambio local del recurso, quitándolo de la cola sin subirlo.</item>
/// </list>
/// La resolución completa sobre el mapa (unificación de marcadores) pertenece a la web (US-26); esta es la
/// versión básica que la demo necesita para cerrar el ciclo de evaluación.
/// </summary>
public sealed class ResolutorConflictos
{
    private readonly BackendSimulado _backend;
    private readonly IChangeQueue _cola;

    public ResolutorConflictos(BackendSimulado backend, IChangeQueue cola)
    {
        _backend = backend;
        _cola = cola;
    }

    /// <summary>Mantiene el cambio local: el backend deja de marcar el recurso en conflicto (la próxima sync lo confirma).</summary>
    public void MantenerLocal(Guid recurso) => _backend.DespejarConflicto(recurso);

    /// <summary>Acepta lo remoto: descarta el cambio local indicado, retirándolo de la cola.</summary>
    public Task AceptarRemotoAsync(Guid changeId, CancellationToken ct = default) =>
        _cola.MarkConfirmedAsync(new[] { changeId }, ct);
}
