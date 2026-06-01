using GeoVial.Application.Abstracciones;
using GeoVial.Domain;
using GeoVial.Shared;

namespace GeoVial.Application.Servicios;

/// <summary>
/// Servicio de acceso: inicio de sesión ROPC → JWT, configuración del método de seguridad,
/// habilitación del modo sin conexión, reingreso en terreno y refresh condicionado
/// (CU-02; US-04, US-05; RN-06; ADR-03).
/// </summary>
public sealed class AccesoService
{
    private readonly ICredencialRepository _credenciales;
    private readonly IUsuarioRepository _usuarios;
    private readonly IHasherClave _hasher;
    private readonly IServicioToken _tokens;
    private readonly IServicioAuditoria _auditoria;

    public AccesoService(
        ICredencialRepository credenciales,
        IUsuarioRepository usuarios,
        IHasherClave hasher,
        IServicioToken tokens,
        IServicioAuditoria auditoria)
    {
        _credenciales = credenciales;
        _usuarios = usuarios;
        _hasher = hasher;
        _tokens = tokens;
        _auditoria = auditoria;
    }

    /// <summary>Inicio de sesión con conexión (ROPC → JWT). Registra el acceso en auditoría (RN-07).</summary>
    public async Task<Resultado<TokenResponse>> IniciarSesionAsync(string nombreUsuario, string clave, CancellationToken ct = default)
    {
        var cred = await _credenciales.ObtenerPorNombreUsuarioAsync(nombreUsuario, ct);
        if (cred is null || !_hasher.Verificar(clave, cred.HashClave))
        {
            return Resultado<TokenResponse>.Fallo(CodigosError.CredencialesInvalidas);
        }

        var usuario = await _usuarios.ObtenerPorIdAsync(cred.UsuarioId, ct);
        if (usuario is null || !usuario.EstadoVigencia)
        {
            return Resultado<TokenResponse>.Fallo(CodigosError.CredencialesInvalidas);
        }

        await _auditoria.RegistrarAsync(usuario.UsuarioId, "ACCESO", "login", ct);
        return Resultado<TokenResponse>.Exito(EmitirTokens(usuario));
    }

    /// <summary>Configura el método de seguridad del teléfono durante un inicio con conexión (CU-02 §5.B, RN-06).</summary>
    public async Task<Resultado> ConfigurarMetodoSeguridadAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(usuarioId, ct);
        if (usuario is null)
        {
            return Resultado.Fallo(CodigosError.UsuarioInexistente);
        }

        usuario.ConfigurarMetodoSeguridad();
        await _usuarios.GuardarCambiosAsync(ct);
        return Resultado.Exito();
    }

    /// <summary>Habilita el modo sin conexión solo si el método de seguridad está configurado (RN-06, CU-02 CA-02).</summary>
    public async Task<Resultado> HabilitarOfflineAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(usuarioId, ct);
        if (usuario is null)
        {
            return Resultado.Fallo(CodigosError.UsuarioInexistente);
        }

        return usuario.MetodoSeguridadConfigurado
            ? Resultado.Exito()
            : Resultado.Fallo(CodigosError.OfflineNoHabilitado);
    }

    /// <summary>Reingreso en terreno con el método de seguridad del teléfono (US-05, CU-02 §5.A, CA-03).</summary>
    public async Task<Resultado<TokenResponse>> ReingresoAsync(string nombreUsuario, bool metodoSeguridadPresente, CancellationToken ct = default)
    {
        var cred = await _credenciales.ObtenerPorNombreUsuarioAsync(nombreUsuario, ct);
        if (cred is null)
        {
            return Resultado<TokenResponse>.Fallo(CodigosError.CredencialesInvalidas);
        }

        var usuario = await _usuarios.ObtenerPorIdAsync(cred.UsuarioId, ct);
        if (usuario is null || !usuario.EstadoVigencia)
        {
            return Resultado<TokenResponse>.Fallo(CodigosError.CredencialesInvalidas);
        }

        if (!metodoSeguridadPresente || !usuario.MetodoSeguridadConfigurado)
        {
            return Resultado<TokenResponse>.Fallo(CodigosError.ReingresoSinMetodoSeguridad);
        }

        await _auditoria.RegistrarAsync(usuario.UsuarioId, "REINGRESO", "terreno", ct);
        return Resultado<TokenResponse>.Exito(EmitirTokens(usuario));
    }

    /// <summary>Refresh del token condicionado al método de seguridad del teléfono para el agente (ADR-03).</summary>
    public async Task<Resultado<TokenResponse>> RefrescarTokenAsync(Guid usuarioId, bool metodoSeguridadPresente, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(usuarioId, ct);
        if (usuario is null || !usuario.EstadoVigencia)
        {
            return Resultado<TokenResponse>.Fallo(CodigosError.CredencialesInvalidas);
        }

        if (usuario.Rol == RolJerarquico.AgenteCampo && (!metodoSeguridadPresente || !usuario.MetodoSeguridadConfigurado))
        {
            return Resultado<TokenResponse>.Fallo(CodigosError.ReingresoSinMetodoSeguridad);
        }

        return Resultado<TokenResponse>.Exito(EmitirTokens(usuario));
    }

    private TokenResponse EmitirTokens(Usuario usuario)
    {
        var access = _tokens.GenerarAccessToken(usuario, out var expira);
        var refresh = _tokens.GenerarRefreshToken();
        return new TokenResponse(access, refresh, expira);
    }
}
