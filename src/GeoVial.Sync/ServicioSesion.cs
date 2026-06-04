using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GeoVial.Sync;

/// <summary>
/// Sesión única del cliente móvil (US-40). Autentica una sola vez contra el backend
/// y asienta el token <c>Bearer</c> en el <see cref="HttpClient"/> compartido (singleton),
/// de modo que todas las páginas reusan la misma sesión sin volver a loguearse.
/// Reemplaza el login hardcodeado (<c>raiz</c>/<c>GeoVial.Raiz.2026</c>) que cada página
/// hacía por su cuenta, que además impedía iniciar sesión con otro usuario.
/// </summary>
public sealed class ServicioSesion
{
    private readonly HttpClient _http;
    private string? _accessToken;

    public ServicioSesion(HttpClient http) => _http = http;

    /// <summary>Usuario con sesión iniciada; <c>null</c> si no hay sesión.</summary>
    public string? Usuario { get; private set; }

    /// <summary>Hay una sesión activa (token asentado en el HttpClient compartido).</summary>
    public bool Autenticado => _http.DefaultRequestHeaders.Authorization is not null;

    /// <summary>Id del usuario en sesión, leído del token (claim <c>sub</c>); <c>null</c> si no hay sesión.</summary>
    public Guid? UsuarioId => LectorTokenJwt.LeerUsuarioId(_accessToken);

    /// <summary>Relevamiento elegido por el usuario para trabajar (F-M-05); se recuerda para usarlo offline.</summary>
    public Guid? RelevamientoActivoId { get; private set; }

    /// <summary>Fija el relevamiento activo elegido por el usuario.</summary>
    public void SeleccionarRelevamiento(Guid relevamientoId) => RelevamientoActivoId = relevamientoId;

    /// <summary>
    /// Inicia sesión contra <c>/api/v1/auth/login</c> y asienta el token en el HttpClient
    /// compartido. No lanza: devuelve un <see cref="ResultadoSesion"/> con un mensaje
    /// apto para mostrar, así un fallo de red o de credenciales nunca tumba la app.
    /// </summary>
    public async Task<ResultadoSesion> IngresarAsync(string? usuario, string? clave)
    {
        usuario = usuario?.Trim() ?? "";
        if (usuario.Length == 0 || string.IsNullOrEmpty(clave))
        {
            return ResultadoSesion.Fallo("Ingresá usuario y clave.");
        }

        HttpResponseMessage login;
        try
        {
            login = await _http.PostAsJsonAsync("api/v1/auth/login", new { nombreUsuario = usuario, clave });
        }
        catch (Exception ex)
        {
            return ResultadoSesion.Fallo($"No se pudo conectar con el backend: {ex.Message}");
        }

        if (!login.IsSuccessStatusCode)
        {
            return ResultadoSesion.Fallo(login.StatusCode == HttpStatusCode.Unauthorized
                ? "Usuario o clave incorrectos."
                : $"El backend rechazó el login ({(int)login.StatusCode}).");
        }

        var token = await login.Content.ReadFromJsonAsync<TokenDto>();
        if (token is null || string.IsNullOrEmpty(token.AccessToken))
        {
            return ResultadoSesion.Fallo("El backend no devolvió un token válido.");
        }

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        _accessToken = token.AccessToken;
        Usuario = usuario;
        return ResultadoSesion.Ok();
    }

    /// <summary>Cierra la sesión y limpia el token del HttpClient compartido.</summary>
    public void Salir()
    {
        _http.DefaultRequestHeaders.Authorization = null;
        _accessToken = null;
        Usuario = null;
        RelevamientoActivoId = null;
    }

    /// <summary>
    /// Lista los relevamientos accesibles para el usuario, marcando los que tiene asignados (F-M-04) y
    /// ordenándolos (asignados primero). Requiere sesión iniciada.
    /// </summary>
    public async Task<IReadOnlyList<RelevamientoResumen>> ListarRelevamientosAsync()
    {
        var relevamientos = await _http.GetFromJsonAsync<List<RelevamientoDatos>>("api/v1/relevamientos")
            ?? new List<RelevamientoDatos>();
        return SelectorRelevamientos.Listar(relevamientos, UsuarioId);
    }

    /// <summary>
    /// Devuelve el relevamiento activo (F-M-05): si el usuario ya eligió uno, ese (sirve sin conexión);
    /// si no, consulta la lista y resuelve por defecto (primer asignado abierto, o el primero), recordándolo.
    /// <c>null</c> si no hay relevamientos.
    /// </summary>
    public async Task<Guid?> RelevamientoActivoAsync()
    {
        if (RelevamientoActivoId is { } yaElegido)
        {
            return yaElegido;
        }

        var lista = await ListarRelevamientosAsync();
        var activo = SelectorRelevamientos.Activo(lista, RelevamientoActivoId);
        if (activo is { } id)
        {
            RelevamientoActivoId = id;
        }

        return activo;
    }

    private sealed record TokenDto(string AccessToken);
}

/// <summary>Resultado de un intento de inicio de sesión: éxito + mensaje apto para la UI.</summary>
public readonly record struct ResultadoSesion(bool Exito, string Mensaje)
{
    public static ResultadoSesion Ok() => new(true, "");

    public static ResultadoSesion Fallo(string mensaje) => new(false, mensaje);
}
