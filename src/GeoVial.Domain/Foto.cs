namespace GeoVial.Domain;

/// <summary>
/// Foto de una observación (modelo-datos-logico §1.7). Persiste la referencia al archivo (no el binario,
/// que vive en la librería de alojamiento, ADR-08) y la fuente de la coordenada (RN-03).
/// </summary>
public sealed class Foto
{
    public Guid FotoId { get; private set; }
    public Guid ObservacionId { get; private set; }
    public Guid? MarcadorId { get; private set; }
    public bool TieneMetadatosUbicacion { get; private set; }
    public FuenteCoordenada? Fuente { get; private set; }
    public string ReferenciaArchivo { get; private set; }

    // ctor para materialización del ORM
    private Foto()
    {
        ReferenciaArchivo = string.Empty;
    }

    private Foto(Guid observacionId, Guid? marcadorId, bool tieneMetadatos, FuenteCoordenada? fuente, string referenciaArchivo)
    {
        FotoId = Guid.NewGuid();
        ObservacionId = observacionId;
        MarcadorId = marcadorId;
        TieneMetadatosUbicacion = tieneMetadatos;
        Fuente = fuente;
        ReferenciaArchivo = referenciaArchivo;
    }

    public static Foto Crear(Guid observacionId, Guid? marcadorId, bool tieneMetadatos, FuenteCoordenada? fuente, string referenciaArchivo) =>
        new(observacionId, marcadorId, tieneMetadatos, fuente, referenciaArchivo);

    /// <summary>Asienta la fuente de coordenada y el marcador cuando se resuelve la georreferenciación.</summary>
    public void Georreferenciar(FuenteCoordenada fuente, Guid marcadorId)
    {
        Fuente = fuente;
        MarcadorId = marcadorId;
    }
}
