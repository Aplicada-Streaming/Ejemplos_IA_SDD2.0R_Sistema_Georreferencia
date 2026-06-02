using System.Text.Json;

namespace GeoVial.Sync;

/// <summary>Payload de un cambio de comentario; formato compartido entre el encolado y el cliente REST.</summary>
public sealed record PayloadComentario(Guid MarcadorId, Guid? FotoId, Guid AutorUsuarioId, string Texto);

/// <summary>Datos de una observación capturada en campo que se encolan como cambio para sincronizar (CU-06, US-16).</summary>
public sealed record ObservacionCapturada(
    Guid ComentarioId, Guid MarcadorId, Guid? FotoId, Guid AutorUsuarioId, string Texto, DateTime Momento);

/// <summary>Construye <see cref="ChangeRecord"/> con el formato de payload del contrato /sync (RC-03).</summary>
public static class ChangeRecordFactory
{
    internal static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static ChangeRecord Comentario(ObservacionCapturada obs, OperationType operacion = OperationType.Create) =>
        new(
            Guid.NewGuid(),
            operacion,
            "comentario",
            obs.ComentarioId,
            obs.Momento,
            JsonSerializer.Serialize(new PayloadComentario(obs.MarcadorId, obs.FotoId, obs.AutorUsuarioId, obs.Texto), Json));
}

/// <summary>
/// Recolector de observaciones sin conexión (US-16, CU-06): toma una observación capturada y la encola en
/// la cola local para sincronizar más tarde, conservándola sin pérdida (NFR de jornada completa). Si la
/// cola no tiene espacio, propaga <see cref="AlmacenamientoLocalInsuficienteException"/> (US-16 CA-03).
/// </summary>
public sealed class ColectorOffline
{
    private readonly IChangeQueue _cola;

    public ColectorOffline(IChangeQueue cola) => _cola = cola;

    public Task RecolectarAsync(ObservacionCapturada observacion, CancellationToken ct = default) =>
        _cola.EnqueueAsync(ChangeRecordFactory.Comentario(observacion), ct);
}
