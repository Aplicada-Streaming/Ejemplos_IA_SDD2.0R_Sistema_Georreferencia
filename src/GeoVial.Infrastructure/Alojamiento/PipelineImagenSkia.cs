using GeoVial.FileHosting;
using SkiaSharp;

namespace GeoVial.Infrastructure.Alojamiento;

/// <summary>
/// Pipeline de imágenes sobre SkiaSharp (MIT, BT-19). Redimensiona la foto hacia abajo hasta un máximo
/// configurable (sin agrandar imágenes pequeñas) y la recodifica en JPEG con una calidad configurable
/// para acotar el payload (arquitectura-solución §8). Si el binario no es una imagen procesable, devuelve
/// el original sin tocarlo. La fuente de coordenada (RN-03) ya vive en el dominio y no depende del binario.
/// </summary>
public sealed class PipelineImagenSkia : IPipelineImagen
{
    private readonly int _maxDimension;
    private readonly int _calidadJpeg;

    public PipelineImagenSkia(int maxDimension, int calidadJpeg)
    {
        _maxDimension = maxDimension;
        _calidadJpeg = calidadJpeg;
    }

    public Task<byte[]> ProcesarAsync(byte[] original, CancellationToken ct = default)
    {
        SKBitmap? bitmap = null;
        try
        {
            bitmap = SKBitmap.Decode(original);
        }
        catch (Exception)
        {
            bitmap = null;
        }

        if (bitmap is null || bitmap.Width == 0 || bitmap.Height == 0)
        {
            // No es una imagen procesable (p. ej. un binario arbitrario): se aloja tal cual.
            bitmap?.Dispose();
            return Task.FromResult(original);
        }

        using var _ = bitmap;

        var lado = Math.Max(bitmap.Width, bitmap.Height);
        SKBitmap? redimensionado = null;
        var fuente = bitmap;

        if (lado > _maxDimension)
        {
            var escala = (double)_maxDimension / lado;
            var ancho = Math.Max(1, (int)Math.Round(bitmap.Width * escala));
            var alto = Math.Max(1, (int)Math.Round(bitmap.Height * escala));
            redimensionado = bitmap.Resize(new SKImageInfo(ancho, alto), SKFilterQuality.High);
            fuente = redimensionado ?? bitmap;
        }

        using var imagen = SKImage.FromBitmap(fuente);
        using var datos = imagen.Encode(SKEncodedImageFormat.Jpeg, _calidadJpeg);
        redimensionado?.Dispose();
        return Task.FromResult(datos.ToArray());
    }
}
