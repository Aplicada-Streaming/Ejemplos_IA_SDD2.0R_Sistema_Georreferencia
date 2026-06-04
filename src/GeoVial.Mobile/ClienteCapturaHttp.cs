using System.Net.Http.Json;
using GeoVial.CapturaCampo;
using GeoVial.Shared;
using GeoVial.Sync;

namespace GeoVial.Mobile;

/// <summary>
/// Implementación REST de <see cref="ICapturaBackendClient"/> (US-16, CU-06): sube una captura encolada
/// creando la observación (POST observaciones) y subiendo el binario de la foto (POST contenido). Reusa el
/// <see cref="HttpClient"/> compartido (con el token de sesión). Devuelve <c>false</c> ante un 4xx (el
/// backend la rechaza; no reintentar); lanza ante red/5xx para que el motor la conserve y reintente.
/// </summary>
public sealed class ClienteCapturaHttp : ICapturaBackendClient
{
    private readonly HttpClient _http;

    public ClienteCapturaHttp(HttpClient http) => _http = http;

    public async Task<bool> SubirAsync(CapturaPendiente captura, CancellationToken ct = default)
    {
        var peticion = new CapturarObservacionRequest(captura.ReferenciaArchivo, captura.LatitudExif, captura.LongitudExif);
        var resp = await _http.PostAsJsonAsync($"api/v1/relevamientos/{captura.RelevamientoId}/observaciones", peticion, ct);

        if ((int)resp.StatusCode is >= 400 and < 500)
        {
            return false; // 4xx: el backend rechaza la captura; no tiene sentido reintentarla.
        }

        resp.EnsureSuccessStatusCode(); // 5xx u otros: lanza → el motor la conserva y reintenta.

        var capt = await resp.Content.ReadFromJsonAsync<CapturaResponse>(cancellationToken: ct);
        if (capt is not null)
        {
            // BT-20 / ADR-08: subir el binario. Best-effort, igual que el flujo online: la observación ya
            // quedó creada, así que la captura se considera subida aunque el binario falle (se podrá resubir).
            using var contenido = ConstructorContenidoMultipart.Construir(captura.ReferenciaArchivo, captura.Foto);
            try
            {
                await _http.PostAsync($"api/v1/fotos/{capt.FotoId}/contenido", contenido, ct);
            }
            catch
            {
                // el binario puede subirse luego; no invalida la observación ya creada.
            }
        }

        return true;
    }
}
