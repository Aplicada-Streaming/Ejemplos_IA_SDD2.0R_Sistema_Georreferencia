using GeoVial.Application.Abstracciones;
using GeoVial.Domain;

namespace GeoVial.Application.Servicios;

/// <summary>
/// Provisión de credenciales de acceso (BT-23). El administrador establece el nombre de usuario y la
/// clave (hasheada con PBKDF2, ADR-03) de un usuario de su nivel administrable (RN-01), habilitando su
/// inicio de sesión. Audita la acción (RN-07).
/// </summary>
public sealed class ProvisionCredencialService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly ICredencialRepository _credenciales;
    private readonly IHasherClave _hasher;
    private readonly IServicioAuditoria _auditoria;

    public ProvisionCredencialService(
        IUsuarioRepository usuarios, ICredencialRepository credenciales, IHasherClave hasher, IServicioAuditoria auditoria)
    {
        _usuarios = usuarios;
        _credenciales = credenciales;
        _hasher = hasher;
        _auditoria = auditoria;
    }

    public async Task<Resultado> EstablecerCredencialAsync(
        Guid administradorId, Guid usuarioId, string nombreUsuario, string clave, CancellationToken ct = default)
    {
        var admin = await _usuarios.ObtenerPorIdAsync(administradorId, ct);
        var objetivo = await _usuarios.ObtenerPorIdAsync(usuarioId, ct);
        if (admin is null || objetivo is null)
        {
            return Resultado.Fallo(CodigosError.UsuarioInexistente);
        }

        if (!Jerarquia.PuedeAdministrar(admin, objetivo.Rol, objetivo.AreaId))
        {
            await _auditoria.RegistrarAsync(administradorId, "CREDENCIAL_RECHAZADA", $"usuario={usuarioId}", ct);
            return Resultado.Fallo(CodigosError.AccesoNoAutorizado);
        }

        if (string.IsNullOrWhiteSpace(nombreUsuario))
        {
            return Resultado.Fallo(CodigosError.NombreUsuarioRequerido);
        }

        if (string.IsNullOrWhiteSpace(clave))
        {
            return Resultado.Fallo(CodigosError.ClaveRequerida);
        }

        if (await _credenciales.ExisteNombreUsuarioAsync(nombreUsuario.Trim(), ct))
        {
            return Resultado.Fallo(CodigosError.NombreUsuarioEnUso);
        }

        var credencial = new Credencial(objetivo.UsuarioId, nombreUsuario.Trim(), _hasher.Hash(clave));

        if (!await _auditoria.RegistrarAsync(administradorId, "ESTABLECER_CREDENCIAL", $"usuario={usuarioId}", ct))
        {
            return Resultado.Fallo(CodigosError.AccionNoAuditada);
        }

        await _credenciales.AgregarAsync(credencial, ct);
        await _credenciales.GuardarCambiosAsync(ct);
        return Resultado.Exito();
    }
}
