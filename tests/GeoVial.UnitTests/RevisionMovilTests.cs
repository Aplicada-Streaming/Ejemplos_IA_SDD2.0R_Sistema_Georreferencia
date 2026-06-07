using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using GeoVial.Revision;
using GeoVial.Shared;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>AT-08 — Navegación del carrusel de revisión sobre mapa en el cliente móvil (US-21, US-22, CU-08).</summary>
public class NavegadorRevisionTests
{
    private static RevisionMarcadorDto Marcador(params string[] fotos) =>
        new(Guid.NewGuid(), -34.6m, -58.4m, EnConflicto: false,
            fotos.Select(r => new RevisionFotoDto(Guid.NewGuid(), r, Array.Empty<string>())).ToList(),
            Array.Empty<RevisionComentarioDto>());

    private static RevisionRelevamientoDto Revision(params RevisionMarcadorDto[] marcadores) =>
        new(Guid.NewGuid(), Estado: 1, marcadores, Array.Empty<Guid>());

    [Fact] // un relevamiento sin marcadores no rompe la navegación
    public void Sin_marcadores_actual_es_null()
    {
        var nav = new NavegadorRevision(Revision());

        nav.HayMarcadores.Should().BeFalse();
        nav.MarcadorActual.Should().BeNull();
        nav.FotoActual.Should().BeNull();
        nav.SiguienteMarcador(); // no-op
        nav.MarcadorActual.Should().BeNull();
    }

    [Fact] // US-22: el carrusel de marcadores es circular en ambos sentidos
    public void Marcadores_circulan_en_ambos_sentidos()
    {
        var m0 = Marcador("a.jpg");
        var m1 = Marcador("b.jpg");
        var m2 = Marcador("c.jpg");
        var nav = new NavegadorRevision(Revision(m0, m1, m2));

        nav.MarcadorActual.Should().Be(m0);
        nav.SiguienteMarcador();
        nav.MarcadorActual.Should().Be(m1);
        nav.SiguienteMarcador();
        nav.SiguienteMarcador(); // desde el último vuelve al primero
        nav.MarcadorActual.Should().Be(m0);
        nav.AnteriorMarcador(); // desde el primero va al último
        nav.MarcadorActual.Should().Be(m2);
    }

    [Fact] // US-22: dentro de un marcador, las fotos circulan
    public void Fotos_del_marcador_circulan()
    {
        var nav = new NavegadorRevision(Revision(Marcador("a.jpg", "b.jpg")));

        nav.FotoActual!.ReferenciaArchivo.Should().Be("a.jpg");
        nav.SiguienteFoto();
        nav.FotoActual!.ReferenciaArchivo.Should().Be("b.jpg");
        nav.SiguienteFoto(); // circular: vuelve a la primera
        nav.FotoActual!.ReferenciaArchivo.Should().Be("a.jpg");
        nav.AnteriorFoto();
        nav.FotoActual!.ReferenciaArchivo.Should().Be("b.jpg");
    }

    [Fact] // cambiar de marcador reinicia la foto en foco
    public void Cambiar_marcador_reinicia_la_foto()
    {
        var nav = new NavegadorRevision(Revision(Marcador("a.jpg", "b.jpg"), Marcador("c.jpg", "d.jpg")));

        nav.SiguienteFoto();
        nav.IndiceFoto.Should().Be(1);
        nav.SiguienteMarcador();
        nav.IndiceFoto.Should().Be(0);
        nav.FotoActual!.ReferenciaArchivo.Should().Be("c.jpg");
    }

    [Fact] // un marcador sin fotos: foto actual null y navegar fotos es no-op
    public void Marcador_sin_fotos_no_rompe()
    {
        var nav = new NavegadorRevision(Revision(Marcador()));

        nav.MarcadorActual.Should().NotBeNull();
        nav.FotoActual.Should().BeNull();
        nav.SiguienteFoto(); // no-op
        nav.FotoActual.Should().BeNull();
    }

    [Fact] // H-04: Avanzar pasa a la foto siguiente dentro del marcador
    public void Avanzar_dentro_del_marcador()
    {
        var nav = new NavegadorRevision(Revision(Marcador("a.jpg", "b.jpg")));

        nav.Avanzar();

        nav.IndiceMarcador.Should().Be(0);
        nav.FotoActual!.ReferenciaArchivo.Should().Be("b.jpg");
    }

    [Fact] // H-04: Avanzar en la última foto cruza al marcador siguiente (primera foto)
    public void Avanzar_en_ultima_foto_cruza_de_marcador()
    {
        var nav = new NavegadorRevision(Revision(Marcador("a.jpg", "b.jpg"), Marcador("c.jpg", "d.jpg")));

        nav.Avanzar(); // a → b
        nav.Avanzar(); // b (última) → marcador 2, c

        nav.IndiceMarcador.Should().Be(1);
        nav.FotoActual!.ReferenciaArchivo.Should().Be("c.jpg");
    }

    [Fact] // H-04: Retroceder en la primera foto cruza al marcador anterior (ÚLTIMA foto)
    public void Retroceder_en_primera_foto_cruza_a_la_ultima_del_anterior()
    {
        var nav = new NavegadorRevision(Revision(Marcador("a.jpg", "b.jpg"), Marcador("c.jpg", "d.jpg")));
        nav.SiguienteMarcador(); // marcador 2, foto c (idx 0)

        nav.Retroceder(); // primera foto → marcador 1, última foto (b)

        nav.IndiceMarcador.Should().Be(0);
        nav.FotoActual!.ReferenciaArchivo.Should().Be("b.jpg");
    }

    [Fact] // H-04: Avanzar es circular sobre todo el relevamiento
    public void Avanzar_circular_vuelve_al_inicio()
    {
        var nav = new NavegadorRevision(Revision(Marcador("a.jpg"), Marcador("b.jpg")));

        nav.Avanzar(); // marcador 1 (única, última) → marcador 2, b
        nav.IndiceMarcador.Should().Be(1);
        nav.Avanzar(); // marcador 2 (última) → vuelve al marcador 1, a
        nav.IndiceMarcador.Should().Be(0);
        nav.FotoActual!.ReferenciaArchivo.Should().Be("a.jpg");
    }

    [Fact] // H-04: un marcador sin fotos: Avanzar cruza directo al siguiente marcador
    public void Avanzar_marcador_sin_fotos_cruza()
    {
        var nav = new NavegadorRevision(Revision(Marcador(), Marcador("c.jpg")));

        nav.Avanzar();

        nav.IndiceMarcador.Should().Be(1);
        nav.FotoActual!.ReferenciaArchivo.Should().Be("c.jpg");
    }
}

/// <summary>Cliente HTTP de la API de revisión (US-21, CU-08).</summary>
public class ClienteRevisionHttpTests
{
    private sealed class StubHandler(string respuesta) : HttpMessageHandler
    {
        public Uri? Capturada { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Capturada = request.RequestUri;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(respuesta, Encoding.UTF8, "application/json"),
            });
        }
    }

    [Fact] // obtiene la revisión y la parsea; el filtro de etiquetas viaja en la query
    public async Task Obtiene_y_parsea_la_revision_con_filtro()
    {
        var relevamientoId = Guid.NewGuid();
        var marcadorId = Guid.NewGuid();
        var web = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var dto = new RevisionRelevamientoDto(relevamientoId, 1,
            new[]
            {
                new RevisionMarcadorDto(marcadorId, -34.6m, -58.4m, false,
                    new[] { new RevisionFotoDto(Guid.NewGuid(), "foto.jpg", new[] { "fisura" }) },
                    Array.Empty<RevisionComentarioDto>()),
            },
            Array.Empty<Guid>());

        var handler = new StubHandler(JsonSerializer.Serialize(dto, web));
        var cliente = new ClienteRevisionHttp(new HttpClient(handler) { BaseAddress = new Uri("https://geovial/") });

        var r = await cliente.ObtenerAsync(relevamientoId, "fisura");

        r.Should().NotBeNull();
        r!.Marcadores.Should().ContainSingle().Which.MarcadorId.Should().Be(marcadorId);
        handler.Capturada!.AbsolutePath.Should().Be($"/api/v1/relevamientos/{relevamientoId}/revision");
        handler.Capturada.Query.Should().Contain("etiquetas=fisura");
    }

    [Fact] // sin filtro, la query queda vacía
    public async Task Sin_filtro_no_agrega_query()
    {
        var web = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var dto = new RevisionRelevamientoDto(Guid.NewGuid(), 1, Array.Empty<RevisionMarcadorDto>(), Array.Empty<Guid>());
        var handler = new StubHandler(JsonSerializer.Serialize(dto, web));
        var cliente = new ClienteRevisionHttp(new HttpClient(handler) { BaseAddress = new Uri("https://geovial/") });

        await cliente.ObtenerAsync(Guid.NewGuid());

        handler.Capturada!.Query.Should().BeEmpty();
    }
}
