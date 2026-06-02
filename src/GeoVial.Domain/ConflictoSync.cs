namespace GeoVial.Domain;

/// <summary>Tipo de conflicto (modelo-datos-logico §1.11). 1=marcadores en un mismo radio, 2=edición en conflicto.</summary>
public enum TipoConflicto
{
    MarcadoresEnRadio = 1,
    EdicionEnConflicto = 2,
}

/// <summary>Estado de resolución de un conflicto (modelo-datos-logico §1.11). 1=pendiente, 2=resuelto.</summary>
public enum EstadoResolucionConflicto
{
    Pendiente = 1,
    Resuelto = 2,
}

/// <summary>
/// Conflicto de sincronización a resolver por decisión humana (modelo-datos-logico §1.11, RN-02/RN-04).
/// Nunca se unifica ni descarta de forma automática: la marca persiste hasta la resolución manual (CU-12).
/// </summary>
public sealed class ConflictoSync
{
    public Guid ConflictoSyncId { get; private set; }
    public TipoConflicto Tipo { get; private set; }
    public Guid RelevamientoId { get; private set; }
    public string RecursosInvolucrados { get; private set; }
    public EstadoResolucionConflicto EstadoResolucion { get; private set; }
    public Guid? DecisorUsuarioId { get; private set; }

    // ctor para materialización del ORM
    private ConflictoSync()
    {
        RecursosInvolucrados = string.Empty;
    }

    private ConflictoSync(TipoConflicto tipo, Guid relevamientoId, string recursosInvolucrados)
    {
        ConflictoSyncId = Guid.NewGuid();
        Tipo = tipo;
        RelevamientoId = relevamientoId;
        RecursosInvolucrados = recursosInvolucrados;
        EstadoResolucion = EstadoResolucionConflicto.Pendiente;
    }

    /// <summary>Crea un conflicto de marcadores en un mismo radio entre dos marcadores (RN-02).</summary>
    public static ConflictoSync MarcadoresEnRadio(Guid relevamientoId, Guid marcadorA, Guid marcadorB)
    {
        // Orden canónico de los recursos para que el par sea estable e idempotente.
        var (primero, segundo) = marcadorA.CompareTo(marcadorB) <= 0 ? (marcadorA, marcadorB) : (marcadorB, marcadorA);
        return new ConflictoSync(TipoConflicto.MarcadoresEnRadio, relevamientoId, $"{primero};{segundo}");
    }

    /// <summary>
    /// Crea un conflicto de edición concurrente sobre un recurso consolidado por última escritura (RN-04, CU-07).
    /// Lo resuelve manualmente un usuario autorizado desde la web (CU-12).
    /// </summary>
    public static ConflictoSync EdicionEnConflicto(Guid relevamientoId, Guid recursoId) =>
        new(TipoConflicto.EdicionEnConflicto, relevamientoId, recursoId.ToString());

    public bool EstaPendiente => EstadoResolucion == EstadoResolucionConflicto.Pendiente;

    /// <summary>Marca el conflicto como resuelto por decisión del usuario (CU-12, RN-07).</summary>
    public void Resolver(Guid decisorUsuarioId)
    {
        EstadoResolucion = EstadoResolucionConflicto.Resuelto;
        DecisorUsuarioId = decisorUsuarioId;
    }

    /// <summary>Devuelve los dos marcadores involucrados en un conflicto de radio.</summary>
    public (Guid MarcadorA, Guid MarcadorB) Marcadores()
    {
        var partes = RecursosInvolucrados.Split(';', 2);
        return (Guid.Parse(partes[0]), Guid.Parse(partes[1]));
    }

    /// <summary>Devuelve el recurso involucrado en un conflicto de edición (CU-07/CU-12, RN-04).</summary>
    public Guid Recurso() => Guid.Parse(RecursosInvolucrados);
}
