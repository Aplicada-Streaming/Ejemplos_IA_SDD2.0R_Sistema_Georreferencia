using GeoVial.Application.Abstracciones;
using GeoVial.Application.Conflictos;
using GeoVial.Application.Cqrs;
using GeoVial.Domain;

namespace GeoVial.Application.Sincronizacion;

/// <summary>
/// US-18 / CU-07: consolida en el backend los cambios de campo subidos por el agente. Es idempotente por
/// CambioId (RC-03), aplica last-write-wins por marca temporal y marca como conflicto las ediciones
/// concurrentes (RN-04); tras consolidar, reutiliza la detección de marcadores en radio (RN-02, Sprint 05)
/// y baja las actualizaciones del relevamiento. Autoriza por área (RN-01) y audita (RN-07).
/// </summary>
public sealed class SincronizarHandler : IManejador<SincronizarCommand, Resultado<ResultadoSincronizacion>>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IComentarioRepository _comentarios;
    private readonly IConflictoRepository _conflictos;
    private readonly ICambioAplicadoRepository _cambios;
    private readonly IServicioAuditoria _auditoria;
    private readonly IRelojUtc _reloj;
    private readonly IMediador _mediador;

    public SincronizarHandler(
        IUsuarioRepository usuarios, IRelevamientoRepository relevamientos, IComentarioRepository comentarios,
        IConflictoRepository conflictos, ICambioAplicadoRepository cambios, IServicioAuditoria auditoria,
        IRelojUtc reloj, IMediador mediador)
    {
        _usuarios = usuarios;
        _relevamientos = relevamientos;
        _comentarios = comentarios;
        _conflictos = conflictos;
        _cambios = cambios;
        _auditoria = auditoria;
        _reloj = reloj;
        _mediador = mediador;
    }

    public async Task<Resultado<ResultadoSincronizacion>> ManejarAsync(SincronizarCommand cmd, CancellationToken ct = default)
    {
        var relevamiento = await _relevamientos.ObtenerPorIdAsync(cmd.RelevamientoId, ct);
        if (relevamiento is null)
        {
            return Resultado<ResultadoSincronizacion>.Fallo(CodigosError.RelevamientoInexistente);
        }

        if (relevamiento.EsSoloLectura)
        {
            return Resultado<ResultadoSincronizacion>.Fallo(CodigosError.RelevamientoSoloLectura);
        }

        var usuario = await _usuarios.ObtenerPorIdAsync(cmd.AgenteId, ct);
        if (usuario is null || !Autorizacion.PuedeAccederArea(usuario, relevamiento.AreaId))
        {
            return Resultado<ResultadoSincronizacion>.Fallo(CodigosError.AccesoNoAutorizado);
        }

        // Audita primero: el asiento se persiste solo, sin arrastrar la consolidación (gate RN-07).
        if (!await _auditoria.RegistrarAsync(cmd.AgenteId, "SINCRONIZAR", $"relevamiento={cmd.RelevamientoId}", ct))
        {
            return Resultado<ResultadoSincronizacion>.Fallo(CodigosError.AccionNoAuditada);
        }

        // Sube y consolida los cambios en orden de marca temporal, de forma idempotente (CU-07 §4.2-§4.3).
        var confirmados = new List<Guid>();
        foreach (var cambio in cmd.Cambios.OrderBy(c => c.MarcaTemporal))
        {
            if (await _cambios.ExisteAsync(cambio.CambioId, ct))
            {
                confirmados.Add(cambio.CambioId); // reintento: ya aplicado, se confirma sin reaplicar
                continue;
            }

            if (await ConsolidarComentarioAsync(cmd.RelevamientoId, cambio, ct))
            {
                await _cambios.AgregarAsync(new CambioAplicado(cambio.CambioId, cmd.RelevamientoId, _reloj.AhoraUtc), ct);
                confirmados.Add(cambio.CambioId);
            }
        }

        await _comentarios.GuardarCambiosAsync(ct);

        // CU-07 §4.4: reutiliza la detección de marcadores en un mismo radio (RN-02, Sprint 05).
        await _mediador.EnviarAsync(new DetectarConflictosCommand(cmd.AgenteId, cmd.RelevamientoId), ct);

        // Baja: conflictos pendientes y comentarios actualizados desde la última marca del cliente (CU-07 §4.5).
        var conflictos = (await _conflictos.ListarPendientesPorRelevamientoAsync(cmd.RelevamientoId, ct))
            .Select(c => new ConflictoSincronizado(c.ConflictoSyncId, (int)c.Tipo, c.RecursosInvolucrados))
            .ToList();

        var desde = cmd.Desde ?? DateTime.MinValue;
        var actualizaciones = (await _comentarios.ListarActualizadosDesdeAsync(cmd.RelevamientoId, desde, ct))
            .Select(c => new ActualizacionComentario(c.ComentarioId, c.MarcadorId, c.Texto, c.MarcaUltimaEdicion))
            .ToList();

        return Resultado<ResultadoSincronizacion>.Exito(new ResultadoSincronizacion(confirmados, conflictos, actualizaciones));
    }

    /// <summary>Consolida un cambio de comentario; devuelve true si se aplicó (false si se descarta sin error).</summary>
    private async Task<bool> ConsolidarComentarioAsync(Guid relevamientoId, CambioComentario cambio, CancellationToken ct)
    {
        if (cambio.Operacion == OperacionSync.Crear)
        {
            var existente = await _comentarios.ObtenerPorIdAsync(cambio.ComentarioId, ct);
            if (existente is not null)
            {
                return false; // ya existe: no se duplica
            }

            var creado = Comentario.Sincronizar(
                cambio.ComentarioId, cambio.MarcadorId, cambio.FotoId, cambio.AutorUsuarioId, cambio.Texto, cambio.MarcaTemporal);
            if (!creado.EsExito)
            {
                return false;
            }

            await _comentarios.AgregarAsync(creado.Valor!, ct);
            return true;
        }

        var comentario = await _comentarios.ObtenerPorIdAsync(cambio.ComentarioId, ct);
        if (comentario is null || string.IsNullOrWhiteSpace(cambio.Texto))
        {
            return false; // nada que actualizar o texto inválido
        }

        // RN-04: hay conflicto si el comentario ya tenía una edición central que compite con la entrante.
        var hayConflicto = comentario.FueEditado;

        // Last-write-wins: prevalece la marca temporal más reciente.
        if (cambio.MarcaTemporal > comentario.MarcaUltimaEdicion)
        {
            comentario.AplicarEdicion(cambio.Texto, cambio.MarcaTemporal);
        }

        if (hayConflicto)
        {
            await MarcarEdicionEnConflictoAsync(relevamientoId, comentario.ComentarioId, ct);
        }

        return true;
    }

    private async Task MarcarEdicionEnConflictoAsync(Guid relevamientoId, Guid comentarioId, CancellationToken ct)
    {
        var conflicto = ConflictoSync.EdicionEnConflicto(relevamientoId, comentarioId);
        if (!await _conflictos.ExistePendienteAsync(relevamientoId, conflicto.RecursosInvolucrados, ct))
        {
            await _conflictos.AgregarAsync(conflicto, ct);
        }
    }
}
