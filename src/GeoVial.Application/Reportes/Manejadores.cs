using GeoVial.Application.Abstracciones;
using GeoVial.Application.Cqrs;
using GeoVial.Domain;
using GeoVial.Shared;

namespace GeoVial.Application.Reportes;

/// <summary>
/// Arma el resumen de actividad de un relevamiento (reporting): autoriza por área (RN-01), carga los datos por
/// los repositorios existentes y delega el agregado en <see cref="CalculadoraResumenRelevamiento"/>.
/// </summary>
public sealed class ResumenRelevamientoHandler : IManejador<ResumenRelevamientoQuery, ResumenRelevamientoDto?>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IMarcadorRepository _marcadores;
    private readonly IObservacionRepository _observaciones;
    private readonly IFotoRepository _fotos;
    private readonly IComentarioRepository _comentarios;

    public ResumenRelevamientoHandler(
        IUsuarioRepository usuarios, IRelevamientoRepository relevamientos, IMarcadorRepository marcadores,
        IObservacionRepository observaciones, IFotoRepository fotos, IComentarioRepository comentarios)
    {
        _usuarios = usuarios;
        _relevamientos = relevamientos;
        _marcadores = marcadores;
        _observaciones = observaciones;
        _fotos = fotos;
        _comentarios = comentarios;
    }

    public async Task<ResumenRelevamientoDto?> ManejarAsync(ResumenRelevamientoQuery query, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(query.SolicitanteId, ct);
        var relevamiento = await _relevamientos.ObtenerPorIdAsync(query.RelevamientoId, ct);
        if (usuario is null || relevamiento is null || !Autorizacion.PuedeAccederArea(usuario, relevamiento.AreaId))
        {
            return null;
        }

        var marcadores = await _marcadores.ListarPorRelevamientoAsync(query.RelevamientoId, ct);
        var totalFotos = 0;
        var totalComentarios = 0;
        foreach (var marcador in marcadores)
        {
            totalFotos += (await _fotos.ListarPorMarcadorAsync(marcador.MarcadorId, ct)).Count;
            totalComentarios += (await _comentarios.ListarPorMarcadorAsync(marcador.MarcadorId, ct)).Count;
        }

        var observaciones = await _observaciones.ListarPorRelevamientoAsync(query.RelevamientoId, ct);

        // Nombre de cada agente que capturó, para la productividad del panel.
        var nombres = new Dictionary<Guid, string>();
        foreach (var agenteId in observaciones.Select(o => o.AgenteUsuarioId).Distinct())
        {
            var agente = await _usuarios.ObtenerPorIdAsync(agenteId, ct);
            nombres[agenteId] = agente?.Nombre ?? "(desconocido)";
        }

        return CalculadoraResumenRelevamiento.Calcular(relevamiento, marcadores, observaciones, totalFotos, totalComentarios, nombres);
    }
}
