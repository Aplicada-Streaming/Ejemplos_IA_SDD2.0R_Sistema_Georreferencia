using GeoVial.Shared;

namespace GeoVial.CapturaCampo;

/// <summary>Resultado de armar una captura: la petición lista para el backend y si quedó georreferenciada.</summary>
public sealed record ResultadoArmado(CapturarObservacionRequest Peticion, bool Georreferenciada);

/// <summary>
/// Arma la petición de captura del cliente móvil (US-11, CU-04). Toma el binario de la foto, deriva la
/// coordenada de sus metadatos EXIF (RN-03) y construye la <see cref="CapturarObservacionRequest"/>. Si la foto
/// no trae una coordenada válida, arma la petición sin coordenada: el backend la deriva a la bandeja sin
/// georreferenciar a la espera de ubicación manual (US-11 CA-02), sin perder la observación.
/// </summary>
public sealed class ArmadorCapturaCampo
{
    private readonly IExtractorGpsExif _extractor;

    public ArmadorCapturaCampo(IExtractorGpsExif extractor) => _extractor = extractor;

    public ResultadoArmado Armar(ReadOnlySpan<byte> foto, string referenciaArchivo)
    {
        if (_extractor.Extraer(foto) is CoordenadaExif c && EsRangoValido(c))
        {
            return new ResultadoArmado(new CapturarObservacionRequest(referenciaArchivo, c.Latitud, c.Longitud), Georreferenciada: true);
        }

        // RN-03: sin coordenada válida ⇒ petición sin georreferencia (el backend la deriva a la bandeja).
        return new ResultadoArmado(new CapturarObservacionRequest(referenciaArchivo, null, null), Georreferenciada: false);
    }

    /// <summary>Una coordenada fuera del rango geográfico (lat −90..90, lon −180..180) se descarta como inválida.</summary>
    private static bool EsRangoValido(CoordenadaExif c) =>
        c.Latitud is >= -90m and <= 90m && c.Longitud is >= -180m and <= 180m;
}
