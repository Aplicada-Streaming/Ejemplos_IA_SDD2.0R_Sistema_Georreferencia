using FluentAssertions;
using GeoVial.Revision;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Configuración y contrato de teselas del mapa (US-21): qué se cachea offline (Service Worker).</summary>
public class MapaTeselasTests
{
    [Fact] // la URL y la atribución apuntan a OpenStreetMap (libre, sin clave)
    public void Config_apunta_a_openstreetmap()
    {
        MapaTeselas.UrlPlantilla.Should().Contain("tile.openstreetmap.org/{z}/{x}/{y}.png");
        MapaTeselas.Atribucion.Should().Contain("OpenStreetMap");
        MapaTeselas.Host.Should().Be("tile.openstreetmap.org");
    }

    [Theory] // una tesela de OSM (host + .png) es cacheable
    [InlineData("https://tile.openstreetmap.org/12/2010/1252.png")]
    [InlineData("https://TILE.OpenStreetMap.org/5/10/12.PNG")]
    public void Url_de_tesela_es_cacheable(string url)
    {
        MapaTeselas.EsUrlDeTesela(url).Should().BeTrue();
    }

    [Theory] // otras URLs no se cachean como tesela
    [InlineData("https://tile.openstreetmap.org/copyright")]        // mismo host, no es .png
    [InlineData("https://example.com/12/2010/1252.png")]            // otro host
    [InlineData("https://unpkg.com/leaflet@1.9.4/dist/leaflet.js")] // la librería, no una tesela
    [InlineData("no-es-una-url")]                                   // no es URL absoluta
    public void Url_que_no_es_tesela_no_se_cachea(string url)
    {
        MapaTeselas.EsUrlDeTesela(url).Should().BeFalse();
    }
}
