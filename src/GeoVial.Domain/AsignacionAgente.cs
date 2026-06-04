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
        // La PK (AsignacionAgenteId) NO se asigna acá a propósito: es ValueGeneratedOnAdd y EF la genera al
        // insertar. Pre-asignarla hacía que, al agregar la asignación por la colección del agregado a un
        // relevamiento ya rastreado, EF la tratara como entidad existente (clave seteada → Modified → UPDATE
        // de 0 filas → DbUpdateConcurrencyException), rompiendo la asignación contra un proveedor relacional.
        RelevamientoId = relevamientoId;
        AgenteUsuarioId = agenteUsuarioId;
        Vigente = true;
    }

    internal void Desactivar() => Vigente = false;

    internal void Reactivar() => Vigente = true;
}
