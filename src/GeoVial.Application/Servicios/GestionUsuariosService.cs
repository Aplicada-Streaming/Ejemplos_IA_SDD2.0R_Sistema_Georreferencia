using GeoVial.Application.Abstracciones;
using GeoVial.Domain;

namespace GeoVial.Application.Servicios;

/// <summary>
/// Servicio de aplicación de gestión de usuarios y jerarquía (CU-03; US-01, US-02; BT-02).
/// Módulo de servicios directos, no CQRS (PROJECT-README §3: el CQRS ligero queda para relevamientos).
/// </summary>
public sealed class GestionUsuariosService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IAreaRepository _areas;
    private readonly IServicioAuditoria _auditoria;

    public GestionUsuariosService(IUsuarioRepository usuarios, IAreaRepository areas, IServicioAuditoria auditoria)
    {
        _usuarios = usuarios;
        _areas = areas;
        _auditoria = auditoria;
    }

    /// <summary>Alta jerárquica de un usuario (CU-03 flujo principal; CA-01/02/03).</summary>
    public async Task<Resultado<Usuario>> AltaUsuarioAsync(
        Guid administradorId, string nombre, RolJerarquico rol, Guid? areaId, CancellationToken ct = default)
    {
        var admin = await _usuarios.ObtenerPorIdAsync(administradorId, ct);
        if (admin is null)
        {
            return Resultado<Usuario>.Fallo(CodigosError.UsuarioInexistente);
        }

        // RN-01 / BT-02: solo administra el nivel inmediato inferior (y su área si es jefe de área).
        if (!Jerarquia.PuedeAdministrar(admin, rol, areaId))
        {
            await _auditoria.RegistrarAsync(administradorId, "ALTA_USUARIO_RECHAZADA", $"rol={(int)rol}", ct);
            return Resultado<Usuario>.Fallo(CodigosError.AccesoNoAutorizado);
        }

        // CU-03 §3: jefe de área y agente quedan asociados a un área existente.
        if (rol is RolJerarquico.JefeArea or RolJerarquico.AgenteCampo)
        {
            if (areaId is null)
            {
                return Resultado<Usuario>.Fallo(CodigosError.AreaRequerida);
            }

            if (!await _areas.ExisteAsync(areaId.Value, ct))
            {
                return Resultado<Usuario>.Fallo(CodigosError.AreaInexistente);
            }
        }

        var creado = Usuario.Crear(nombre, rol, areaId);
        if (!creado.EsExito)
        {
            return creado;
        }

        var usuario = creado.Valor!;

        // RN-07: la acción administrativa debe quedar auditada; si no se puede, se rechaza (ACCION_NO_AUDITADA).
        if (!await _auditoria.RegistrarAsync(administradorId, "ALTA_USUARIO", $"usuario={usuario.UsuarioId}", ct))
        {
            return Resultado<Usuario>.Fallo(CodigosError.AccionNoAuditada);
        }

        await _usuarios.AgregarAsync(usuario, ct);
        await _usuarios.GuardarCambiosAsync(ct);
        return Resultado<Usuario>.Exito(usuario);
    }

    /// <summary>Baja lógica jerárquica (CU-03 §5.A): conserva datos e información producida (RN-08).</summary>
    public async Task<Resultado> BajaUsuarioAsync(Guid administradorId, Guid objetivoId, CancellationToken ct = default)
    {
        var admin = await _usuarios.ObtenerPorIdAsync(administradorId, ct);
        var objetivo = await _usuarios.ObtenerPorIdAsync(objetivoId, ct);
        if (admin is null || objetivo is null)
        {
            return Resultado.Fallo(CodigosError.UsuarioInexistente);
        }

        if (!Jerarquia.PuedeAdministrar(admin, objetivo.Rol, objetivo.AreaId))
        {
            await _auditoria.RegistrarAsync(administradorId, "BAJA_USUARIO_RECHAZADA", $"usuario={objetivoId}", ct);
            return Resultado.Fallo(CodigosError.AccesoNoAutorizado);
        }

        objetivo.DarDeBaja();

        if (!await _auditoria.RegistrarAsync(administradorId, "BAJA_USUARIO", $"usuario={objetivoId}", ct))
        {
            return Resultado.Fallo(CodigosError.AccionNoAuditada);
        }

        await _usuarios.GuardarCambiosAsync(ct);
        return Resultado.Exito();
    }

    /// <summary>Asociar un usuario a su área (US-02).</summary>
    public async Task<Resultado> AsociarAreaAsync(Guid administradorId, Guid usuarioId, Guid areaId, CancellationToken ct = default)
    {
        var admin = await _usuarios.ObtenerPorIdAsync(administradorId, ct);
        var objetivo = await _usuarios.ObtenerPorIdAsync(usuarioId, ct);
        if (admin is null || objetivo is null)
        {
            return Resultado.Fallo(CodigosError.UsuarioInexistente);
        }

        if (!Jerarquia.PuedeAdministrar(admin, objetivo.Rol, areaId))
        {
            await _auditoria.RegistrarAsync(administradorId, "ASOCIAR_AREA_RECHAZADA", $"usuario={usuarioId}", ct);
            return Resultado.Fallo(CodigosError.AccesoNoAutorizado);
        }

        if (!await _areas.ExisteAsync(areaId, ct))
        {
            return Resultado.Fallo(CodigosError.AreaInexistente);
        }

        objetivo.AsociarArea(areaId);

        if (!await _auditoria.RegistrarAsync(administradorId, "ASOCIAR_AREA", $"usuario={usuarioId};area={areaId}", ct))
        {
            return Resultado.Fallo(CodigosError.AccionNoAuditada);
        }

        await _usuarios.GuardarCambiosAsync(ct);
        return Resultado.Exito();
    }

    /// <summary>Lista los usuarios que el solicitante puede ver según rol y área (US-31, RN-01/RN-08).</summary>
    public async Task<IReadOnlyList<Usuario>> ListarVisiblesAsync(Guid solicitanteId, CancellationToken ct = default)
    {
        var solicitante = await _usuarios.ObtenerPorIdAsync(solicitanteId, ct);
        if (solicitante is null)
        {
            return Array.Empty<Usuario>();
        }

        var todos = await _usuarios.ListarTodosAsync(ct);
        return todos.Where(u => Autorizacion.PuedeAccederDatoPersonal(solicitante, u)).ToList();
    }
}
