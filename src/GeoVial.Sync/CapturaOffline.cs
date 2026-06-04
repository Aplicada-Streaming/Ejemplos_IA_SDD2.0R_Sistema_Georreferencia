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

/// <summary>
/// Una captura de campo (foto + coordenada) tomada sin conexión y pendiente de subir (US-16, CU-06).
/// A diferencia de un <see cref="ChangeRecord"/> (sólo JSON), lleva el binario de la foto, así que se
/// encola y se sube por su propio camino (observación + binario) en vez de por /sync.
/// <see cref="CapturaId"/> es la clave de idempotencia local (re-encolar no duplica).
/// </summary>
public sealed record CapturaPendiente(
    Guid CapturaId,
    Guid RelevamientoId,
    string ReferenciaArchivo,
    decimal? LatitudExif,
    decimal? LongitudExif,
    byte[] Foto,
    DateTime Momento);

/// <summary>
/// Cola local de capturas de campo pendientes de subir (US-16, CU-06; ADR-05). Conserva la foto y la
/// coordenada hasta que la captura se sube con éxito. Idempotente por <see cref="CapturaPendiente.CapturaId"/>.
/// </summary>
public interface IColaCapturas
{
    Task EncolarAsync(CapturaPendiente captura, CancellationToken ct = default);
    Task<IReadOnlyList<CapturaPendiente>> LeerPendientesAsync(int max, CancellationToken ct = default);
    Task MarcarSubidasAsync(IEnumerable<Guid> capturaIds, CancellationToken ct = default);
    Task<int> PendientesAsync(CancellationToken ct = default);
}

/// <summary>
/// Puerto hacia el backend para subir una captura completa: crea la observación y sube el binario de la
/// foto (POST observación + POST contenido). Devuelve <c>true</c> si la captura quedó subida; <c>false</c>
/// si el backend la rechazó (p. ej. 4xx, no reintentar). Lanza ante un corte de conexión (se reintenta).
/// La implementación REST vive fuera de Abstractions (ADR-07).
/// </summary>
public interface ICapturaBackendClient
{
    Task<bool> SubirAsync(CapturaPendiente captura, CancellationToken ct = default);
}

/// <summary>Resultado de drenar la cola de capturas: las capturas subidas con éxito.</summary>
public sealed record ResultadoCapturas(IReadOnlyList<Guid> Subidas)
{
    public static ResultadoCapturas Vacio { get; } = new(Array.Empty<Guid>());
}
