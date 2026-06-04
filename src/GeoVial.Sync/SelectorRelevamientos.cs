namespace GeoVial.Sync;

/// <summary>Datos de un relevamiento tal como los devuelve el backend (espejo local del contrato REST).</summary>
public sealed record RelevamientoDatos(
    Guid RelevamientoId,
    string IdentificacionObra,
    int Estado,
    IReadOnlyList<Guid> AgentesVigentes);

/// <summary>Relevamiento listo para mostrar en la selección: marca si el usuario está asignado.</summary>
public sealed record RelevamientoResumen(
    Guid RelevamientoId,
    string IdentificacionObra,
    int Estado,
    bool Asignado);

/// <summary>
/// Lógica de selección de relevamiento del cliente móvil (F-M-04/F-M-05): arma la lista para elegir
/// (marcando los asignados al usuario y ordenándolos), y resuelve cuál es el relevamiento activo a partir
/// de la selección del usuario. Reemplaza el "siempre el primero" que usaba la app. Pura y testeable.
/// </summary>
public static class SelectorRelevamientos
{
    // Estado de relevamiento cerrado (solo lectura): los agentes no capturan ahí (RN-05).
    private const int EstadoCerrado = 3;

    /// <summary>
    /// Arma la lista para mostrar: marca como asignado el relevamiento cuyo conjunto de agentes vigentes
    /// incluye al usuario, y la ordena con los asignados primero y luego por nombre de obra.
    /// </summary>
    public static IReadOnlyList<RelevamientoResumen> Listar(IEnumerable<RelevamientoDatos> relevamientos, Guid? usuarioId) =>
        relevamientos
            .Select(r => new RelevamientoResumen(
                r.RelevamientoId,
                r.IdentificacionObra,
                r.Estado,
                usuarioId is { } id && r.AgentesVigentes.Contains(id)))
            .OrderByDescending(r => r.Asignado)
            .ThenBy(r => r.IdentificacionObra, StringComparer.OrdinalIgnoreCase)
            .ToList();

    /// <summary>
    /// Resuelve el relevamiento activo: la selección del usuario si sigue disponible; si no, el primer
    /// asignado no cerrado; si tampoco, el primero de la lista; <c>null</c> si no hay relevamientos.
    /// </summary>
    public static Guid? Activo(IReadOnlyList<RelevamientoResumen> lista, Guid? seleccionado)
    {
        if (lista.Count == 0)
        {
            return null;
        }

        if (seleccionado is { } sel && lista.Any(r => r.RelevamientoId == sel))
        {
            return sel;
        }

        var asignadoAbierto = lista.FirstOrDefault(r => r.Asignado && r.Estado != EstadoCerrado);
        return (asignadoAbierto ?? lista[0]).RelevamientoId;
    }
}
