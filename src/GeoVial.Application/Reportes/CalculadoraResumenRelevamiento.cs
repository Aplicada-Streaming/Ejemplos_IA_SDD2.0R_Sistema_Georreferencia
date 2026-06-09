using GeoVial.Domain;
using GeoVial.Shared;

namespace GeoVial.Application.Reportes;

/// <summary>
/// Calcula el <see cref="ResumenRelevamientoDto"/> a partir de los datos del relevamiento (reporting). Lógica
/// **pura y testeable**: cuenta marcadores, conflictos, observaciones, bandeja, fotos y comentarios, y arma la
/// productividad por agente (observaciones por agente, de mayor a menor). El handler sólo carga los datos y la
/// invoca; así el agregado se cubre en el gate sin base ni repositorios.
/// </summary>
public static class CalculadoraResumenRelevamiento
{
    public static ResumenRelevamientoDto Calcular(
        Relevamiento relevamiento,
        IReadOnlyList<Marcador> marcadores,
        IReadOnlyList<Observacion> observaciones,
        int totalFotos,
        int totalComentarios,
        IReadOnlyDictionary<Guid, string> nombresAgentes)
    {
        ArgumentNullException.ThrowIfNull(relevamiento);
        ArgumentNullException.ThrowIfNull(marcadores);
        ArgumentNullException.ThrowIfNull(observaciones);
        ArgumentNullException.ThrowIfNull(nombresAgentes);

        var productividad = observaciones
            .GroupBy(o => o.AgenteUsuarioId)
            .Select(g => new ProductividadAgenteDto(
                g.Key,
                nombresAgentes.TryGetValue(g.Key, out var nombre) ? nombre : "(desconocido)",
                g.Count()))
            .OrderByDescending(p => p.Observaciones)
            .ThenBy(p => p.Nombre, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new ResumenRelevamientoDto(
            relevamiento.RelevamientoId,
            relevamiento.IdentificacionObra,
            (int)relevamiento.Estado,
            marcadores.Count,
            marcadores.Count(m => m.EnConflicto),
            observaciones.Count,
            observaciones.Count(o => o.SinGeorreferenciar),
            totalFotos,
            totalComentarios,
            productividad);
    }
}
