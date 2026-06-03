using FluentAssertions;
using GeoVial.Revision;
using GeoVial.Shared;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>HTML del mapa de la revisión para el WebView del móvil (US-21): Leaflet + OSM con los marcadores.</summary>
public class MapaRevisionHtmlTests
{
    private static RevisionMarcadorDto Marcador(decimal lat, decimal lon, bool enConflicto = false, int fotos = 0, int comentarios = 0) =>
        new(Guid.NewGuid(), lat, lon, enConflicto,
            Enumerable.Range(0, fotos).Select(_ => new RevisionFotoDto(Guid.NewGuid(), "f.jpg", Array.Empty<string>())).ToArray(),
            Enumerable.Range(0, comentarios).Select(_ => new RevisionComentarioDto(Guid.NewGuid(), "c", null, Array.Empty<string>())).ToArray());

    [Fact] // el HTML siempre incluye Leaflet y las teselas de OSM con su atribución (sin clave)
    public void Incluye_leaflet_y_osm_con_atribucion()
    {
        var html = MapaRevisionHtml.Construir(new VistaMapa(Array.Empty<RevisionMarcadorDto>()));

        html.Should().Contain("leaflet.js");
        html.Should().Contain("tile.openstreetmap.org/{z}/{x}/{y}.png");
        html.Should().Contain("OpenStreetMap");
        html.Should().NotContain("__"); // no quedan tokens sin reemplazar
    }

    [Fact] // sin marcadores: no hay pines y centra en el por defecto
    public void Sin_marcadores_centra_por_defecto()
    {
        var html = MapaRevisionHtml.Construir(new VistaMapa(Array.Empty<RevisionMarcadorDto>()));

        html.Should().Contain("var pines = []");
        html.Should().Contain($"setView([{VistaMapa.CentroPorDefectoLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
    }

    [Fact] // un marcador: su coordenada queda embebida en el JSON de pines (con punto decimal invariante)
    public void Un_marcador_embebe_su_coordenada()
    {
        var html = MapaRevisionHtml.Construir(new VistaMapa(new[] { Marcador(-34.6m, -58.4m, enConflicto: true, fotos: 2, comentarios: 1) }));

        html.Should().Contain("\"lat\":-34.6");
        html.Should().Contain("\"lon\":-58.4");
        html.Should().Contain("\"conflicto\":true");
        html.Should().Contain("\"fotos\":2");
        html.Should().Contain("\"comentarios\":1");
    }

    [Fact] // varios marcadores: se encuadra a la caja contenedora (bounds)
    public void Varios_marcadores_usan_bounds()
    {
        var html = MapaRevisionHtml.Construir(new VistaMapa(new[]
        {
            Marcador(-34.0m, -58.0m),
            Marcador(-36.0m, -60.0m),
        }));

        html.Should().Contain("fitBounds([[-36, -60], [-34, -58]]");
    }

    [Fact] // vista nula: falla con argumento nulo
    public void Vista_nula_falla()
    {
        var crear = () => MapaRevisionHtml.Construir(null!);
        crear.Should().Throw<ArgumentNullException>();
    }
}
