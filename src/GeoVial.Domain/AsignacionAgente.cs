namespace GeoVial.Domain;

/// <summary>
/// Asignación de un agente de campo a un relevamiento (modelo-datos-logico §1.4). La baja es lógica
/// (Vigente=false) para conservar las observaciones recolectadas por agentes removidos (CU-01 §5.A).
/// </summary>
public sealed class AsignacionAgente
{
    public Guid AsignacionAgenteId { get; private set; }
    public Guid RelevamientoId { get; private set; }
    public Guid AgenteUsuarioId { get; private set; }
    public bool Vigente { get; private set; }

    // ctor para materialización del ORM
    private AsignacionAgente()
    {
    }

    internal AsignacionAgente(Guid relevamientoId, Guid agenteUsuarioId)
    {
        AsignacionAgenteId = Guid.NewGuid();
        RelevamientoId = relevamientoId;
        AgenteUsuarioId = agenteUsuarioId;
        Vigente = true;
    }

    internal void Desactivar() => Vigente = false;

    internal void Reactivar() => Vigente = true;
}
