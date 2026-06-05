using System.Net.Http.Json;
using GeoVial.Shared;

namespace GeoVial.Revision;

/// <summary>
/// Cliente REST para ubicar manualmente una observación de la bandeja (S51, CU-05): postea la coordenada
/// elegida al endpoint de ubicación. Reusa el <see cref="HttpClient"/> compartido (con el token de sesión).
/// Devuelve <c>true</c> si el backend la aceptó.
/// </summary>
public sealed class ClienteUbicacionManual
{
    private readonly HttpClient _http;

    public ClienteUbicacionManual(HttpClient http) => _http = http;

    public async Task<bool> UbicarAsync(Guid observacionId, decimal latitud, decimal longitud, CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync(
            $"api/v1/observaciones/{observacionId}/ubicacion", new UbicarManualRequest(latitud, longitud), ct);
        return resp.IsSuccessStatusCode;
    }
}
