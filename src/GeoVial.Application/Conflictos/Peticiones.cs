using GeoVial.Application.Cqrs;
using GeoVial.Domain;

namespace GeoVial.Application.Conflictos;

/// <summary>Commands y queries de detección y resolución de conflictos por radio (CU-11, CU-12).</summary>

/// <summary>Decisión humana de resolución de un conflicto (CU-12, RN-02).</summary>
public enum DecisionConflicto
{
    Unificar = 1,
    MantenerSeparados = 2,
}

/// <summary>US-25 / CU-11: detecta los pares de marcadores dentro del radio y los registra como pendientes (RN-02).</summary>
public sealed record DetectarConflictosCommand(Guid UsuarioId, Guid RelevamientoId)
    : IPeticion<Resultado<IReadOnlyList<ConflictoDetectado>>>;

/// <summary>US-25 / CU-11 §5.A: ajusta el radio del relevamiento, lo que reevalúa la cercanía.</summary>
public sealed record AjustarRadioCommand(Guid UsuarioId, Guid RelevamientoId, decimal RadioMetros)
    : IPeticion<Resultado>;

/// <summary>US-26 / CU-12: resuelve un conflicto por decisión humana (unificar o mantener separados).</summary>
public sealed record ResolverConflictoCommand(
    Guid UsuarioId, Guid ConflictoSyncId, DecisionConflicto Decision, Guid? MarcadorResultanteId)
    : IPeticion<Resultado>;

/// <summary>Lista los conflictos pendientes de un relevamiento (CU-12 §5.A).</summary>
public sealed record ConflictosPendientesQuery(Guid SolicitanteId, Guid RelevamientoId)
    : IPeticion<IReadOnlyList<ConflictoPendiente>>;

// --- Proyecciones ---

public sealed record ConflictoDetectado(Guid ConflictoSyncId, Guid MarcadorA, Guid MarcadorB, double DistanciaMetros);

public sealed record ConflictoPendiente(Guid ConflictoSyncId, int Tipo, Guid MarcadorA, Guid MarcadorB);
