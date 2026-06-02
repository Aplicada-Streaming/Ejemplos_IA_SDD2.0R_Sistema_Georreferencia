using System.Net;
using GeoVial.Revision;
using FluentAssertions;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>AT-09 — Edición sobre el marcador desde el móvil: comentarios y etiquetas (US-15, CU-09).</summary>
public class ClienteEdicionMarcadorTests
{
    private sealed class StubHandler(HttpStatusCode estado) : HttpMessageHandler
    {
        public int Llamadas { get; private set; }
        public Uri? UltimaUri { get; private set; }
        public string? UltimoCuerpo { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Llamadas++;
            UltimaUri = request.RequestUri;
            UltimoCuerpo = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(estado);
        }
    }

    private static (ClienteEdicionMarcador Cliente, StubHandler Handler) Crear(HttpStatusCode estado = HttpStatusCode.Created)
    {
        var handler = new StubHandler(estado);
        var cliente = new ClienteEdicionMarcador(new HttpClient(handler) { BaseAddress = new Uri("https://geovial/") });
        return (cliente, handler);
    }

    [Fact] // US-15: agregar un comentario al marcador postea al endpoint con el texto
    public async Task Agregar_comentario_postea_al_marcador()
    {
        var (cliente, handler) = Crear();
        var marcadorId = Guid.NewGuid();

        var r = await cliente.AgregarComentarioAsync(marcadorId, null, "  Fisura en la viga  ");

        r.Exito.Should().BeTrue();
        handler.UltimaUri!.AbsolutePath.Should().Be($"/api/v1/marcadores/{marcadorId}/comentarios");
        handler.UltimoCuerpo.Should().Contain("Fisura en la viga"); // texto recortado y enviado
    }

    [Fact] // validación local: un comentario vacío no llama al backend
    public async Task Comentario_vacio_no_llama_al_backend()
    {
        var (cliente, handler) = Crear();

        var r = await cliente.AgregarComentarioAsync(Guid.NewGuid(), null, "   ");

        r.Exito.Should().BeFalse();
        handler.Llamadas.Should().Be(0);
    }

    [Fact] // US-15 / RC-04: etiquetar una foto postea al endpoint de etiquetas de foto
    public async Task Etiquetar_foto_postea_al_endpoint()
    {
        var (cliente, handler) = Crear();
        var fotoId = Guid.NewGuid();

        var r = await cliente.EtiquetarFotoAsync(fotoId, "fisura");

        r.Exito.Should().BeTrue();
        handler.UltimaUri!.AbsolutePath.Should().Be($"/api/v1/fotos/{fotoId}/etiquetas");
        handler.UltimoCuerpo.Should().Contain("fisura");
    }

    [Fact] // etiquetar un comentario postea a su endpoint
    public async Task Etiquetar_comentario_postea_al_endpoint()
    {
        var (cliente, handler) = Crear();
        var comentarioId = Guid.NewGuid();

        await cliente.EtiquetarComentarioAsync(comentarioId, "urgente");

        handler.UltimaUri!.AbsolutePath.Should().Be($"/api/v1/comentarios/{comentarioId}/etiquetas");
    }

    [Fact] // validación local: una etiqueta vacía no llama al backend
    public async Task Etiqueta_vacia_no_llama_al_backend()
    {
        var (cliente, handler) = Crear();

        var r = await cliente.EtiquetarFotoAsync(Guid.NewGuid(), "  ");

        r.Exito.Should().BeFalse();
        handler.Llamadas.Should().Be(0);
    }

    [Fact] // RN-05: el rechazo del backend (relevamiento cerrado → 422/409) se refleja como error
    public async Task Rechazo_del_backend_se_refleja_como_error()
    {
        var (cliente, _) = Crear(HttpStatusCode.UnprocessableEntity);

        var r = await cliente.AgregarComentarioAsync(Guid.NewGuid(), null, "tarde");

        r.Exito.Should().BeFalse();
        r.Mensaje.Should().Contain("422");
    }
}
