using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoVial.Revision;
using GeoVial.Shared;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Ubicar una observación de la bandeja desde la app (S51, CU-05): el puente WebView→app (parser del mensaje
/// del mapa), el HTML del mapa de ubicación y el cliente REST. Núcleo testeable; el WebView es glue.
/// </summary>
public class ParseadorMensajeUbicacionTests
{
    [Fact] // mensaje válido: extrae la coordenada con punto decimal invariante
    public void Mensaje_valido_extrae_coordenada()
    {
        var c = ParseadorMensajeUbicacion.Intentar("geovial-ubicar://place?lat=-34.61&lon=-58.41");

        c.Should().NotBeNull();
        c!.Latitud.Should().Be(-34.61m);
        c.Longitud.Should().Be(-58.41m);
    }

    [Theory] // navegaciones que no son el mensaje de ubicación → null (la app las ignora como mensaje)
    [InlineData("https://tile.openstreetmap.org/1/2/3.png")]
    [InlineData("about:blank")]
    [InlineData("geovial-ubicar://place?lat=-34.6")] // falta lon
    [InlineData("geovial-ubicar://place?lat=abc&lon=-58.4")] // lat no numérica
    [InlineData("")]
    public void Navegacion_que_no_es_mensaje_devuelve_null(string url)
    {
        ParseadorMensajeUbicacion.Intentar(url).Should().BeNull();
    }

    [Theory] // coordenadas fuera de rango geográfico → null (ruido, no un punto real)
    [InlineData("geovial-ubicar://place?lat=-91&lon=0")]
    [InlineData("geovial-ubicar://place?lat=0&lon=200")]
    public void Coordenada_fuera_de_rango_devuelve_null(string url)
    {
        ParseadorMensajeUbicacion.Intentar(url).Should().BeNull();
    }
}

public class MapaUbicacionHtmlTests
{
    [Fact] // el HTML trae Leaflet + OSM y centra en el punto pedido (punto decimal invariante)
    public void Incluye_leaflet_osm_y_centro()
    {
        var html = MapaUbicacionHtml.Construir(-34.6, -58.4);

        html.Should().Contain("leaflet.js");
        html.Should().Contain("tile.openstreetmap.org/{z}/{x}/{y}.png");
        html.Should().Contain("setView([-34.6, -58.4]");
        html.Should().NotContain("__"); // no quedan tokens sin reemplazar
    }

    [Fact] // el mapa devuelve la coordenada por el esquema centinela al confirmar, y escucha el toque
    public void Usa_el_esquema_centinela_y_escucha_el_toque()
    {
        var html = MapaUbicacionHtml.Construir(-34.6, -58.4);

        html.Should().Contain(ParseadorMensajeUbicacion.Esquema + "://place?lat=");
        html.Should().Contain("mapa.on('click'");
    }

    [Fact] // el puente cierra el círculo: lo que arma el HTML lo entiende el parser
    public void El_html_y_el_parser_acuerdan_el_esquema()
    {
        var html = MapaUbicacionHtml.Construir(0, 0);
        // simula el mensaje que el JS construiría
        var mensaje = $"{ParseadorMensajeUbicacion.Esquema}://place?lat=-34.61&lon=-58.41";

        html.Should().Contain(ParseadorMensajeUbicacion.Esquema + "://place");
        ParseadorMensajeUbicacion.Intentar(mensaje).Should().NotBeNull();
    }
}

public class ClienteUbicacionManualTests
{
    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _codigo;
        public StubHandler(HttpStatusCode codigo) => _codigo = codigo;
        public HttpRequestMessage? Capturada { get; private set; }
        public string? Cuerpo { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Capturada = request;
            Cuerpo = request.Content is null ? null : await request.Content.ReadAsStringAsync(ct);
            return new HttpResponseMessage(_codigo);
        }
    }

    [Fact] // postea la coordenada al endpoint de ubicación y devuelve true si el backend la acepta
    public async Task Ubicar_postea_y_devuelve_true()
    {
        var handler = new StubHandler(HttpStatusCode.NoContent);
        var cliente = new ClienteUbicacionManual(new HttpClient(handler) { BaseAddress = new Uri("https://geovial/") });
        var obs = Guid.NewGuid();

        var ok = await cliente.UbicarAsync(obs, -34.61m, -58.41m);

        ok.Should().BeTrue();
        handler.Capturada!.Method.Should().Be(HttpMethod.Post);
        handler.Capturada.RequestUri!.AbsolutePath.Should().Be($"/api/v1/observaciones/{obs}/ubicacion");
        handler.Cuerpo.Should().Contain("-34.61").And.Contain("-58.41");
    }

    [Fact] // ante un error del backend devuelve false (no rompe; la observación sigue en la bandeja)
    public async Task Ubicar_con_error_devuelve_false()
    {
        var handler = new StubHandler(HttpStatusCode.BadRequest);
        var cliente = new ClienteUbicacionManual(new HttpClient(handler) { BaseAddress = new Uri("https://geovial/") });

        (await cliente.UbicarAsync(Guid.NewGuid(), -34.6m, -58.4m)).Should().BeFalse();
    }
}
