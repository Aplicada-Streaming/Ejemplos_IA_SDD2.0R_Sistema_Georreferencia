using FluentAssertions;
using GeoVial.Revision;
using GeoVial.Shared;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Pin tocable → abrir el carrusel del marcador (S55): el HTML del mapa avisa qué marcador se tocó por un
/// esquema centinela y la app lo parsea. Espejo del puente de ubicación.
/// </summary>
public class ParseadorMensajeMarcadorTests
{
    [Fact] // mensaje válido: extrae el id del marcador
    public void Mensaje_valido_extrae_id()
    {
        var id = Guid.NewGuid();
        ParseadorMensajeMarcador.Intentar($"geovial-marcador://abrir?id={id}").Should().Be(id);
    }

    [Theory] // navegaciones que no son el mensaje → null
    [InlineData("https://tile.openstreetmap.org/1/2/3.png")]
    [InlineData("geovial-marcador://abrir?id=no-es-guid")]
    [InlineData("geovial-marcador://abrir")]
    [InlineData("geovial-ubicar://place?lat=-34.6&lon=-58.4")]
    [InlineData("")]
    public void Navegacion_que_no_es_mensaje_devuelve_null(string url) =>
        ParseadorMensajeMarcador.Intentar(url).Should().BeNull();
}

public class MapaRevisionHtmlMarcadorTests
{
    private static RevisionMarcadorDto Marcador(Guid id) =>
        new(id, -34.6m, -58.4m, EnConflicto: false,
            new[] { new RevisionFotoDto(Guid.NewGuid(), "f.jpg", System.Array.Empty<string>()) },
            System.Array.Empty<RevisionComentarioDto>());

    [Fact] // S55: el pin embebe su id y un click que avisa por el esquema centinela; lo entiende el parser
    public void El_pin_es_tocable_y_acuerda_con_el_parser()
    {
        var id = Guid.NewGuid();
        var html = MapaRevisionHtml.Construir(new VistaMapa(new[] { Marcador(id) }));

        html.Should().Contain($"\"id\":\"{id}\"");
        html.Should().Contain("m.on('click'");
        html.Should().Contain(ParseadorMensajeMarcador.Esquema + "://abrir?id=");
        html.Should().NotContain("__"); // sin tokens sin reemplazar

        // El mensaje que arma el JS lo entiende el parser (puente cerrado).
        ParseadorMensajeMarcador.Intentar($"{ParseadorMensajeMarcador.Esquema}://abrir?id={id}").Should().Be(id);
    }
}
