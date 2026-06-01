namespace GeoVial.Application.ExportImport;

/// <summary>
/// Manifiesto de un relevamiento completo para exportación/importación (CU-08 §5.A/§5.B, contratos-rest §4.1).
/// Las "claves locales" son los identificadores originales del archivo; sirven solo para enlazar las entidades
/// dentro del manifiesto y se remapean a identificadores nuevos al importar, sin colisionar con datos existentes.
/// </summary>
public sealed record ManifiestoRelevamiento(
    int Version,
    string IdentificacionObra,
    int Estado,
    decimal RadioAgrupacionMetros,
    Guid AreaId,
    IReadOnlyList<ManifiestoMarcador> Marcadores,
    IReadOnlyList<ManifiestoObservacion> Observaciones,
    IReadOnlyList<ManifiestoFoto> Fotos,
    IReadOnlyList<ManifiestoComentario> Comentarios)
{
    /// <summary>Versión del formato del manifiesto. Permite validar compatibilidad al importar.</summary>
    public const int VersionActual = 1;
}

public sealed record ManifiestoMarcador(Guid ClaveLocal, decimal Latitud, decimal Longitud, bool EnConflicto);

public sealed record ManifiestoObservacion(
    Guid ClaveLocal, Guid? MarcadorClaveLocal, Guid AgenteUsuarioId, DateTime MomentoCaptura, bool SinGeorreferenciar);

public sealed record ManifiestoFoto(
    Guid ClaveLocal, Guid ObservacionClaveLocal, Guid? MarcadorClaveLocal,
    bool TieneMetadatos, int? Fuente, string ReferenciaArchivo, IReadOnlyList<string> Etiquetas);

public sealed record ManifiestoComentario(
    Guid MarcadorClaveLocal, Guid? FotoClaveLocal, Guid AutorUsuarioId, string Texto, DateTime Momento, IReadOnlyList<string> Etiquetas);

/// <summary>
/// Paquete completo desempaquetado de un archivo de exportación: el manifiesto y los binarios de las
/// fotos, indexados por la referencia que el manifiesto declara (CU-08 §5.B).
/// </summary>
public sealed record PaqueteRelevamiento(
    ManifiestoRelevamiento Manifiesto, IReadOnlyDictionary<string, byte[]> BinariosFotos);
