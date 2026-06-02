using System.Net.Http.Json;
using GeoVial.Shared;

namespace GeoVial.Revision;

/// <summary>
/// Cliente de la API de revisión sobre mapa (US-21, CU-08): obtiene el mapa de revisión de un relevamiento
/// (marcadores con sus fotos y comentarios) desde <c>GET /api/v1/relevamientos/{id}/revision</c>, con filtro
/// opcional por etiquetas (US-23). Devuelve null si el relevamiento no existe o no se autoriza.
/// </summary>
public sealed class ClienteRevisionHttp
{
    private readonly HttpClient _http;

    public ClienteRevisionHttp(HttpClient http) => _http = http;

    public Task<RevisionRelevamientoDto?> ObtenerAsync(Guid relevamientoId, string? etiquetas = null, CancellationToken ct = default)
    {
        var ruta = $"api/v1/relevamientos/{relevamientoId}/revision";
        if (!string.IsNullOrWhiteSpace(etiquetas))
        {
            ruta += $"?etiquetas={Uri.EscapeDataString(etiquetas)}";
        }

        return _http.GetFromJsonAsync<RevisionRelevamientoDto>(ruta, ct);
    }
}
