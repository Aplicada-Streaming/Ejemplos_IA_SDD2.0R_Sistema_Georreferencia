using FluentAssertions;
using GeoVial.Revision;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Caché en disco de teselas para offline en el móvil (US-21): round-trip, miss y descarte.</summary>
public sealed class CacheTeselasDiscoTests : IDisposable
{
    private readonly string _carpeta = Path.Combine(Path.GetTempPath(), $"geovial-teselas-{Guid.NewGuid():N}");

    private const string Url = "https://tile.openstreetmap.org/12/2010/1252.png";

    [Fact] // guardar y recuperar los bytes por URL
    public void Guardar_y_recuperar()
    {
        var cache = new CacheTeselasDisco(_carpeta);

        cache.Guardar(Url, new byte[] { 1, 2, 3 });

        cache.Obtener(Url).Should().Equal(1, 2, 3);
        cache.Cantidad.Should().Be(1);
    }

    [Fact] // una URL no cacheada devuelve null
    public void Url_ausente_devuelve_null()
    {
        new CacheTeselasDisco(_carpeta).Obtener(Url).Should().BeNull();
    }

    [Fact] // guardar la misma URL actualiza los bytes sin crecer
    public void Guardar_misma_url_actualiza()
    {
        var cache = new CacheTeselasDisco(_carpeta);

        cache.Guardar(Url, new byte[] { 1 });
        cache.Guardar(Url, new byte[] { 9, 9 });

        cache.Cantidad.Should().Be(1);
        cache.Obtener(Url).Should().Equal(9, 9);
    }

    [Fact] // al exceder la capacidad, la caché se acota descartando las más viejas
    public void Excede_capacidad_descarta_las_viejas()
    {
        var cache = new CacheTeselasDisco(_carpeta, capacidad: 5);

        for (var i = 0; i < 12; i++)
        {
            cache.Guardar($"https://tile.openstreetmap.org/12/{i}/1252.png", new byte[] { (byte)i });
        }

        cache.Cantidad.Should().BeLessThanOrEqualTo(5);
        // la última guardada sigue presente (las descartadas son las más viejas)
        cache.Obtener("https://tile.openstreetmap.org/12/11/1252.png").Should().NotBeNull();
    }

    [Fact] // capacidad inválida y carpeta vacía fallan
    public void Argumentos_invalidos_fallan()
    {
        var capInvalida = () => new CacheTeselasDisco(_carpeta, 0);
        capInvalida.Should().Throw<ArgumentOutOfRangeException>();

        var carpetaVacia = () => new CacheTeselasDisco("  ");
        carpetaVacia.Should().Throw<ArgumentException>();
    }

    public void Dispose()
    {
        if (Directory.Exists(_carpeta))
        {
            Directory.Delete(_carpeta, recursive: true);
        }
    }
}
