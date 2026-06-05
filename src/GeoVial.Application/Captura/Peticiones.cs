using GeoVial.Application.Cqrs;
using GeoVial.Domain;

namespace GeoVial.Application.Captura;

/// <summary>Commands y queries del módulo de captura y georreferenciación (CU-04, CU-05).</summary>

/// <summary>Captura una observación. Si llegan coordenadas de metadatos (EXIF) se usan como fuente primaria (RN-03).
/// <paramref name="CapturaId"/> es la clave de idempotencia del cliente (S42): si se reenvía una captura ya
/// registrada con esa clave, el handler devuelve la observación original en vez de duplicarla. Opcional para
/// no romper a los llamadores previos ni el comportamiento sin cliente idempotente.</summary>
public sealed record CapturarObservacionCommand(
    Guid AgenteId,
    Guid RelevamientoId,
    string ReferenciaArchivo,
    decimal? LatitudExif,
    decimal? LongitudExif,
    Guid? CapturaId = null)
    : IPeticion<Resultado<ResultadoCaptura>>;

/// <summary>Resultado de la captura: si quedó georreferenciada, el marcador; si no, la bandeja sin georreferenciar.
/// Incluye el <see cref="FotoId"/> de la foto creada para encadenar la subida de su binario (CU-04, ADR-08).</summary>
public sealed record ResultadoCaptura(Guid ObservacionId, Guid? MarcadorId, bool SinGeorreferenciar, Guid FotoId);

/// <summary>Ubica manualmente el punto de una observación sin georreferenciar (CU-05).</summary>
public sealed record UbicarObservacionManualCommand(
    Guid UsuarioId,
    Guid ObservacionId,
    decimal Latitud,
    decimal Longitud)
    : IPeticion<Resultado>;

public sealed record ListarObservacionesQuery(Guid SolicitanteId, Guid RelevamientoId)
    : IPeticion<IReadOnlyList<Observacion>>;

/// <summary>
/// Sube el binario de una foto al backend de alojamiento y asienta en la foto la referencia devuelta
/// (CU-04, ADR-08). La base persiste solo la referencia; el binario vive en el backend activo.
/// </summary>
public sealed record SubirContenidoFotoCommand(Guid UsuarioId, Guid FotoId, string NombreArchivo, byte[] Contenido)
    : IPeticion<Resultado>;

/// <summary>
/// Descarga el binario de una foto del backend de alojamiento para el visor a pantalla completa (CU-09, US-24).
/// Devuelve null si no se autoriza (RN-01) o si el binario no está disponible.
/// </summary>
public sealed record DescargarContenidoFotoQuery(Guid SolicitanteId, Guid FotoId)
    : IPeticion<byte[]?>;
