using System.Globalization;
using FluentAssertions;
using GeoVial.Revision;
using GeoVial.Shared;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// HTML del mapa de la pantalla de captura (evolución de H-01): Leaflet + OSM con los marcadores del relevamiento
/// como contexto y un pin de captura que el agente coloca tocando el mapa, avisando por el esquema centinela.
/// </summary>
public class MapaCapturaHtmlTests
{
    private static RevisionMarcadorDto Marcador(decimal lat, decimal lon, bool enConflicto = false) =>
        new(Guid.NewGuid(), lat, lon, enConflicto, Array.Empty<RevisionFotoDto>(), Array.Empty<RevisionComentarioDto>());

    [Fact] // el HTML siempre incluye Leaflet y las teselas de OSM con su atribución (sin clave)
    public void Incluye_leaflet_y_osm_con_atribucion()
    {
        var html = MapaCapturaHtml.Construir(new VistaMapa(Array.Empty<RevisionMarcadorDto>()));

        html.Should().Contain("leaflet.js");
        html.Should().Contain("tile.openstreetmap.org/{z}/{x}/{y}.png");
        html.Should().Contain("OpenStreetMap");
        html.Should().NotContain("__"); // no quedan tokens sin reemplazar
    }

    [Fact] // tocar el mapa avisa la coordenada por el esquema centinela; hay pin de captura y escucha del toque
    public void Escucha_el_toque_y_usa_el_esquema_centinela()
    {
        var html = MapaCapturaHtml.Construir(new VistaMapa(Array.Empty<RevisionMarcadorDto>()));

        html.Should().Contain("mapa.on('click'");
        html.Should().Contain(ParseadorMensajeUbicacion.Esquema + "://place?lat=");
        html.Should().Contain("draggable: true"); // el pin de captura se puede arrastrar para afinar
    }

    [Fact] // el puente cierra el círculo: el mensaje que arma el HTML lo entiende el parser
    public void El_html_y_el_parser_acuerdan_el_esquema()
    {
        var html = MapaCapturaHtml.Construir(new VistaMapa(Array.Empty<RevisionMarcadorDto>()));
        var mensaje = $"{ParseadorMensajeUbicacion.Esquema}://place?lat=-34.61&lon=-58.41";

        html.Should().Contain(ParseadorMensajeUbicacion.Esquema + "://place");
        ParseadorMensajeUbicacion.Intentar(mensaje).Should().NotBeNull();
    }

    [Fact] // sin marcadores: no hay pines de contexto y centra en el por defecto
    public void Sin_marcadores_centra_por_defecto()
    {
        var html = MapaCapturaHtml.Construir(new VistaMapa(Array.Empty<RevisionMarcadorDto>()));

        html.Should().Contain("var pines = []");
        html.Should().Contain($"setView([{VistaMapa.CentroPorDefectoLat.ToString(CultureInfo.InvariantCulture)}");
    }

    [Fact] // un marcador de contexto: su coordenada queda embebida (con punto decimal invariante)
    public void Un_marcador_de_contexto_embebe_su_coordenada()
    {
        var html = MapaCapturaHtml.Construir(new VistaMapa(new[] { Marcador(-34.6m, -58.4m, enConflicto: true) }));

        html.Should().Contain("\"lat\":-34.6");
        html.Should().Contain("\"lon\":-58.4");
        html.Should().Contain("\"conflicto\":true");
    }

    [Fact] // varios marcadores de contexto: se encuadra a la caja contenedora (bounds)
    public void Varios_marcadores_usan_bounds()
    {
        var html = MapaCapturaHtml.Construir(new VistaMapa(new[]
        {
            Marcador(-34.0m, -58.0m),
            Marcador(-36.0m, -60.0m),
        }));

        html.Should().Contain("fitBounds([[-36, -60], [-34, -58]]");
    }

    [Fact] // vista nula: falla con argumento nulo
    public void Vista_nula_falla()
    {
        var crear = () => MapaCapturaHtml.Construir(null!);
        crear.Should().Throw<ArgumentNullException>();
    }
}
