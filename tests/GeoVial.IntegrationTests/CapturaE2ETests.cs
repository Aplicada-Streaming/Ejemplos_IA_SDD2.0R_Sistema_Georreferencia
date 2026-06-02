using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using GeoVial.Application.Abstracciones;
using GeoVial.Domain;
using GeoVial.Infrastructure.Persistencia;
using GeoVial.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GeoVial.IntegrationTests;

/// <summary>
/// Pruebas E2E del camino completo por HTTP (Sprint 20, endurecimiento del MVP). Siembran un escenario
/// completo (área → agente con credencial → relevamiento asignado) en el proveedor en memoria y verifican
/// los flujos centrales sobre la API real: captura georreferenciada, ubicación manual y autorización.
/// </summary>
public class CapturaE2ETests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CapturaE2ETests(WebApplicationFactory<Program> factory) =>
        _factory = factory.WithWebHostBuilder(b => b.UseEnvironment("Development"));

    private sealed record Escenario(Guid RelevamientoId, Guid AreaId, string Usuario, string Clave);

    private const string Clave = "Clave.E2E.2026";

    // Siembra un agente de campo con credencial en el área dada y devuelve el usuario y su nombre de login.
    private static async Task<(Usuario Agente, string NombreUsuario)> SembrarAgenteAsync(GeoVialDbContext db, IHasherClave hasher, Guid areaId)
    {
        var agente = Usuario.Crear("Agente E2E", RolJerarquico.AgenteCampo, areaId).Valor!;
        await db.Usuarios.AddAsync(agente);
        var usuario = $"agente.e2e.{Guid.NewGuid():N}";
        await db.Credenciales.AddAsync(new Credencial(agente.UsuarioId, usuario, hasher.Hash(Clave)));
        return (agente, usuario);
    }

    // Escenario completo: área existente + agente asignado a un relevamiento listo para capturar.
    private async Task<Escenario> SembrarEscenarioAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GeoVialDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IHasherClave>();

        var area = await db.Areas.FirstAsync();
        var (agente, usuario) = await SembrarAgenteAsync(db, hasher, area.AreaId);

        var rel = Relevamiento.Crear("Obra E2E", 15m, area.AreaId).Valor!;
        rel.AsignarAgente(agente);
        await db.Relevamientos.AddAsync(rel);
        await db.SaveChangesAsync();

        return new Escenario(rel.RelevamientoId, area.AreaId, usuario, Clave);
    }

    // Un agente en un área nueva (distinta de la del escenario), para la prueba de autorización.
    private async Task<string> SembrarAgenteEnNuevaAreaAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GeoVialDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IHasherClave>();

        var area = Area.Crear($"Área E2E {Guid.NewGuid():N}");
        await db.Areas.AddAsync(area);
        var (_, usuario) = await SembrarAgenteAsync(db, hasher, area.AreaId);
        await db.SaveChangesAsync();
        return usuario;
    }

    private async Task<HttpClient> ClienteAutenticadoAsync(string usuario, string clave)
    {
        var cliente = _factory.CreateClient();
        var login = await cliente.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(usuario, clave));
        login.StatusCode.Should().Be(HttpStatusCode.OK);
        var token = await login.Content.ReadFromJsonAsync<TokenResponse>();
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.AccessToken);
        return cliente;
    }

    [Fact] // E2E: captura georreferenciada → FotoId → subir binario → descargar → revisión → comentar
    public async Task Happy_path_de_captura_completo()
    {
        var esc = await SembrarEscenarioAsync();
        var cliente = await ClienteAutenticadoAsync(esc.Usuario, esc.Clave);

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
        var esc = await SembrarEscenarioAsync();
        var cliente = await ClienteAutenticadoAsync(esc.Usuario, esc.Clave);

        var capResp = await cliente.PostAsJsonAsync(
            $"/api/v1/relevamientos/{esc.RelevamientoId}/observaciones",
            new CapturarObservacionRequest("obra/sin-gps.jpg", null, null));
        var capt = (await capResp.Content.ReadFromJsonAsync<CapturaResponse>())!;
        capt.SinGeorreferenciar.Should().BeTrue();
        capt.MarcadorId.Should().BeNull();

        // Ubicar manualmente el punto
        var ubicar = await cliente.PostAsJsonAsync(
            $"/api/v1/observaciones/{capt.ObservacionId}/ubicacion", new UbicarManualRequest(-34.61m, -58.41m));
        ubicar.IsSuccessStatusCode.Should().BeTrue();

        // La revisión ahora tiene un marcador y la bandeja quedó vacía
        var rev = (await (await cliente.GetAsync($"/api/v1/relevamientos/{esc.RelevamientoId}/revision"))
            .Content.ReadFromJsonAsync<RevisionRelevamientoDto>())!;
        rev.Marcadores.Should().ContainSingle();
        rev.ObservacionesSinGeorreferenciar.Should().BeEmpty();
    }

    [Fact] // E2E / RN-01: un agente de otra área no puede capturar en el relevamiento
    public async Task Agente_de_otra_area_no_captura()
    {
        var esc = await SembrarEscenarioAsync();
        var ajeno = await SembrarAgenteEnNuevaAreaAsync();
        var cliente = await ClienteAutenticadoAsync(ajeno, Clave);

        var capResp = await cliente.PostAsJsonAsync(
            $"/api/v1/relevamientos/{esc.RelevamientoId}/observaciones",
            new CapturarObservacionRequest("obra/ajeno.jpg", -34.6m, -58.4m));

        capResp.IsSuccessStatusCode.Should().BeFalse();
    }
}
