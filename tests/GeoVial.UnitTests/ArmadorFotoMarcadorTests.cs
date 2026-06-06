using GeoVial.Sync;
using FluentAssertions;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// US-15 / CU-09 — Agregar una foto a un marcador desde el carrusel: el armado fuerza la coordenada del marcador
/// (descarta el EXIF de la foto) para que el backend, por agrupación de radio (RN-02), la asocie a ese marcador.
/// </summary>
public class ArmadorFotoMarcadorTests
{
    private static readonly DateTime Momento = new(2026, 6, 6, 12, 0, 0, DateTimeKind.Utc);
    private readonly ArmadorFotoMarcador _armador = new();

    [Fact] // la captura queda georreferenciada SIEMPRE en la coordenada del marcador, no en el EXIF de la foto
    public void Usa_la_coordenada_del_marcador()
    {
        var capturaId = Guid.NewGuid();
        var relevamientoId = Guid.NewGuid();

        var captura = _armador.Armar(capturaId, relevamientoId, -34.6037m, -58.3816m, new byte[] { 1, 2, 3 }, "obra.jpg", Momento);

        captura.Should().NotBeNull();
        captura!.CapturaId.Should().Be(capturaId);
        captura.RelevamientoId.Should().Be(relevamientoId);
        captura.LatitudExif.Should().Be(-34.6037m);
        captura.LongitudExif.Should().Be(-58.3816m);
        captura.ReferenciaArchivo.Should().Be("obra.jpg");
        captura.Foto.Should().Equal(1, 2, 3);
        captura.Momento.Should().Be(Momento);
    }

    [Fact] // sin binario no hay nada que subir
    public void Binario_vacio_devuelve_null()
    {
        _armador.Armar(Guid.NewGuid(), Guid.NewGuid(), -34m, -58m, Array.Empty<byte>(), "x.jpg", Momento).Should().BeNull();
        _armador.Armar(Guid.NewGuid(), Guid.NewGuid(), -34m, -58m, null, "x.jpg", Momento).Should().BeNull();
    }

    [Theory] // RN-03: una coordenada de marcador fuera de rango se descarta (no se arma la captura)
    [InlineData(-91, -58)]
    [InlineData(91, -58)]
    [InlineData(-34, -181)]
    [InlineData(-34, 181)]
    public void Coordenada_fuera_de_rango_devuelve_null(double lat, double lon)
    {
        _armador.Armar(Guid.NewGuid(), Guid.NewGuid(), (decimal)lat, (decimal)lon, new byte[] { 1 }, "x.jpg", Momento)
            .Should().BeNull();
    }

    [Theory] // si la foto no trae nombre, se usa uno por defecto
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Nombre_vacio_usa_uno_por_defecto(string? nombre)
    {
        var captura = _armador.Armar(Guid.NewGuid(), Guid.NewGuid(), 0m, 0m, new byte[] { 1 }, nombre, Momento);

        captura.Should().NotBeNull();
        captura!.ReferenciaArchivo.Should().Be("foto.jpg");
    }
}
