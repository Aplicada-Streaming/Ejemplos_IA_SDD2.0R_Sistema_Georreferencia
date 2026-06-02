using System.Net.Http.Json;
using System.Text.Json;

namespace GeoVial.Sync;

/// <summary>
/// Implementación REST de <see cref="ISyncBackendClient"/> contra el endpoint del backend GeoVial
/// (POST /api/v1/relevamientos/{id}/sync, contratos-rest §3). Vive fuera de la superficie de Abstractions
/// (ADR-07). Para la entidad "comentario", deserializa el payload del cambio al contrato del backend.
/// </summary>
public sealed class ClienteSyncHttp : ISyncBackendClient
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;

    public ClienteSyncHttp(HttpClient http) => _http = http;

    public async Task<SyncResult> UploadAsync(Guid relevamientoId, IReadOnlyList<ChangeRecord> changes, DateTime? since, CancellationToken ct = default)
    {
        var cambios = new List<CambioDto>();
        foreach (var c in changes)
        {
            var payload = JsonSerializer.Deserialize<PayloadComentario>(c.Payload, Json)
                ?? throw new ConsolidationException($"Payload de cambio inválido para {c.ChangeId}.");
            cambios.Add(new CambioDto(
                c.ChangeId, (int)c.OperationType, c.EntityRef, payload.MarcadorId, payload.FotoId, payload.AutorUsuarioId, payload.Texto, c.Timestamp));
        }

        var respuesta = await _http.PostAsJsonAsync(
            $"api/v1/relevamientos/{relevamientoId}/sync", new SyncRequestDto(since, cambios), Json, ct);
        respuesta.EnsureSuccessStatusCode();

        var cuerpo = await respuesta.Content.ReadFromJsonAsync<SyncResponseDto>(Json, ct)
            ?? throw new ConsolidationException("Respuesta de sincronización vacía.");

        var conflictos = cuerpo.Conflictos
            .Select(x => new ConflictInfo(x.ConflictoSyncId, (ConflictKind)x.Tipo, x.RecursosInvolucrados.Split(';', StringSplitOptions.RemoveEmptyEntries)))
            .ToList();
        var actualizaciones = cuerpo.Actualizaciones.Select(a => a.ComentarioId.ToString()).ToList();

        return new SyncResult(cuerpo.Confirmados, conflictos, actualizaciones);
    }

    // Espejo del contrato REST de /sync (contratos-rest §4), local para no acoplar la librería a GeoVial.Shared.
    private sealed record SyncRequestDto(DateTime? Desde, IReadOnlyList<CambioDto> Cambios);

    private sealed record CambioDto(
        Guid CambioId, int Operacion, Guid ComentarioId, Guid MarcadorId, Guid? FotoId, Guid AutorUsuarioId, string Texto, DateTime MarcaTemporal);

    private sealed record SyncResponseDto(
        IReadOnlyList<Guid> Confirmados, IReadOnlyList<ConflictoDto> Conflictos, IReadOnlyList<ActualizacionDto> Actualizaciones);

    private sealed record ConflictoDto(Guid ConflictoSyncId, int Tipo, string RecursosInvolucrados);

    private sealed record ActualizacionDto(Guid ComentarioId, Guid MarcadorId, string Texto, DateTime MarcaTemporal);
}
