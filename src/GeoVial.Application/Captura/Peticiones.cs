using GeoVial.Application.Cqrs;
using GeoVial.Domain;

namespace GeoVial.Application.Captura;

/// <summary>Commands y queries del módulo de captura y georreferenciación (CU-04, CU-05).</summary>

/// <summary>Captura una observación. Si llegan coordenadas de metadatos (EXIF) se usan como fuente primaria (RN-03).</summary>
public sealed record CapturarObservacionCommand(
    Guid AgenteId,
    Guid RelevamientoId,
    string ReferenciaArchivo,
    decimal? LatitudExif,
    decimal? LongitudExif)
    : IPeticion<Resultado<ResultadoCaptura>>;

/// <summary>Resultado de la captura: si quedó georreferenciada, el marcador; si no, la bandeja sin georreferenciar.</summary>
public sealed record ResultadoCaptura(Guid ObservacionId, Guid? MarcadorId, bool SinGeorreferenciar);

/// <summary>Ubica manualmente el punto de una observación sin georreferenciar (CU-05).</summary>
public sealed record UbicarObservacionManualCommand(
    Guid UsuarioId,
    Guid ObservacionId,
    decimal Latitud,
    decimal Longitud)
    : IPeticion<Resultado>;

public sealed record ListarObservacionesQuery(Guid SolicitanteId, Guid RelevamientoId)
    : IPeticion<IReadOnlyList<Observacion>>;
