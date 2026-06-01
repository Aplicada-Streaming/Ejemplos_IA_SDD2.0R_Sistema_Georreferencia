using GeoVial.Application.Abstracciones;
using GeoVial.Application.Cqrs;
using GeoVial.Domain;

namespace GeoVial.Application.Relevamientos;

/// <summary>Carga y autorización común a los manejadores de relevamientos (RN-01).</summary>
internal static class AccesoRelevamiento
{
    public static async Task<(Relevamiento? Relevamiento, string? Error)> CargarAutorizadoAsync(
        IUsuarioRepository usuarios,
        IRelevamientoRepository relevamientos,
        Guid usuarioId,
        Guid relevamientoId,
        CancellationToken ct)
    {
        var usuario = await usuarios.ObtenerPorIdAsync(usuarioId, ct);
        if (usuario is null)
        {
            return (null, CodigosError.UsuarioInexistente);
        }

        var relevamiento = await relevamientos.ObtenerPorIdAsync(relevamientoId, ct);
        if (relevamiento is null)
        {
            return (null, CodigosError.RelevamientoInexistente);
        }

        if (!Autorizacion.PuedeAccederArea(usuario, relevamiento.AreaId))
        {
            return (null, CodigosError.AccesoNoAutorizado);
        }

        return (relevamiento, null);
    }
}

public sealed class CrearRelevamientoHandler : IManejador<CrearRelevamientoCommand, Resultado<Relevamiento>>
{
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IUsuarioRepository _usuarios;
    private readonly IServicioAuditoria _auditoria;

    public CrearRelevamientoHandler(IRelevamientoRepository relevamientos, IUsuarioRepository usuarios, IServicioAuditoria auditoria)
    {
        _relevamientos = relevamientos;
        _usuarios = usuarios;
        _auditoria = auditoria;
    }

    public async Task<Resultado<Relevamiento>> ManejarAsync(CrearRelevamientoCommand cmd, CancellationToken ct = default)
    {
        var jefe = await _usuarios.ObtenerPorIdAsync(cmd.JefeId, ct);
        if (jefe is null)
        {
            return Resultado<Relevamiento>.Fallo(CodigosError.UsuarioInexistente);
        }

        // CU-01: el actor es un jefe de área con área vigente (RN-01).
        if (jefe.Rol != RolJerarquico.JefeArea || jefe.AreaId is null)
        {
            return Resultado<Relevamiento>.Fallo(CodigosError.AccesoNoAutorizado);
        }

        var creado = Relevamiento.Crear(cmd.IdentificacionObra, cmd.RadioAgrupacionMetros, jefe.AreaId.Value);
        if (!creado.EsExito)
        {
            return creado;
        }

        if (!await _auditoria.RegistrarAsync(cmd.JefeId, "ALTA_RELEVAMIENTO", $"relevamiento={creado.Valor!.RelevamientoId}", ct))
        {
            return Resultado<Relevamiento>.Fallo(CodigosError.AccionNoAuditada);
        }

        await _relevamientos.AgregarAsync(creado.Valor!, ct);
        await _relevamientos.GuardarCambiosAsync(ct);
        return creado;
    }
}

public sealed class AsignarAgentesHandler : IManejador<AsignarAgentesCommand, Resultado>
{
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IUsuarioRepository _usuarios;
    private readonly IServicioAuditoria _auditoria;

    public AsignarAgentesHandler(IRelevamientoRepository relevamientos, IUsuarioRepository usuarios, IServicioAuditoria auditoria)
    {
        _relevamientos = relevamientos;
        _usuarios = usuarios;
        _auditoria = auditoria;
    }

    public async Task<Resultado> ManejarAsync(AsignarAgentesCommand cmd, CancellationToken ct = default)
    {
        var (relevamiento, error) = await AccesoRelevamiento.CargarAutorizadoAsync(_usuarios, _relevamientos, cmd.JefeId, cmd.RelevamientoId, ct);
        if (error is not null)
        {
            return Resultado.Fallo(error);
        }

        foreach (var agenteId in cmd.AgentesIds)
        {
            var agente = await _usuarios.ObtenerPorIdAsync(agenteId, ct);
            if (agente is null)
            {
                return Resultado.Fallo(CodigosError.UsuarioInexistente);
            }

            var asignado = relevamiento!.AsignarAgente(agente);
            if (!asignado.EsExito)
            {
                return asignado;
            }
        }

        if (!await _auditoria.RegistrarAsync(cmd.JefeId, "ASIGNAR_AGENTES", $"relevamiento={cmd.RelevamientoId}", ct))
        {
            return Resultado.Fallo(CodigosError.AccionNoAuditada);
        }

        await _relevamientos.GuardarCambiosAsync(ct);
        return Resultado.Exito();
    }
}

public sealed class ReasignarAgentesHandler : IManejador<ReasignarAgentesCommand, Resultado>
{
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IUsuarioRepository _usuarios;
    private readonly IServicioAuditoria _auditoria;

    public ReasignarAgentesHandler(IRelevamientoRepository relevamientos, IUsuarioRepository usuarios, IServicioAuditoria auditoria)
    {
        _relevamientos = relevamientos;
        _usuarios = usuarios;
        _auditoria = auditoria;
    }

    public async Task<Resultado> ManejarAsync(ReasignarAgentesCommand cmd, CancellationToken ct = default)
    {
        var (relevamiento, error) = await AccesoRelevamiento.CargarAutorizadoAsync(_usuarios, _relevamientos, cmd.JefeId, cmd.RelevamientoId, ct);
        if (error is not null)
        {
            return Resultado.Fallo(error);
        }

        // Quita las asignaciones vigentes que no figuran en el nuevo conjunto (CU-01 §5.A).
        foreach (var actual in relevamiento!.AgentesVigentes())
        {
            if (!cmd.AgentesIds.Contains(actual))
            {
                relevamiento.QuitarAgente(actual);
            }
        }

        foreach (var agenteId in cmd.AgentesIds)
        {
            var agente = await _usuarios.ObtenerPorIdAsync(agenteId, ct);
            if (agente is null)
            {
                return Resultado.Fallo(CodigosError.UsuarioInexistente);
            }

            var asignado = relevamiento.AsignarAgente(agente);
            if (!asignado.EsExito)
            {
                return asignado;
            }
        }

        if (!await _auditoria.RegistrarAsync(cmd.JefeId, "REASIGNAR_AGENTES", $"relevamiento={cmd.RelevamientoId}", ct))
        {
            return Resultado.Fallo(CodigosError.AccionNoAuditada);
        }

        await _relevamientos.GuardarCambiosAsync(ct);
        return Resultado.Exito();
    }
}

public sealed class TransicionarEstadoHandler : IManejador<TransicionarEstadoCommand, Resultado>
{
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IUsuarioRepository _usuarios;
    private readonly IServicioAuditoria _auditoria;

    public TransicionarEstadoHandler(IRelevamientoRepository relevamientos, IUsuarioRepository usuarios, IServicioAuditoria auditoria)
    {
        _relevamientos = relevamientos;
        _usuarios = usuarios;
        _auditoria = auditoria;
    }

    public async Task<Resultado> ManejarAsync(TransicionarEstadoCommand cmd, CancellationToken ct = default)
    {
        var (relevamiento, error) = await AccesoRelevamiento.CargarAutorizadoAsync(_usuarios, _relevamientos, cmd.JefeId, cmd.RelevamientoId, ct);
        if (error is not null)
        {
            return Resultado.Fallo(error);
        }

        var transicion = relevamiento!.TransicionarA(cmd.Destino);
        if (!transicion.EsExito)
        {
            return transicion;
        }

        if (!await _auditoria.RegistrarAsync(cmd.JefeId, "TRANSICION_RELEVAMIENTO", $"relevamiento={cmd.RelevamientoId};estado={(int)cmd.Destino}", ct))
        {
            return Resultado.Fallo(CodigosError.AccionNoAuditada);
        }

        await _relevamientos.GuardarCambiosAsync(ct);
        return Resultado.Exito();
    }
}

public sealed class ReabrirRelevamientoHandler : IManejador<ReabrirRelevamientoCommand, Resultado>
{
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IUsuarioRepository _usuarios;
    private readonly IServicioAuditoria _auditoria;

    public ReabrirRelevamientoHandler(IRelevamientoRepository relevamientos, IUsuarioRepository usuarios, IServicioAuditoria auditoria)
    {
        _relevamientos = relevamientos;
        _usuarios = usuarios;
        _auditoria = auditoria;
    }

    public async Task<Resultado> ManejarAsync(ReabrirRelevamientoCommand cmd, CancellationToken ct = default)
    {
        var (relevamiento, error) = await AccesoRelevamiento.CargarAutorizadoAsync(_usuarios, _relevamientos, cmd.JefeId, cmd.RelevamientoId, ct);
        if (error is not null)
        {
            return Resultado.Fallo(error);
        }

        var reapertura = relevamiento!.Reabrir();
        if (!reapertura.EsExito)
        {
            return reapertura;
        }

        if (!await _auditoria.RegistrarAsync(cmd.JefeId, "REAPERTURA_RELEVAMIENTO", $"relevamiento={cmd.RelevamientoId}", ct))
        {
            return Resultado.Fallo(CodigosError.AccionNoAuditada);
        }

        await _relevamientos.GuardarCambiosAsync(ct);
        return Resultado.Exito();
    }
}

public sealed class ListarRelevamientosHandler : IManejador<ListarRelevamientosQuery, IReadOnlyList<Relevamiento>>
{
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IUsuarioRepository _usuarios;

    public ListarRelevamientosHandler(IRelevamientoRepository relevamientos, IUsuarioRepository usuarios)
    {
        _relevamientos = relevamientos;
        _usuarios = usuarios;
    }

    public async Task<IReadOnlyList<Relevamiento>> ManejarAsync(ListarRelevamientosQuery query, CancellationToken ct = default)
    {
        var solicitante = await _usuarios.ObtenerPorIdAsync(query.SolicitanteId, ct);
        if (solicitante is null)
        {
            return Array.Empty<Relevamiento>();
        }

        var todos = await _relevamientos.ListarTodosAsync(ct);
        return todos.Where(r => Autorizacion.PuedeAccederArea(solicitante, r.AreaId)).ToList();
    }
}
