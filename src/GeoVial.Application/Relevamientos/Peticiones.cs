using GeoVial.Application.Cqrs;
using GeoVial.Domain;

namespace GeoVial.Application.Relevamientos;

/// <summary>Commands y queries del módulo de relevamientos (CU-01, CU-10). CQRS ligero (ADR-01).</summary>

public sealed record CrearRelevamientoCommand(Guid JefeId, string IdentificacionObra, decimal RadioAgrupacionMetros)
    : IPeticion<Resultado<Relevamiento>>;

public sealed record AsignarAgentesCommand(Guid JefeId, Guid RelevamientoId, IReadOnlyList<Guid> AgentesIds)
    : IPeticion<Resultado>;

public sealed record ReasignarAgentesCommand(Guid JefeId, Guid RelevamientoId, IReadOnlyList<Guid> AgentesIds)
    : IPeticion<Resultado>;

public sealed record TransicionarEstadoCommand(Guid JefeId, Guid RelevamientoId, EstadoRelevamiento Destino)
    : IPeticion<Resultado>;

public sealed record ReabrirRelevamientoCommand(Guid JefeId, Guid RelevamientoId)
    : IPeticion<Resultado>;

public sealed record ListarRelevamientosQuery(Guid SolicitanteId)
    : IPeticion<IReadOnlyList<Relevamiento>>;

/// <summary>
/// Relevamientos con asignación vigente del agente autenticado ("asignados a mí", F-M-04/05). A diferencia de
/// <see cref="ListarRelevamientosQuery"/> (todos los del área), filtra por la asignación para que el dispositivo
/// del agente sólo reciba su propio trabajo (minimización de datos).
/// </summary>
public sealed record ListarRelevamientosAsignadosQuery(Guid AgenteId)
    : IPeticion<IReadOnlyList<Relevamiento>>;
