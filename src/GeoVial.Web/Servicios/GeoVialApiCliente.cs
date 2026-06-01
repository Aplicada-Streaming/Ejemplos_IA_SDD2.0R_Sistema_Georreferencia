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

    private void Autorizar(HttpRequestMessage req)
    {
        if (_token is not null)
        {
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }
    }

    private sealed record ProblemaApi(string? Codigo, string? Title);
}
