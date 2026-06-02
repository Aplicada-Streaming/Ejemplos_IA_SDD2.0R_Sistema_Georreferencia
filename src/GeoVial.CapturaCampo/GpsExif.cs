namespace GeoVial.CapturaCampo;

/// <summary>Coordenada geográfica derivada de los metadatos EXIF de una foto (latitud/longitud en grados decimales).</summary>
public readonly record struct CoordenadaExif(decimal Latitud, decimal Longitud);

/// <summary>Extrae la coordenada GPS de los metadatos de una foto (RN-03). Devuelve null si la foto no la trae.</summary>
public interface IExtractorGpsExif
{
    CoordenadaExif? Extraer(ReadOnlySpan<byte> foto);
}

/// <summary>
/// Lector de la coordenada GPS embebida en los metadatos EXIF de una foto JPEG (US-11, RN-03). Recorre el
/// segmento APP1/Exif → cabecera TIFF → GPS IFD y convierte los rationals grados/minutos/segundos a grados
/// decimales, con signo según el hemisferio (N/S, E/W). Soporta ambos órdenes de bytes (little/big endian).
/// Ante cualquier ausencia o malformación devuelve null: la observación irá a la bandeja sin georreferenciar,
/// nunca se pierde. Implementación propia, sin paquetes externos (TreatWarningsAsErrors / NU1902).
/// </summary>
public sealed class LectorGpsExif : IExtractorGpsExif
{
    // Tags del GPS IFD (EXIF 2.3 §4.6.6).
    private const int TagGpsIfdPointer = 0x8825;
    private const int TagLatitudeRef = 0x0001;
    private const int TagLatitude = 0x0002;
    private const int TagLongitudeRef = 0x0003;
    private const int TagLongitude = 0x0004;

    public CoordenadaExif? Extraer(ReadOnlySpan<byte> foto)
    {
        try
        {
            return ExtraerInterno(foto);
        }
        catch
        {
            // Cualquier malformación (índice fuera de rango, datos truncados) ⇒ sin coordenada.
            return null;
        }
    }

    private static CoordenadaExif? ExtraerInterno(ReadOnlySpan<byte> jpeg)
    {
        // SOI: todo JPEG empieza con FF D8.
        if (jpeg.Length < 4 || jpeg[0] != 0xFF || jpeg[1] != 0xD8)
        {
            return null;
        }

        // Buscar el segmento APP1 (FF E1) que arranca con "Exif\0\0".
        var i = 2;
        while (i + 4 <= jpeg.Length && jpeg[i] == 0xFF)
        {
            var marcador = jpeg[i + 1];
            if (marcador == 0xD9 || marcador == 0xDA) // EOI o inicio del scan: no hay más metadatos.
            {
                break;
            }

            var longitud = (jpeg[i + 2] << 8) | jpeg[i + 3]; // longitud big-endian, incluye sus 2 bytes.
            var inicioDatos = i + 4;
            var largoDatos = longitud - 2;
            if (largoDatos < 0 || inicioDatos + largoDatos > jpeg.Length)
            {
                return null;
            }

            if (marcador == 0xE1 && largoDatos >= 6 &&
                jpeg[inicioDatos] == (byte)'E' && jpeg[inicioDatos + 1] == (byte)'x' &&
                jpeg[inicioDatos + 2] == (byte)'i' && jpeg[inicioDatos + 3] == (byte)'f' &&
                jpeg[inicioDatos + 4] == 0 && jpeg[inicioDatos + 5] == 0)
            {
                var tiff = jpeg.Slice(inicioDatos + 6, largoDatos - 6);
                return LeerDesdeTiff(tiff);
            }

            i += 2 + longitud;
        }

        return null;
    }

    private static CoordenadaExif? LeerDesdeTiff(ReadOnlySpan<byte> tiff)
    {
        if (tiff.Length < 8)
        {
            return null;
        }

        bool little = tiff[0] == (byte)'I' && tiff[1] == (byte)'I';
        bool big = tiff[0] == (byte)'M' && tiff[1] == (byte)'M';
        if (!little && !big)
        {
            return null;
        }

        var ifd0 = (int)LeerU32(tiff, 4, little);
        var gpsIfd = BuscarPunteroGps(tiff, ifd0, little);
        if (gpsIfd is not int gps)
        {
            return null;
        }

        var latRef = LeerRefAscii(tiff, gps, TagLatitudeRef, little);
        var lat = LeerGradosDms(tiff, gps, TagLatitude, little);
        var lonRef = LeerRefAscii(tiff, gps, TagLongitudeRef, little);
        var lon = LeerGradosDms(tiff, gps, TagLongitude, little);
        if (lat is not decimal latitud || lon is not decimal longitud || latRef is null || lonRef is null)
        {
            return null;
        }

        if (latRef is 'S')
        {
            latitud = -latitud;
        }

        if (lonRef is 'W')
        {
            longitud = -longitud;
        }

        return new CoordenadaExif(latitud, longitud);
    }

    private static int? BuscarPunteroGps(ReadOnlySpan<byte> tiff, int ifdOffset, bool little)
    {
        var conteo = LeerU16(tiff, ifdOffset, little);
        for (var n = 0; n < conteo; n++)
        {
            var entrada = ifdOffset + 2 + n * 12;
            if (LeerU16(tiff, entrada, little) == TagGpsIfdPointer)
            {
                return (int)LeerU32(tiff, entrada + 8, little);
            }
        }

        return null;
    }

    /// <summary>Lee el primer carácter ASCII del valor de un tag de referencia (N/S/E/W); su valor cabe inline.</summary>
    private static char? LeerRefAscii(ReadOnlySpan<byte> tiff, int ifdOffset, int tag, bool little)
    {
        var entrada = BuscarEntrada(tiff, ifdOffset, tag, little);
        if (entrada is not int e)
        {
            return null;
        }

        // type=2 (ASCII), count=2 ("N\0"): el valor de 1 carácter va inline en el campo valor (offset 8 de la entrada).
        return (char)tiff[e + 8];
    }

    /// <summary>Lee los 3 rationals (grados, minutos, segundos) de un tag y los combina en grados decimales.</summary>
    private static decimal? LeerGradosDms(ReadOnlySpan<byte> tiff, int ifdOffset, int tag, bool little)
    {
        var entrada = BuscarEntrada(tiff, ifdOffset, tag, little);
        if (entrada is not int e)
        {
            return null;
        }

        // type=5 (RATIONAL), count=3 = 24 bytes > 4 ⇒ el campo valor es un offset a los datos (desde el inicio del TIFF).
        var datos = (int)LeerU32(tiff, e + 8, little);
        var grados = LeerRational(tiff, datos, little);
        var minutos = LeerRational(tiff, datos + 8, little);
        var segundos = LeerRational(tiff, datos + 16, little);
        if (grados is not decimal g || minutos is not decimal m || segundos is not decimal s)
        {
            return null;
        }

        return g + m / 60m + s / 3600m;
    }

    private static int? BuscarEntrada(ReadOnlySpan<byte> tiff, int ifdOffset, int tag, bool little)
    {
        var conteo = LeerU16(tiff, ifdOffset, little);
        for (var n = 0; n < conteo; n++)
        {
            var entrada = ifdOffset + 2 + n * 12;
            if (LeerU16(tiff, entrada, little) == tag)
            {
                return entrada;
            }
        }

        return null;
    }

    private static decimal? LeerRational(ReadOnlySpan<byte> tiff, int offset, bool little)
    {
        var numerador = LeerU32(tiff, offset, little);
        var denominador = LeerU32(tiff, offset + 4, little);
        if (denominador == 0)
        {
            return null;
        }

        return (decimal)numerador / denominador;
    }

    private static ushort LeerU16(ReadOnlySpan<byte> b, int o, bool little) =>
        little ? (ushort)(b[o] | (b[o + 1] << 8)) : (ushort)((b[o] << 8) | b[o + 1]);

    private static uint LeerU32(ReadOnlySpan<byte> b, int o, bool little) =>
        little
            ? (uint)(b[o] | (b[o + 1] << 8) | (b[o + 2] << 16) | (b[o + 3] << 24))
            : (uint)((b[o] << 24) | (b[o + 1] << 16) | (b[o + 2] << 8) | b[o + 3]);
}
