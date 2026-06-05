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
/// <summary>
/// Almacén seguro del token de sesión para que sobreviva a que el SO mate el proceso (p. ej. al abrir la
/// cámara): si no se persiste, al volver la app arranca sin sesión y rebota al login. La implementación móvil
/// usa SecureStorage (Keystore). Abstracción aquí para mantener el núcleo testeable y sin dependencia del SO.
/// </summary>
public interface IAlmacenTokenSesion
{
    Task GuardarAsync(string token, string usuario);
    Task<(string Token, string Usuario)?> LeerAsync();
    Task LimpiarAsync();
}

public sealed class ServicioSesion
{
    private readonly HttpClient _http;
    private readonly IAlmacenTokenSesion? _almacen;
    private string? _accessToken;

    public ServicioSesion(HttpClient http, IAlmacenTokenSesion? almacen = null)
    {
        _http = http;
        _almacen = almacen;
    }

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

        return await AsentarTokenAsync(login, usuario);
    }

    /// <summary>
    /// Reingreso en terreno (US-05, CU-02 §5.A, RN-06): re-autentica con el método de seguridad del teléfono
    /// presente, sin reescribir la clave. <paramref name="metodoPresente"/> lo informa el dispositivo. No lanza.
    /// </summary>
    public async Task<ResultadoSesion> ReingresarAsync(string? usuario, bool metodoPresente)
    {
        usuario = usuario?.Trim() ?? "";
        if (usuario.Length == 0)
        {
            return ResultadoSesion.Fallo("No hay un usuario recordado para el reingreso.");
        }

        if (!metodoPresente)
        {
            return ResultadoSesion.Fallo("Se necesita el método de seguridad del teléfono para reingresar.");
        }

        HttpResponseMessage reingreso;
        try
        {
            reingreso = await _http.PostAsJsonAsync(
                "api/v1/auth/reingreso", new { nombreUsuario = usuario, metodoSeguridadPresente = true });
        }
        catch (Exception ex)
        {
            return ResultadoSesion.Fallo($"No se pudo conectar con el backend: {ex.Message}");
        }

        if (!reingreso.IsSuccessStatusCode)
        {
            return ResultadoSesion.Fallo(reingreso.StatusCode == HttpStatusCode.Conflict
                ? "El método de seguridad no está configurado; reingresá con usuario y clave una vez con conexión."
                : "No se pudo reingresar. Iniciá sesión con usuario y clave.");
        }

        return await AsentarTokenAsync(reingreso, usuario);
    }

    /// <summary>
    /// Configura el método de seguridad del teléfono en el backend (US-04, CU-02 §5.B, RN-06): precondición
    /// para trabajar sin conexión. Requiere sesión iniciada. Devuelve <c>true</c> si quedó configurado.
    /// </summary>
    public async Task<bool> ConfigurarMetodoSeguridadAsync()
    {
        try
        {
            var resp = await _http.PostAsync("api/v1/auth/metodo-seguridad", content: null);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Habilita el modo sin conexión (RN-06): el backend lo permite sólo si el método de seguridad está
    /// configurado. Devuelve <c>true</c> si quedó habilitado (204); <c>false</c> si falta el método (409) o falla.
    /// </summary>
    public async Task<bool> HabilitarOfflineAsync()
    {
        try
        {
            var resp = await _http.PostAsync("api/v1/auth/offline", content: null);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task<ResultadoSesion> AsentarTokenAsync(HttpResponseMessage respuesta, string usuario)
    {
        var token = await respuesta.Content.ReadFromJsonAsync<TokenDto>();
        if (token is null || string.IsNullOrEmpty(token.AccessToken))
        {
            return ResultadoSesion.Fallo("El backend no devolvió un token válido.");
        }

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        _accessToken = token.AccessToken;
        Usuario = usuario;
        // Persiste la sesión para sobrevivir a que el SO mate el proceso (p. ej. al abrir la cámara): así, al
        // volver, la app la restaura en vez de rebotar al login (S54). Best-effort: no rompe el login si falla.
        if (_almacen is not null)
        {
            try { await _almacen.GuardarAsync(token.AccessToken, usuario); } catch { /* la sesión en memoria ya vale */ }
        }

        return ResultadoSesion.Ok();
    }

    /// <summary>
    /// Restaura la sesión persistida (S54): si hay un token guardado, lo asienta en el HttpClient compartido.
    /// Se llama al arrancar para que la app vuelva autenticada tras una recreación de proceso. Devuelve si quedó
    /// autenticada.
    /// </summary>
    public async Task<bool> RestaurarAsync()
    {
        if (Autenticado)
        {
            return true;
        }

        if (_almacen is null)
        {
            return false;
        }

        (string Token, string Usuario)? guardado;
        try { guardado = await _almacen.LeerAsync(); } catch { return false; }
        if (guardado is not { } g || string.IsNullOrEmpty(g.Token))
        {
            return false;
        }

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", g.Token);
        _accessToken = g.Token;
        Usuario = g.Usuario;
        return true;
    }

    /// <summary>Cierra la sesión y limpia el token del HttpClient compartido y del almacén seguro.</summary>
    public void Salir()
    {
        _http.DefaultRequestHeaders.Authorization = null;
        _accessToken = null;
        Usuario = null;
        RelevamientoActivoId = null;
        if (_almacen is not null)
        {
            _ = _almacen.LimpiarAsync(); // best-effort; la sesión en memoria ya quedó cerrada
        }
    }

    /// <summary>
    /// Lista los relevamientos asignados al agente (F-M-04/05). Consume el endpoint "asignados a mí", que ya
    /// filtra del lado del servidor por la asignación vigente (S47), así el dispositivo sólo recibe su trabajo.
    /// El <see cref="SelectorRelevamientos"/> conserva el marcado/orden y la resolución del activo. Requiere sesión.
    /// </summary>
    public async Task<IReadOnlyList<RelevamientoResumen>> ListarRelevamientosAsync()
    {
        var relevamientos = await _http.GetFromJsonAsync<List<RelevamientoDatos>>("api/v1/relevamientos/mios")
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
