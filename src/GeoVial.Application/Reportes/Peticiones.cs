using GeoVial.Application.Cqrs;
using GeoVial.Shared;

namespace GeoVial.Application.Reportes;

/// <summary>
/// Resumen de actividad de un relevamiento para el tablero de jefes (reporting). Devuelve <c>null</c> si el
/// relevamiento no existe o el solicitante no tiene acceso a su área (RN-01), sin filtrar información.
/// </summary>
public sealed record ResumenRelevamientoQuery(Guid SolicitanteId, Guid RelevamientoId)
    : IPeticion<ResumenRelevamientoDto?>;
