using FluentAssertions;
using GeoVial.Revision;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Andamiaje HTML compartido de los mapas Leaflet+OSM (control de mapa compartido): un único lugar para la
/// cabecera de Leaflet, las teselas de OSM y la estructura del documento; cada mapa aporta estilos/cuerpo/script.
/// </summary>
public class MapaLeafletTests
{
    [Fact] // el documento incluye Leaflet, las teselas de OSM con su atribución y crea el mapa una vez
    public void Documento_incluye_leaflet_teselas_y_crea_el_mapa()
    {
        var html = MapaLeaflet.Documento("#mapa { height: 100%; }", "<div id=\"mapa\"></div>", "/* sin script */");

        html.Should().Contain("leaflet.js");
        html.Should().Contain("tile.openstreetmap.org/{z}/{x}/{y}.png");
        html.Should().Contain("OpenStreetMap");
        html.Should().Contain("var mapa = L.map('mapa');");
        html.Should().Contain("L.tileLayer(");
    }

    [Fact] // inyecta los estilos, el cuerpo y el script del mapa concreto, sin dejar tokens del andamiaje
    public void Documento_inyecta_estilos_cuerpo_y_script()
    {
        var html = MapaLeaflet.Documento("#mapa { height: 50%; }", "<div id=\"mapa\"></div><div id=\"barra\"></div>", "mapa.setView([0, 0], 3);");

        html.Should().Contain("#mapa { height: 50%; }");
        html.Should().Contain("<div id=\"barra\"></div>");
        html.Should().Contain("mapa.setView([0, 0], 3);");
        html.Should().NotContain("__"); // no quedan tokens del andamiaje sin reemplazar
    }
}
