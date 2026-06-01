namespace GeoVial.FileHosting;

/// <summary>
/// Pipeline de procesamiento del binario de una foto antes de alojarlo (BT-19, arquitectura-solución §8):
/// comprime y redimensiona para acotar el payload. La fuente de coordenada (RN-03) ya está asentada en el
/// dominio en la captura, así que el reprocesado del binario no afecta la georreferenciación.
/// </summary>
public interface IPipelineImagen
{
    /// <summary>Devuelve el binario comprimido/redimensionado; si no es una imagen procesable, devuelve el original.</summary>
    Task<byte[]> ProcesarAsync(byte[] original, CancellationToken ct = default);
}
