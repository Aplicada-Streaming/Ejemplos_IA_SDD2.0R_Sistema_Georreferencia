using System.Net.Http.Headers;
using System.Net.Http.Json;
using GeoVial.Shared;

namespace GeoVial.Web.Servicios;

/// <summary>
/// Cliente del front web hacia la API REST de GeoVial (ADR-02). Mantiene el token de la sesión
/// dentro del circuito Blazor (servicio scoped). Consume el slice de acceso y gestión de usuarios.
/// </summary>
public sealed class GeoVialApiCliente
{
    private readonly HttpClient _http;
    private string? _token;

    public GeoVialApiCliente(HttpClient http) => _http = http;

    public bool Autenticado => _token is not null;

    public async Task<bool> LoginAsync(string nombreUsuario, string clave, CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync("api/v1/auth/login", new LoginRequest(nombreUsuario, clave), ct);
        if (!resp.IsSuccessStatusCode)
        {
            _token = null;
            return false;
        }

        var token = await resp.Content.ReadFromJsonAsync<TokenResponse>(ct);
        _token = token?.AccessToken;
        return _token is not null;
    }

    public void CerrarSesion() => _token = null;

    public async Task<IReadOnlyList<UsuarioDto>> ListarUsuariosAsync(CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "api/v1/usuarios");
        Autorizar(req);
        var resp = await _http.SendAsync(req, ct);
        if (!resp.IsSuccessStatusCode)
        {
            return Array.Empty<UsuarioDto>();
        }

        return await resp.Content.ReadFromJsonAsync<List<UsuarioDto>>(ct) ?? new List<UsuarioDto>();
    }

    public async Task<(bool Ok, string Mensaje)> AltaUsuarioAsync(string nombre, int rol, Guid? areaId, CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "api/v1/usuarios")
        {
            Content = JsonContent.Create(new AltaUsuarioRequest(nombre, rol, areaId)),
        };
        Autorizar(req);

        var resp = await _http.SendAsync(req, ct);
        if (resp.IsSuccessStatusCode)
        {
            return (true, "Usuario creado.");
        }

        var problema = await resp.Content.ReadFromJsonAsync<ProblemaApi>(ct);
        return (false, problema?.Codigo ?? $"Error {(int)resp.StatusCode}");
    }

    public async Task<(bool Ok, string Mensaje)> BajaUsuarioAsync(Guid usuarioId, CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/usuarios/{usuarioId}");
        Autorizar(req);

        var resp = await _http.SendAsync(req, ct);
        if (resp.IsSuccessStatusCode)
        {
            return (true, "Usuario dado de baja.");
        }

        var problema = await resp.Content.ReadFromJsonAsync<ProblemaApi>(ct);
        return (false, problema?.Codigo ?? $"Error {(int)resp.StatusCode}");
    }

    public async Task<(bool Ok, string Mensaje)> AsociarAreaAsync(Guid usuarioId, Guid areaId, CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Put, $"api/v1/usuarios/{usuarioId}/area")
        {
            Content = JsonContent.Create(new AsociarAreaRequest(areaId)),
        };
        Autorizar(req);

        var resp = await _http.SendAsync(req, ct);
        if (resp.IsSuccessStatusCode)
        {
            return (true, "Área asociada.");
        }

        var problema = await resp.Content.ReadFromJsonAsync<ProblemaApi>(ct);
        return (false, problema?.Codigo ?? $"Error {(int)resp.StatusCode}");
    }

    // --- Relevamientos (CU-01, CU-10) ---

    public async Task<IReadOnlyList<RelevamientoDto>> ListarRelevamientosAsync(CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "api/v1/relevamientos");
        Autorizar(req);
        var resp = await _http.SendAsync(req, ct);
        if (!resp.IsSuccessStatusCode)
        {
            return Array.Empty<RelevamientoDto>();
        }

        return await resp.Content.ReadFromJsonAsync<List<RelevamientoDto>>(ct) ?? new List<RelevamientoDto>();
    }

    public Task<(bool Ok, string Mensaje)> CrearRelevamientoAsync(string identificacion, decimal radio, CancellationToken ct = default) =>
        EnviarAsync(HttpMethod.Post, "api/v1/relevamientos", new CrearRelevamientoRequest(identificacion, radio), "Relevamiento creado.", ct);

    public Task<(bool Ok, string Mensaje)> TransicionarAsync(Guid id, int estadoDestino, CancellationToken ct = default) =>
        EnviarAsync(HttpMethod.Post, $"api/v1/relevamientos/{id}/transicion", new TransicionRequest(estadoDestino), "Estado actualizado.", ct);

    public Task<(bool Ok, string Mensaje)> ReabrirAsync(Guid id, CancellationToken ct = default) =>
        EnviarAsync<object?>(HttpMethod.Post, $"api/v1/relevamientos/{id}/reabrir", null, "Relevamiento reabierto.", ct);

    public Task<(bool Ok, string Mensaje)> AsignarAgentesAsync(Guid id, IReadOnlyList<Guid> agentes, CancellationToken ct = default) =>
        EnviarAsync(HttpMethod.Post, $"api/v1/relevamientos/{id}/agentes", new AsignarAgentesRequest(agentes), "Agentes asignados.", ct);

    // --- Captura y georreferenciación (CU-04, CU-05) ---

    public async Task<IReadOnlyList<ObservacionDto>> ListarObservacionesAsync(Guid relevamientoId, CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"api/v1/relevamientos/{relevamientoId}/observaciones");
        Autorizar(req);
        var resp = await _http.SendAsync(req, ct);
        if (!resp.IsSuccessStatusCode)
        {
            return Array.Empty<ObservacionDto>();
        }

        return await resp.Content.ReadFromJsonAsync<List<ObservacionDto>>(ct) ?? new List<ObservacionDto>();
    }

    public async Task<(bool Ok, string Mensaje)> CapturarObservacionAsync(
        Guid relevamientoId, string referencia, decimal? latitud, decimal? longitud, CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, $"api/v1/relevamientos/{relevamientoId}/observaciones")
        {
            Content = JsonContent.Create(new CapturarObservacionRequest(referencia, latitud, longitud)),
        };
        Autorizar(req);

        var resp = await _http.SendAsync(req, ct);
        if (resp.IsSuccessStatusCode)
        {
            var capt = await resp.Content.ReadFromJsonAsync<CapturaResponse>(ct);
            return (true, capt!.SinGeorreferenciar ? "Observación en bandeja sin georreferenciar." : "Observación georreferenciada.");
        }

        var problema = await resp.Content.ReadFromJsonAsync<ProblemaApi>(ct);
        return (false, problema?.Codigo ?? $"Error {(int)resp.StatusCode}");
    }

    public Task<(bool Ok, string Mensaje)> UbicarManualAsync(Guid observacionId, decimal latitud, decimal longitud, CancellationToken ct = default) =>
        EnviarAsync(HttpMethod.Post, $"api/v1/observaciones/{observacionId}/ubicacion", new UbicarManualRequest(latitud, longitud), "Observación ubicada.", ct);

    // --- Provisión de credenciales (BT-23) ---

    public Task<(bool Ok, string Mensaje)> EstablecerCredencialAsync(Guid usuarioId, string nombreUsuario, string clave, CancellationToken ct = default) =>
        EnviarAsync(HttpMethod.Post, $"api/v1/usuarios/{usuarioId}/credencial", new EstablecerCredencialRequest(nombreUsuario, clave), "Credencial establecida.", ct);

    // --- Revisión sobre mapa y gestión de marcador (CU-08, CU-09) ---

    public async Task<RevisionRelevamientoDto?> RevisarRelevamientoAsync(Guid relevamientoId, IReadOnlyList<string>? etiquetas = null, CancellationToken ct = default)
    {
        var ruta = $"api/v1/relevamientos/{relevamientoId}/revision";
        if (etiquetas is { Count: > 0 })
        {
            ruta += $"?etiquetas={Uri.EscapeDataString(string.Join(",", etiquetas))}";
        }

        using var req = new HttpRequestMessage(HttpMethod.Get, ruta);
        Autorizar(req);
        var resp = await _http.SendAsync(req, ct);
        return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<RevisionRelevamientoDto>(ct) : null;
    }

    public async Task<byte[]?> DescargarContenidoFotoAsync(Guid fotoId, CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"api/v1/fotos/{fotoId}/contenido");
        Autorizar(req);
        var resp = await _http.SendAsync(req, ct);
        return resp.IsSuccessStatusCode ? await resp.Content.ReadAsByteArrayAsync(ct) : null;
    }

    public Task<(bool Ok, string Mensaje)> AgregarComentarioAsync(Guid marcadorId, Guid? fotoId, string texto, CancellationToken ct = default) =>
        EnviarAsync(HttpMethod.Post, $"api/v1/marcadores/{marcadorId}/comentarios", new AgregarComentarioRequest(fotoId, texto), "Comentario agregado.", ct);

    public Task<(bool Ok, string Mensaje)> EtiquetarFotoAsync(Guid fotoId, string etiqueta, CancellationToken ct = default) =>
        EnviarAsync(HttpMethod.Post, $"api/v1/fotos/{fotoId}/etiquetas", new EtiquetarRequest(etiqueta), "Foto etiquetada.", ct);

    public async Task<(bool Ok, string Mensaje)> SubirContenidoFotoAsync(Guid fotoId, byte[] contenido, string nombre, CancellationToken ct = default)
    {
        using var formulario = new MultipartFormDataContent();
        var archivo = new ByteArrayContent(contenido);
        archivo.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        formulario.Add(archivo, "archivo", string.IsNullOrWhiteSpace(nombre) ? "foto.bin" : nombre);

        using var req = new HttpRequestMessage(HttpMethod.Post, $"api/v1/fotos/{fotoId}/contenido") { Content = formulario };
        Autorizar(req);

        var resp = await _http.SendAsync(req, ct);
        if (resp.IsSuccessStatusCode)
        {
            return (true, "Contenido de la foto subido.");
        }

        var problema = await resp.Content.ReadFromJsonAsync<ProblemaApi>(ct);
        return (false, problema?.Codigo ?? $"Error {(int)resp.StatusCode}");
    }

    // --- Detección y resolución de conflictos por radio (CU-11, CU-12) ---

    public async Task<IReadOnlyList<ConflictoPendienteDto>> ListarConflictosAsync(Guid relevamientoId, CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"api/v1/relevamientos/{relevamientoId}/conflictos");
        Autorizar(req);
        var resp = await _http.SendAsync(req, ct);
        if (!resp.IsSuccessStatusCode)
        {
            return Array.Empty<ConflictoPendienteDto>();
        }

        return await resp.Content.ReadFromJsonAsync<List<ConflictoPendienteDto>>(ct) ?? new List<ConflictoPendienteDto>();
    }

    public async Task<(bool Ok, string Mensaje)> DetectarConflictosAsync(Guid relevamientoId, CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, $"api/v1/relevamientos/{relevamientoId}/conflictos/deteccion");
        Autorizar(req);

        var resp = await _http.SendAsync(req, ct);
        if (resp.IsSuccessStatusCode)
        {
            var detectados = await resp.Content.ReadFromJsonAsync<List<ConflictoDetectadoDto>>(ct) ?? new List<ConflictoDetectadoDto>();
            return (true, $"Detección completada: {detectados.Count} conflicto(s) nuevo(s).");
        }

        var problema = await resp.Content.ReadFromJsonAsync<ProblemaApi>(ct);
        return (false, problema?.Codigo ?? $"Error {(int)resp.StatusCode}");
    }

    public Task<(bool Ok, string Mensaje)> AjustarRadioAsync(Guid relevamientoId, decimal radio, CancellationToken ct = default) =>
        EnviarAsync(HttpMethod.Put, $"api/v1/relevamientos/{relevamientoId}/radio", new AjustarRadioRequest(radio), "Radio ajustado.", ct);

    public Task<(bool Ok, string Mensaje)> ResolverConflictoAsync(Guid conflictoId, int decision, Guid? marcadorResultanteId, CancellationToken ct = default) =>
        EnviarAsync(HttpMethod.Post, $"api/v1/conflictos/{conflictoId}/resolucion",
            new ResolverConflictoRequest(decision, marcadorResultanteId),
            decision == 1 ? "Marcadores unificados." : "Marcadores mantenidos separados.", ct);

    // --- Exportación e importación del relevamiento completo (CU-08 §5.A/§5.B) ---

    public async Task<(byte[]? Contenido, string Nombre, string Mensaje)> ExportarRelevamientoAsync(Guid relevamientoId, CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"api/v1/relevamientos/{relevamientoId}/export");
        Autorizar(req);
        var resp = await _http.SendAsync(req, ct);
        if (!resp.IsSuccessStatusCode)
        {
            var problema = await resp.Content.ReadFromJsonAsync<ProblemaApi>(ct);
            return (null, string.Empty, problema?.Codigo ?? $"Error {(int)resp.StatusCode}");
        }

        var bytes = await resp.Content.ReadAsByteArrayAsync(ct);
        var nombre = resp.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? $"relevamiento-{relevamientoId}.zip";
        return (bytes, nombre, $"Relevamiento exportado ({bytes.Length} bytes).");
    }

    public async Task<(bool Ok, string Mensaje, Guid? Id)> ImportarRelevamientoAsync(byte[] contenido, string nombre, CancellationToken ct = default)
    {
        using var formulario = new MultipartFormDataContent();
        var archivo = new ByteArrayContent(contenido);
        archivo.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
        formulario.Add(archivo, "archivo", string.IsNullOrWhiteSpace(nombre) ? "relevamiento.zip" : nombre);

        using var req = new HttpRequestMessage(HttpMethod.Post, "api/v1/relevamientos/import") { Content = formulario };
        Autorizar(req);

        var resp = await _http.SendAsync(req, ct);
        if (resp.IsSuccessStatusCode)
        {
            var creado = await resp.Content.ReadFromJsonAsync<ImportadoApi>(ct);
            return (true, "Relevamiento importado.", creado?.RelevamientoId);
        }

        var problema = await resp.Content.ReadFromJsonAsync<ProblemaApi>(ct);
        return (false, problema?.Codigo ?? $"Error {(int)resp.StatusCode}", null);
    }

    private async Task<(bool Ok, string Mensaje)> EnviarAsync<TBody>(HttpMethod metodo, string ruta, TBody? cuerpo, string exito, CancellationToken ct)
    {
        using var req = new HttpRequestMessage(metodo, ruta);
        if (cuerpo is not null)
        {
            req.Content = JsonContent.Create(cuerpo);
        }

        Autorizar(req);
        var resp = await _http.SendAsync(req, ct);
        if (resp.IsSuccessStatusCode)
        {
            return (true, exito);
        }

        var problema = await resp.Content.ReadFromJsonAsync<ProblemaApi>(ct);
        return (false, problema?.Codigo ?? $"Error {(int)resp.StatusCode}");
    }

    private void Autorizar(HttpRequestMessage req)
    {
        if (_token is not null)
        {
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }
    }

    private sealed record ProblemaApi(string? Codigo, string? Title);

    private sealed record ImportadoApi(Guid RelevamientoId);
}
