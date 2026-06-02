using GeoVial.Application.Abstracciones;
using GeoVial.Application.Cqrs;
using GeoVial.Domain;

namespace GeoVial.Application.Conflictos;

/// <summary>Carga del relevamiento y autorización por rol y área (RN-01) para el módulo de conflictos.</summary>
internal static class AccesoConflicto
{
    public static async Task<(Relevamiento? Relevamiento, string? Error)> CargarRelevamientoAsync(
        IUsuarioRepository usuarios, IRelevamientoRepository relevamientos,
        Guid usuarioId, Guid relevamientoId, CancellationToken ct)
    {
        var relevamiento = await relevamientos.ObtenerPorIdAsync(relevamientoId, ct);
        if (relevamiento is null)
        {
            return (null, CodigosError.RelevamientoInexistente);
        }

        var usuario = await usuarios.ObtenerPorIdAsync(usuarioId, ct);
        if (usuario is null || !Autorizacion.PuedeAccederArea(usuario, relevamiento.AreaId))
        {
            return (null, CodigosError.AccesoNoAutorizado);
        }

        return (relevamiento, null);
    }
}

/// <summary>
/// US-25 / CU-11: detecta los pares de marcadores que quedan dentro del radio de agrupación (RN-02),
/// los registra como conflictos pendientes (sin unificar ni descartar) y marca los marcadores en conflicto.
/// Idempotente: no duplica un conflicto pendiente ya registrado para el mismo par.
/// </summary>
public sealed class DetectarConflictosHandler : IManejador<DetectarConflictosCommand, Resultado<IReadOnlyList<ConflictoDetectado>>>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IMarcadorRepository _marcadores;
    private readonly IConflictoRepository _conflictos;
    private readonly IServicioAuditoria _auditoria;

    public DetectarConflictosHandler(
        IUsuarioRepository usuarios, IRelevamientoRepository relevamientos, IMarcadorRepository marcadores,
        IConflictoRepository conflictos, IServicioAuditoria auditoria)
    {
        _usuarios = usuarios;
        _relevamientos = relevamientos;
        _marcadores = marcadores;
        _conflictos = conflictos;
        _auditoria = auditoria;
    }

    public async Task<Resultado<IReadOnlyList<ConflictoDetectado>>> ManejarAsync(
        DetectarConflictosCommand cmd, CancellationToken ct = default)
    {
        var (relevamiento, error) = await AccesoConflicto.CargarRelevamientoAsync(
            _usuarios, _relevamientos, cmd.UsuarioId, cmd.RelevamientoId, ct);
        if (error is not null)
        {
            return Resultado<IReadOnlyList<ConflictoDetectado>>.Fallo(error);
        }

        if (relevamiento!.EsSoloLectura)
        {
            return Resultado<IReadOnlyList<ConflictoDetectado>>.Fallo(CodigosError.RelevamientoSoloLectura);
        }

        var marcadores = await _marcadores.ListarPorRelevamientoAsync(cmd.RelevamientoId, ct);
        var radio = (double)relevamiento.RadioAgrupacionMetros;
        var detectados = new List<ConflictoDetectado>();

        for (var i = 0; i < marcadores.Count; i++)
        {
            for (var j = i + 1; j < marcadores.Count; j++)
            {
                var distancia = marcadores[i].Coordenada.DistanciaMetrosA(marcadores[j].Coordenada);
                if (distancia >= radio)
                {
                    continue;
                }

                var conflicto = ConflictoSync.MarcadoresEnRadio(cmd.RelevamientoId, marcadores[i].MarcadorId, marcadores[j].MarcadorId);
                if (await _conflictos.ExistePendienteAsync(cmd.RelevamientoId, conflicto.RecursosInvolucrados, ct))
                {
                    continue;
                }

                await _conflictos.AgregarAsync(conflicto, ct);
                await MarcarEnConflictoAsync(marcadores[i].MarcadorId, ct);
                await MarcarEnConflictoAsync(marcadores[j].MarcadorId, ct);
                detectados.Add(new ConflictoDetectado(
                    conflicto.ConflictoSyncId, marcadores[i].MarcadorId, marcadores[j].MarcadorId, distancia));
            }
        }

        if (!await _auditoria.RegistrarAsync(cmd.UsuarioId, "DETECTAR_CONFLICTOS", $"relevamiento={cmd.RelevamientoId}", ct))
        {
            return Resultado<IReadOnlyList<ConflictoDetectado>>.Fallo(CodigosError.AccionNoAuditada);
        }

        await _conflictos.GuardarCambiosAsync(ct);
        return Resultado<IReadOnlyList<ConflictoDetectado>>.Exito(detectados);
    }

    private async Task MarcarEnConflictoAsync(Guid marcadorId, CancellationToken ct)
    {
        var marcador = await _marcadores.ObtenerParaEdicionAsync(marcadorId, ct);
        marcador?.MarcarConflicto();
    }
}

/// <summary>US-25 / CU-11 §5.A: ajusta el radio de agrupación del relevamiento (RN-02), reevaluable por una nueva detección.</summary>
public sealed class AjustarRadioHandler : IManejador<AjustarRadioCommand, Resultado>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IServicioAuditoria _auditoria;

    public AjustarRadioHandler(IUsuarioRepository usuarios, IRelevamientoRepository relevamientos, IServicioAuditoria auditoria)
    {
        _usuarios = usuarios;
        _relevamientos = relevamientos;
        _auditoria = auditoria;
    }

    public async Task<Resultado> ManejarAsync(AjustarRadioCommand cmd, CancellationToken ct = default)
    {
        var (relevamiento, error) = await AccesoConflicto.CargarRelevamientoAsync(
            _usuarios, _relevamientos, cmd.UsuarioId, cmd.RelevamientoId, ct);
        if (error is not null)
        {
            return Resultado.Fallo(error);
        }

        var ajuste = relevamiento!.AjustarRadio(cmd.RadioMetros);
        if (!ajuste.EsExito)
        {
            return ajuste;
        }

        if (!await _auditoria.RegistrarAsync(cmd.UsuarioId, "AJUSTAR_RADIO", $"relevamiento={cmd.RelevamientoId};radio={cmd.RadioMetros}", ct))
        {
            return Resultado.Fallo(CodigosError.AccionNoAuditada);
        }

        await _relevamientos.GuardarCambiosAsync(ct);
        return Resultado.Exito();
    }
}

/// <summary>
/// US-26 / CU-12: resuelve un conflicto por decisión humana. Unificar reasigna observaciones, fotos y
/// comentarios del marcador absorbido al resultante y elimina el absorbido; mantener separados conserva ambos.
/// En ambos casos levanta la marca de conflicto, deja la base consistente y audita (RN-02, RN-07).
/// </summary>
public sealed class ResolverConflictoHandler : IManejador<ResolverConflictoCommand, Resultado>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IConflictoRepository _conflictos;
    private readonly IMarcadorRepository _marcadores;
    private readonly IObservacionRepository _observaciones;
    private readonly IFotoRepository _fotos;
    private readonly IComentarioRepository _comentarios;
    private readonly IServicioAuditoria _auditoria;

    public ResolverConflictoHandler(
        IUsuarioRepository usuarios, IRelevamientoRepository relevamientos, IConflictoRepository conflictos,
        IMarcadorRepository marcadores, IObservacionRepository observaciones, IFotoRepository fotos,
        IComentarioRepository comentarios, IServicioAuditoria auditoria)
    {
        _usuarios = usuarios;
        _relevamientos = relevamientos;
        _conflictos = conflictos;
        _marcadores = marcadores;
        _observaciones = observaciones;
        _fotos = fotos;
        _comentarios = comentarios;
        _auditoria = auditoria;
    }

    public async Task<Resultado> ManejarAsync(ResolverConflictoCommand cmd, CancellationToken ct = default)
    {
        var conflicto = await _conflictos.ObtenerPorIdAsync(cmd.ConflictoSyncId, ct);
        if (conflicto is null || !conflicto.EstaPendiente)
        {
            return Resultado.Fallo(CodigosError.ConflictoInexistente);
        }

        var (relevamiento, error) = await AccesoConflicto.CargarRelevamientoAsync(
            _usuarios, _relevamientos, cmd.UsuarioId, conflicto.RelevamientoId, ct);
        if (error is not null)
        {
            return Resultado.Fallo(error);
        }

        if (relevamiento!.EsSoloLectura)
        {
            return Resultado.Fallo(CodigosError.RelevamientoSoloLectura);
        }

        // La decisión debe corresponder al tipo de conflicto: radio → unificar/mantener; edición → confirmar.
        var (resuelto, operacion) = conflicto.Tipo == TipoConflicto.EdicionEnConflicto
            ? await ResolverEdicionAsync(conflicto, cmd.Decision, ct)
            : await ResolverRadioAsync(conflicto, cmd.Decision, cmd.MarcadorResultanteId, ct);
        if (!resuelto.EsExito)
        {
            return resuelto;
        }

        conflicto.Resolver(cmd.UsuarioId);

        if (!await _auditoria.RegistrarAsync(cmd.UsuarioId, operacion, $"conflicto={cmd.ConflictoSyncId}", ct))
        {
            return Resultado.Fallo(CodigosError.AccionNoAuditada);
        }

        await _conflictos.GuardarCambiosAsync(ct);
        return Resultado.Exito();
    }

    private async Task<(Resultado, string)> ResolverRadioAsync(ConflictoSync conflicto, DecisionConflicto decision, Guid? resultanteId, CancellationToken ct)
    {
        var (marcadorA, marcadorB) = conflicto.Marcadores();
        if (decision == DecisionConflicto.Unificar)
        {
            return (await UnificarAsync(resultanteId, marcadorA, marcadorB, ct), "UNIFICAR_MARCADORES");
        }

        if (decision == DecisionConflicto.MantenerSeparados)
        {
            await LevantarMarcaAsync(marcadorA, ct);
            await LevantarMarcaAsync(marcadorB, ct);
            return (Resultado.Exito(), "MANTENER_SEPARADOS");
        }

        // ConfirmarEdicion no aplica a un conflicto de radio.
        return (Resultado.Fallo(CodigosError.DecisionConflictoInaplicable), string.Empty);
    }

    private static Task<(Resultado, string)> ResolverEdicionAsync(ConflictoSync conflicto, DecisionConflicto decision, CancellationToken ct)
    {
        // RN-04: el valor consolidado por última escritura ya prevaleció; confirmar da por dirimida la edición.
        if (decision != DecisionConflicto.ConfirmarEdicion)
        {
            return Task.FromResult((Resultado.Fallo(CodigosError.DecisionConflictoInaplicable), string.Empty));
        }

        _ = conflicto.Recurso(); // recurso dirimido (queda con el valor de última escritura)
        return Task.FromResult((Resultado.Exito(), "CONFIRMAR_EDICION"));
    }

    private async Task<Resultado> UnificarAsync(Guid? resultanteId, Guid marcadorA, Guid marcadorB, CancellationToken ct)
    {
        if (resultanteId != marcadorA && resultanteId != marcadorB)
        {
            return Resultado.Fallo(CodigosError.UnificacionNoAutorizada);
        }

        var resultante = resultanteId!.Value;
        var absorbidoId = resultante == marcadorA ? marcadorB : marcadorA;

        var resultanteMarcador = await _marcadores.ObtenerParaEdicionAsync(resultante, ct);
        var absorbidoMarcador = await _marcadores.ObtenerParaEdicionAsync(absorbidoId, ct);
        if (resultanteMarcador is null || absorbidoMarcador is null)
        {
            return Resultado.Fallo(CodigosError.MarcadorInexistente);
        }

        foreach (var observacion in await _observaciones.ListarPorMarcadorParaEdicionAsync(absorbidoId, ct))
        {
            observacion.AsociarMarcador(resultante);
        }

        foreach (var foto in await _fotos.ListarPorMarcadorParaEdicionAsync(absorbidoId, ct))
        {
            foto.ReasignarMarcador(resultante);
        }

        foreach (var comentario in await _comentarios.ListarPorMarcadorParaEdicionAsync(absorbidoId, ct))
        {
            comentario.ReasignarMarcador(resultante);
        }

        resultanteMarcador.LevantarConflicto();
        await _marcadores.EliminarAsync(absorbidoMarcador, ct);
        return Resultado.Exito();
    }

    private async Task LevantarMarcaAsync(Guid marcadorId, CancellationToken ct)
    {
        var marcador = await _marcadores.ObtenerParaEdicionAsync(marcadorId, ct);
        marcador?.LevantarConflicto();
    }
}

/// <summary>Lista los conflictos pendientes de un relevamiento para la web (CU-12 §5.A); autoriza por área (RN-01).</summary>
public sealed class ConflictosPendientesHandler : IManejador<ConflictosPendientesQuery, IReadOnlyList<ConflictoPendiente>>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IConflictoRepository _conflictos;

    public ConflictosPendientesHandler(
        IUsuarioRepository usuarios, IRelevamientoRepository relevamientos, IConflictoRepository conflictos)
    {
        _usuarios = usuarios;
        _relevamientos = relevamientos;
        _conflictos = conflictos;
    }

    public async Task<IReadOnlyList<ConflictoPendiente>> ManejarAsync(ConflictosPendientesQuery query, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(query.SolicitanteId, ct);
        var relevamiento = await _relevamientos.ObtenerPorIdAsync(query.RelevamientoId, ct);
        if (usuario is null || relevamiento is null || !Autorizacion.PuedeAccederArea(usuario, relevamiento.AreaId))
        {
            return Array.Empty<ConflictoPendiente>();
        }

        var pendientes = await _conflictos.ListarPendientesPorRelevamientoAsync(query.RelevamientoId, ct);
        return pendientes.Select(c =>
        {
            // Los conflictos de radio llevan dos marcadores; los de edición, un único recurso (RN-04).
            if (c.Tipo == TipoConflicto.MarcadoresEnRadio)
            {
                var (a, b) = c.Marcadores();
                return new ConflictoPendiente(c.ConflictoSyncId, (int)c.Tipo, a, b, null);
            }

            return new ConflictoPendiente(c.ConflictoSyncId, (int)c.Tipo, null, null, c.Recurso());
        }).ToList();
    }
}
