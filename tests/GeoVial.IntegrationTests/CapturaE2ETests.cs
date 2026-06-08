using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoVial.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GeoVial.IntegrationTests;

/// <summary>
/// Pruebas E2E del camino completo por HTTP (Sprint 20, endurecimiento del MVP). Usan el helper compartido
/// <see cref="EscenarioE2E"/> (Sprint 23) para sembrar un escenario autocontenido —área propia → agente con
/// credencial → relevamiento asignado— y autenticar, y verifican los flujos centrales sobre la API real:
/// captura georreferenciada, ubicación manual y autorización por área (RN-01).
/// </summary>
public class CapturaE2ETests : IClassFixture<FabricaPruebas>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CapturaE2ETests(FabricaPruebas factory) =>
        _factory = factory.WithWebHostBuilder(b => b.UseEnvironment("Development"));

    [Fact] // E2E: captura georreferenciada → FotoId → subir binario → descargar → revisión → comentar
    public async Task Happy_path_de_captura_completo()
    {
        var esc = await EscenarioE2E.SembrarAsync(_factory);
        var cliente = await EscenarioE2E.ClienteAutenticadoAsync(_factory, esc.Usuario, esc.Clave);

        // Capturar con coordenadas EXIF
        var capResp = await cliente.PostAsJsonAsync(
            $"/api/v1/relevamientos/{esc.RelevamientoId}/observaciones",
            new CapturarObservacionRequest("obra/foto-e2e.jpg", -34.6m, -58.4m));
        capResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var capt = (await capResp.Content.ReadFromJsonAsync<CapturaResponse>())!;
        capt.SinGeorreferenciar.Should().BeFalse();
        capt.MarcadorId.Should().NotBeNull();
        capt.FotoId.Should().NotBe(Guid.Empty);

        // Subir el binario de la foto (el pipeline aloja tal cual los binarios que no son imágenes)
        var bytes = new byte[] { 9, 8, 7, 6, 5, 4, 3 };
        using var contenido = new MultipartFormDataContent { { new ByteArrayContent(bytes), "archivo", "foto-e2e.jpg" } };
        var subir = await cliente.PostAsync($"/api/v1/fotos/{capt.FotoId}/contenido", contenido);
        subir.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Descargarlo y verificar que coincide
        var descarga = await cliente.GetAsync($"/api/v1/fotos/{capt.FotoId}/contenido");
        descarga.StatusCode.Should().Be(HttpStatusCode.OK);
        (await descarga.Content.ReadAsByteArrayAsync()).Should().Equal(bytes);

        // La revisión muestra el marcador con su foto
        var rev = (await (await cliente.GetAsync($"/api/v1/relevamientos/{esc.RelevamientoId}/revision"))
            .Content.ReadFromJsonAsync<RevisionRelevamientoDto>())!;
        var marcador = rev.Marcadores.Should().ContainSingle(m => m.MarcadorId == capt.MarcadorId).Subject;
        marcador.Fotos.Should().ContainSingle();

        // Comentar el marcador
        var com = await cliente.PostAsJsonAsync(
            $"/api/v1/marcadores/{capt.MarcadorId}/comentarios", new AgregarComentarioRequest(null, "Comentario E2E"));
        com.IsSuccessStatusCode.Should().BeTrue();

        // La revisión refleja el comentario
        var rev2 = (await (await cliente.GetAsync($"/api/v1/relevamientos/{esc.RelevamientoId}/revision"))
            .Content.ReadFromJsonAsync<RevisionRelevamientoDto>())!;
        rev2.Marcadores.First(m => m.MarcadorId == capt.MarcadorId).Comentarios
            .Should().Contain(c => c.Texto == "Comentario E2E");
    }

    [Fact] // E2E: captura sin GPS → bandeja sin georreferenciar → ubicar manual → la revisión muestra el marcador
    public async Task Ubicacion_manual_completo()
    {
        var esc = await EscenarioE2E.SembrarAsync(_factory);
        var cliente = await EscenarioE2E.ClienteAutenticadoAsync(_factory, esc.Usuario, esc.Clave);

        var capResp = await cliente.PostAsJsonAsync(
            $"/api/v1/relevamientos/{esc.RelevamientoId}/observaciones",
            new CapturarObservacionRequest("obra/sin-gps.jpg", null, null));
        var capt = (await capResp.Content.ReadFromJsonAsync<CapturaResponse>())!;
        capt.SinGeorreferenciar.Should().BeTrue();
        capt.MarcadorId.Should().BeNull();

        // S50: la revisión expone la bandeja enriquecida (id + momento + referencia de foto) antes de ubicar
        var antes = (await (await cliente.GetAsync($"/api/v1/relevamientos/{esc.RelevamientoId}/revision"))
            .Content.ReadFromJsonAsync<RevisionRelevamientoDto>())!;
        var enBandeja = antes.Bandeja.Should().ContainSingle().Subject;
        enBandeja.ObservacionId.Should().Be(capt.ObservacionId);
        enBandeja.ReferenciaArchivo.Should().Be("obra/sin-gps.jpg");

        // Ubicar manualmente el punto
        var ubicar = await cliente.PostAsJsonAsync(
            $"/api/v1/observaciones/{capt.ObservacionId}/ubicacion", new UbicarManualRequest(-34.61m, -58.41m));
        ubicar.IsSuccessStatusCode.Should().BeTrue();

        // La revisión ahora tiene un marcador y la bandeja quedó vacía
        var rev = (await (await cliente.GetAsync($"/api/v1/relevamientos/{esc.RelevamientoId}/revision"))
            .Content.ReadFromJsonAsync<RevisionRelevamientoDto>())!;
        rev.Marcadores.Should().ContainSingle();
        rev.ObservacionesSinGeorreferenciar.Should().BeEmpty();
        rev.Bandeja.Should().BeEmpty();
    }

    [Fact] // E2E / S46: reenviar la misma CapturaId no duplica — devuelve la observación original y la revisión muestra un solo marcador
    public async Task Reenvio_de_captura_es_idempotente()
    {
        var esc = await EscenarioE2E.SembrarAsync(_factory);
        var cliente = await EscenarioE2E.ClienteAutenticadoAsync(_factory, esc.Usuario, esc.Clave);

        var capturaId = Guid.NewGuid();
        var peticion = new CapturarObservacionRequest("obra/idem.jpg", -34.6m, -58.4m, capturaId);

        var primera = (await (await cliente.PostAsJsonAsync(
            $"/api/v1/relevamientos/{esc.RelevamientoId}/observaciones", peticion)).Content.ReadFromJsonAsync<CapturaResponse>())!;

        // Reenvío de la MISMA captura (simula el reintento del auto-sync tras un corte posterior al alta).
        var reenvio = (await (await cliente.PostAsJsonAsync(
            $"/api/v1/relevamientos/{esc.RelevamientoId}/observaciones", peticion)).Content.ReadFromJsonAsync<CapturaResponse>())!;

        reenvio.ObservacionId.Should().Be(primera.ObservacionId);
        reenvio.FotoId.Should().Be(primera.FotoId);

        // La revisión no muestra duplicados: un único marcador con una sola foto.
        var rev = (await (await cliente.GetAsync($"/api/v1/relevamientos/{esc.RelevamientoId}/revision"))
            .Content.ReadFromJsonAsync<RevisionRelevamientoDto>())!;
        rev.Marcadores.Should().ContainSingle().Which.Fotos.Should().ContainSingle();
    }

    [Fact] // E2E / US-15 / CU-09 §5.A: capturar → quitar la foto → la revisión ya no la muestra y el binario no se descarga
    public async Task Quitar_foto_del_marcador_completo()
    {
        var esc = await EscenarioE2E.SembrarAsync(_factory);
        var cliente = await EscenarioE2E.ClienteAutenticadoAsync(_factory, esc.Usuario, esc.Clave);

        // Capturar (georreferenciada) y subir el binario
        var capt = (await (await cliente.PostAsJsonAsync(
            $"/api/v1/relevamientos/{esc.RelevamientoId}/observaciones",
            new CapturarObservacionRequest("obra/quitar.jpg", -34.6m, -58.4m))).Content.ReadFromJsonAsync<CapturaResponse>())!;
        using var contenido = new MultipartFormDataContent { { new ByteArrayContent(new byte[] { 1, 2, 3 }), "archivo", "quitar.jpg" } };
        (await cliente.PostAsync($"/api/v1/fotos/{capt.FotoId}/contenido", contenido)).StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Quitar la foto del marcador
        var quitar = await cliente.DeleteAsync($"/api/v1/fotos/{capt.FotoId}");
        quitar.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // El binario ya no se descarga y la revisión no muestra ese marcador con foto
        (await cliente.GetAsync($"/api/v1/fotos/{capt.FotoId}/contenido")).StatusCode.Should().Be(HttpStatusCode.NotFound);
        var rev = (await (await cliente.GetAsync($"/api/v1/relevamientos/{esc.RelevamientoId}/revision"))
            .Content.ReadFromJsonAsync<RevisionRelevamientoDto>())!;
        rev.Marcadores.Where(m => m.MarcadorId == capt.MarcadorId).SelectMany(m => m.Fotos).Should().BeEmpty();
    }

    [Fact] // E2E / RN-01: un agente de otra área no puede capturar en el relevamiento
    public async Task Agente_de_otra_area_no_captura()
    {
        var esc = await EscenarioE2E.SembrarAsync(_factory);
        // Un segundo escenario siembra otra área con su propio agente: es ajeno al relevamiento de 'esc'.
        var ajeno = await EscenarioE2E.SembrarAsync(_factory);
        var cliente = await EscenarioE2E.ClienteAutenticadoAsync(_factory, ajeno.Usuario, ajeno.Clave);

        var capResp = await cliente.PostAsJsonAsync(
            $"/api/v1/relevamientos/{esc.RelevamientoId}/observaciones",
            new CapturarObservacionRequest("obra/ajeno.jpg", -34.6m, -58.4m));

        capResp.IsSuccessStatusCode.Should().BeFalse();
    }
}
