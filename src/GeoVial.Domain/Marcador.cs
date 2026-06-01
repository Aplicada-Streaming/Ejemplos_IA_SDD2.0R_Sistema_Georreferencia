namespace GeoVial.Domain;

/// <summary>
/// Marcador geográfico: punto del mapa que agrupa observaciones dentro de un relevamiento
/// (modelo-datos-logico §1.6, RN-02). La marca de conflicto solo la levanta la resolución humana.
/// </summary>
public sealed class Marcador
{
    public Guid MarcadorId { get; private set; }
    public Guid RelevamientoId { get; private set; }
    public decimal Latitud { get; private set; }
    public decimal Longitud { get; private set; }
    public bool EnConflicto { get; private set; }

    // ctor para materialización del ORM
    private Marcador()
    {
    }

    private Marcador(Guid relevamientoId, Coordenada coordenada)
    {
        MarcadorId = Guid.NewGuid();
        RelevamientoId = relevamientoId;
        Latitud = coordenada.Latitud;
        Longitud = coordenada.Longitud;
    }

    public Coordenada Coordenada => new(Latitud, Longitud);

    public static Marcador Crear(Guid relevamientoId, Coordenada coordenada) => new(relevamientoId, coordenada);

    /// <summary>Marca el conflicto por decisión humana (RN-02); nunca de forma automática.</summary>
    public void MarcarConflicto() => EnConflicto = true;
}
