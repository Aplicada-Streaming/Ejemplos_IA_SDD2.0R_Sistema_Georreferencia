using System.Net.Http.Json;
using GeoVial.Shared;

namespace GeoVial.Revision;

/// <summary>Resultado uniforme de una edición sobre el marcador: éxito o un mensaje de error.</summary>
public sealed record ResultadoEdicion(bool Exito, string Mensaje);

/// <summary>
/// Cliente de edición sobre el marcador desde el móvil (US-15, CU-09): agrega un comentario al marcador y
/// etiqueta sus fotos o comentarios, consumiendo los endpoints existentes. Valida localmente que el texto y
/// la etiqueta no estén vacíos antes de llamar al backend; los rechazos del backend (por ejemplo, relevamiento
/// cerrado → solo lectura) se reflejan como un resultado de error.
/// </summary>
public sealed class ClienteEdicionMarcador
{
    private readonly HttpClient _http;

    public ClienteEdicionMarcador(HttpClient http) => _http = http;

    public Task<ResultadoEdicion> AgregarComentarioAsync(Guid marcadorId, Guid? fotoId, string texto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return Task.FromResult(new ResultadoEdicion(false, "El comentario no puede estar vacío."));
        }

        return PostAsync($"api/v1/marcadores/{marcadorId}/comentarios", new AgregarComentarioRequest(fotoId, texto.Trim()), "Comentario agregado.", ct);
    }

    public Task<ResultadoEdicion> EtiquetarFotoAsync(Guid fotoId, string etiqueta, CancellationToken ct = default) =>
        EtiquetarAsync($"api/v1/fotos/{fotoId}/etiquetas", etiqueta, ct);

    public Task<ResultadoEdicion> EtiquetarComentarioAsync(Guid comentarioId, string etiqueta, CancellationToken ct = default) =>
        EtiquetarAsync($"api/v1/comentarios/{comentarioId}/etiquetas", etiqueta, ct);

    /// <summary>Quita una foto del marcador (US-15, CU-09 §5.A): DELETE al endpoint de la foto. El backend rechaza si el relevamiento está cerrado.</summary>
    public async Task<ResultadoEdicion> QuitarFotoAsync(Guid fotoId, CancellationToken ct = default)
    {
        var resp = await _http.DeleteAsync($"api/v1/fotos/{fotoId}", ct);
        return resp.IsSuccessStatusCode
            ? new ResultadoEdicion(true, "Foto quitada del marcador.")
            : new ResultadoEdicion(false, $"El backend rechazó la operación ({(int)resp.StatusCode}).");
    }

    private Task<ResultadoEdicion> EtiquetarAsync(string ruta, string etiqueta, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(etiqueta))
        {
            return Task.FromResult(new ResultadoEdicion(false, "La etiqueta no puede estar vacía."));
        }

        return PostAsync(ruta, new EtiquetarRequest(etiqueta.Trim()), "Etiqueta agregada.", ct);
    }

    private async Task<ResultadoEdicion> PostAsync<T>(string ruta, T cuerpo, string exito, CancellationToken ct)
    {
        var resp = await _http.PostAsJsonAsync(ruta, cuerpo, ct);
        return resp.IsSuccessStatusCode
            ? new ResultadoEdicion(true, exito)
            : new ResultadoEdicion(false, $"El backend rechazó la operación ({(int)resp.StatusCode}).");
    }
}
