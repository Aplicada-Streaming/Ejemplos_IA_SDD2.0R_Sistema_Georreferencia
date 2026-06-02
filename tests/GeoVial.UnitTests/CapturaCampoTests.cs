using FluentAssertions;
using GeoVial.CapturaCampo;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Constructor hermético de un JPEG con metadatos EXIF-GPS, para probar el lector sin binarios de fixture.
/// Escribe la estructura SOI + APP1(Exif + TIFF con GPS IFD) + EOI según la especificación EXIF 2.3, de forma
/// independiente del parser (el helper codifica; el lector decodifica).
/// </summary>
internal static class ConstructorJpegExif
{
    public static byte[] Crear(bool little, char latRef, int latD, int latM, decimal latS, char lonRef, int lonD, int lonM, decimal lonS)
    {
        // TIFF de 128 bytes con offsets fijos (ver layout en el plan del Sprint 14).
        var tiff = new byte[128];
        tiff[0] = (byte)(little ? 'I' : 'M');
        tiff[1] = (byte)(little ? 'I' : 'M');
        U16(tiff, 2, 42, little);   // magic
        U32(tiff, 4, 8, little);    // offset a IFD0

        // IFD0: 1 entrada (puntero al GPS IFD).
        U16(tiff, 8, 1, little);
        Entrada(tiff, 10, 0x8825, 4, 1, little); U32(tiff, 18, 26, little); // tag, type=LONG, count=1, value=offset GPS IFD
        U32(tiff, 22, 0, little);   // siguiente IFD = 0

        // GPS IFD en 26: 4 entradas.
        U16(tiff, 26, 4, little);
        Entrada(tiff, 28, 0x0001, 2, 2, little); tiff[36] = (byte)latRef;  // LatitudeRef (ASCII inline)
        Entrada(tiff, 40, 0x0002, 5, 3, little); U32(tiff, 48, 80, little);  // Latitude (3 rationals → offset 80)
        Entrada(tiff, 52, 0x0003, 2, 2, little); tiff[60] = (byte)lonRef;  // LongitudeRef
        Entrada(tiff, 64, 0x0004, 5, 3, little); U32(tiff, 72, 104, little); // Longitude (3 rationals → offset 104)
        U32(tiff, 76, 0, little);   // siguiente IFD = 0

        // Datos de los rationals (grados/1, minutos/1, segundos*100/100).
        Dms(tiff, 80, latD, latM, latS, little);
        Dms(tiff, 104, lonD, lonM, lonS, little);

        // Envoltorio JPEG: SOI + APP1(Exif\0\0 + TIFF) + EOI.
        var exif = new byte[6 + tiff.Length];
        exif[0] = (byte)'E'; exif[1] = (byte)'x'; exif[2] = (byte)'i'; exif[3] = (byte)'f';
        Array.Copy(tiff, 0, exif, 6, tiff.Length);

        var longitudApp1 = exif.Length + 2; // incluye los 2 bytes de longitud
        using var ms = new MemoryStream();
        ms.Write(new byte[] { 0xFF, 0xD8 }); // SOI
        ms.Write(new byte[] { 0xFF, 0xE1, (byte)(longitudApp1 >> 8), (byte)(longitudApp1 & 0xFF) });
        ms.Write(exif);
        ms.Write(new byte[] { 0xFF, 0xD9 }); // EOI
        return ms.ToArray();
    }

    private static void Entrada(byte[] b, int o, int tag, int tipo, int count, bool little)
    {
        U16(b, o, (ushort)tag, little);
        U16(b, o + 2, (ushort)tipo, little);
        U32(b, o + 4, (uint)count, little);
    }

    private static void Dms(byte[] b, int o, int d, int m, decimal s, bool little)
    {
        Rational(b, o, (uint)d, 1, little);
        Rational(b, o + 8, (uint)m, 1, little);
        Rational(b, o + 16, (uint)(s * 100m), 100, little); // segundos con 2 decimales
    }

    private static void Rational(byte[] b, int o, uint num, uint den, bool little)
    {
        U32(b, o, num, little);
        U32(b, o + 4, den, little);
    }

    private static void U16(byte[] b, int o, ushort v, bool little)
    {
        if (little) { b[o] = (byte)(v & 0xFF); b[o + 1] = (byte)(v >> 8); }
        else { b[o] = (byte)(v >> 8); b[o + 1] = (byte)(v & 0xFF); }
    }

    private static void U32(byte[] b, int o, uint v, bool little)
    {
        if (little)
        {
            b[o] = (byte)(v & 0xFF); b[o + 1] = (byte)((v >> 8) & 0xFF);
            b[o + 2] = (byte)((v >> 16) & 0xFF); b[o + 3] = (byte)((v >> 24) & 0xFF);
        }
        else
        {
            b[o] = (byte)((v >> 24) & 0xFF); b[o + 1] = (byte)((v >> 16) & 0xFF);
            b[o + 2] = (byte)((v >> 8) & 0xFF); b[o + 3] = (byte)(v & 0xFF);
        }
    }
}

/// <summary>AT-04 — Lectura de la coordenada EXIF y armado de la captura del cliente móvil (US-11, RN-03).</summary>
public class LectorGpsExifTests
{
    private readonly LectorGpsExif _lector = new();

    [Fact] // RN-03: deriva la coordenada de los metadatos (little-endian), con signo según hemisferio S/W
    public void Lee_coordenada_little_endian_hemisferio_sur_oeste()
    {
        // 34°36'00"S, 58°22'12"W → -34,6 / -58,37 (exactos)
        var jpeg = ConstructorJpegExif.Crear(little: true, 'S', 34, 36, 0m, 'W', 58, 22, 12m);

        var coord = _lector.Extraer(jpeg);

        coord.Should().NotBeNull();
        coord!.Value.Latitud.Should().Be(-34.6m);
        coord.Value.Longitud.Should().Be(-58.37m);
    }

    [Fact] // soporta el orden de bytes big-endian (MM) y el hemisferio N/E
    public void Lee_coordenada_big_endian_hemisferio_norte_este()
    {
        // 10°30'00"N, 20°15'00"E → 10,5 / 20,25
        var jpeg = ConstructorJpegExif.Crear(little: false, 'N', 10, 30, 0m, 'E', 20, 15, 0m);

        var coord = _lector.Extraer(jpeg);

        coord.Should().NotBeNull();
        coord!.Value.Latitud.Should().Be(10.5m);
        coord.Value.Longitud.Should().Be(20.25m);
    }

    [Fact] // combina grados/minutos/segundos fraccionarios a grados decimales
    public void Combina_grados_minutos_segundos()
    {
        var jpeg = ConstructorJpegExif.Crear(little: true, 'N', 40, 26, 46.8m, 'E', 3, 42, 12m);

        var coord = _lector.Extraer(jpeg)!.Value;

        Math.Round(coord.Latitud, 5).Should().Be(40.44633m);  // 40 + 26/60 + 46,8/3600
        Math.Round(coord.Longitud, 5).Should().Be(3.70333m);  // 3 + 42/60 + 12/3600
    }

    [Fact] // una foto que no es JPEG no aporta coordenada
    public void Sin_jpeg_devuelve_null()
    {
        _lector.Extraer(new byte[] { 1, 2, 3, 4, 5 }).Should().BeNull();
    }

    [Fact] // un JPEG sin segmento EXIF no aporta coordenada
    public void Jpeg_sin_exif_devuelve_null()
    {
        var jpeg = new byte[] { 0xFF, 0xD8, 0xFF, 0xD9 }; // SOI + EOI, sin APP1
        _lector.Extraer(jpeg).Should().BeNull();
    }

    [Fact] // entrada vacía no rompe
    public void Vacio_devuelve_null()
    {
        _lector.Extraer(ReadOnlySpan<byte>.Empty).Should().BeNull();
    }

    [Fact] // recorre y saltea un segmento previo (APP0/JFIF) antes de encontrar el APP1/Exif
    public void Saltea_segmento_app0_antes_del_exif()
    {
        var conExif = ConstructorJpegExif.Crear(little: true, 'N', 1, 0, 0m, 'E', 2, 0, 0m);
        // APP0 (FF E0) de 18 bytes insertado tras el SOI: el lector debe saltearlo y llegar al APP1.
        var app0 = new byte[] { 0xFF, 0xE0, 0x00, 0x10 }.Concat(new byte[14]).ToArray();
        var jpeg = conExif.Take(2).Concat(app0).Concat(conExif.Skip(2)).ToArray();

        var coord = _lector.Extraer(jpeg);

        coord.Should().NotBeNull();
        coord!.Value.Latitud.Should().Be(1m);
        coord.Value.Longitud.Should().Be(2m);
    }

    [Fact] // un APP1 cuya longitud excede el buffer (truncado) no rompe
    public void App1_truncado_devuelve_null()
    {
        var jpeg = ConstructorJpegExif.Crear(little: true, 'N', 1, 0, 0m, 'E', 2, 0, 0m);
        Array.Resize(ref jpeg, jpeg.Length - 30); // se corta el cuerpo del APP1; la longitud declarada ya no entra
        _lector.Extraer(jpeg).Should().BeNull();
    }

    [Fact] // un orden de bytes TIFF inválido (ni II ni MM) no aporta coordenada
    public void Tiff_con_orden_de_bytes_invalido_devuelve_null()
    {
        var jpeg = ConstructorJpegExif.Crear(little: true, 'N', 1, 0, 0m, 'E', 2, 0, 0m);
        jpeg[12] = (byte)'X'; jpeg[13] = (byte)'X'; // el TIFF empieza en el offset 12 (SOI+APP1+"Exif\0\0")
        _lector.Extraer(jpeg).Should().BeNull();
    }

    [Fact] // un rational con denominador 0 (dato corrupto) no rompe
    public void Rational_con_denominador_cero_devuelve_null()
    {
        var jpeg = ConstructorJpegExif.Crear(little: true, 'N', 1, 30, 0m, 'E', 2, 0, 0m);
        // Denominador del primer rational de latitud: TIFF offset 84 → jpeg offset 96 (little-endian).
        jpeg[96] = 0; jpeg[97] = 0; jpeg[98] = 0; jpeg[99] = 0;
        _lector.Extraer(jpeg).Should().BeNull();
    }
}

public class ArmadorCapturaCampoTests
{
    private sealed class ExtractorFijo(CoordenadaExif? valor) : IExtractorGpsExif
    {
        public CoordenadaExif? Extraer(ReadOnlySpan<byte> foto) => valor;
    }

    [Fact] // US-11: con coordenada válida arma una petición georreferenciada
    public void Con_coordenada_arma_peticion_georreferenciada()
    {
        var armador = new ArmadorCapturaCampo(new ExtractorFijo(new CoordenadaExif(-34.6m, -58.37m)));

        var r = armador.Armar(new byte[] { 0xFF, 0xD8 }, "obra/foto-1.jpg");

        r.Georreferenciada.Should().BeTrue();
        r.Peticion.ReferenciaArchivo.Should().Be("obra/foto-1.jpg");
        r.Peticion.LatitudExif.Should().Be(-34.6m);
        r.Peticion.LongitudExif.Should().Be(-58.37m);
    }

    [Fact] // US-11 CA-02: sin coordenada, la petición va sin georreferencia (bandeja)
    public void Sin_coordenada_arma_peticion_sin_georreferencia()
    {
        var armador = new ArmadorCapturaCampo(new ExtractorFijo(null));

        var r = armador.Armar(new byte[] { 0xFF, 0xD8 }, "obra/foto-2.jpg");

        r.Georreferenciada.Should().BeFalse();
        r.Peticion.LatitudExif.Should().BeNull();
        r.Peticion.LongitudExif.Should().BeNull();
    }

    [Theory] // RN-03: una coordenada fuera de rango se descarta como inválida (va a la bandeja)
    [InlineData(95, 0)]    // latitud > 90
    [InlineData(-91, 0)]   // latitud < -90
    [InlineData(0, 200)]   // longitud > 180
    [InlineData(0, -181)]  // longitud < -180
    public void Coordenada_fuera_de_rango_se_trata_como_sin_georreferencia(int lat, int lon)
    {
        var armador = new ArmadorCapturaCampo(new ExtractorFijo(new CoordenadaExif(lat, lon)));

        var r = armador.Armar(new byte[] { 0xFF, 0xD8 }, "x.jpg");

        r.Georreferenciada.Should().BeFalse();
        r.Peticion.LatitudExif.Should().BeNull();
    }
}
