namespace GeoVial.Shared;

/// <summary>
/// Resumen de actividad de un relevamiento para el tablero de jefes (reporting/analytics): totales del
/// relevamiento y productividad por agente. Es un reporte de **conteos** (no trae las fotos ni los comentarios),
/// pensado para un panel de gestión sobre los datos ya capturados.
/// </summary>
public sealed record ResumenRelevamientoDto(
    Guid RelevamientoId,
    string IdentificacionObra,
    int Estado,
    int Marcadores,
    int MarcadoresEnConflicto,
    int Observaciones,
    int ObservacionesSinGeorreferenciar,
    int Fotos,
    int Comentarios,
    IReadOnlyList<ProductividadAgenteDto> Productividad);

/// <summary>Observaciones capturadas por un agente en el relevamiento (productividad).</summary>
public sealed record ProductividadAgenteDto(Guid AgenteId, string Nombre, int Observaciones);
