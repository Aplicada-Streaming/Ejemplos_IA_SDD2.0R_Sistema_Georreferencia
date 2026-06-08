using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoVial.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GeoVial.IntegrationTests;

/// <summary>
/// Pruebas E2E del ciclo de conflictos por radio (Sprint 23, CU-11/CU-12). Sobre la API real: capturar dos
/// marcadores, ampliar el radio para que queden en conflicto, detectar y resolver (unificar / mantener
/// separados). Cierra la deuda de E2E de conflictos de las retros S18/S20.
/// </summary>
public class ConflictosE2ETests : IClassFixture<FabricaPruebas>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ConflictosE2ETests(FabricaPruebas factory) =>
        _factory = factory.WithWebHostBuilder(b => b.UseEnvironment("Development"));

    private static async Task<Guid> CapturarMarcadorAsync(HttpClient cliente, Guid relevamientoId, decimal lat, decimal lon)
    {
        var resp = await cliente.PostAsJsonAsync(
            $"/api/v1/relevamientos/{relevamientoId}/observaciones",
            new CapturarObservacionRequest("foto.jpg", lat, lon));
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var capt = (await resp.Content.ReadFromJsonAsync<CapturaResponse>())!;
        capt.MarcadorId.Should().NotBeNull();
        return capt.MarcadorId!.Value;
    }

    // Captura dos marcadores a ~33 m (radio 15 ⇒ distintos), amplía el radio a 50 y detecta el conflicto.
    private static async Task<ConflictoPendienteDto> PrepararConflictoAsync(HttpClient cliente, Guid relevamientoId)
    {
        var mA = await CapturarMarcadorAsync(cliente, relevamientoId, -34.600000m, -58.400000m);
        var mB = await CapturarMarcadorAsync(cliente, relevamientoId, -34.600300m, -58.400000m);
        mA.Should().NotBe(mB); // dos marcadores distintos

        var radio = await cliente.PutAsJsonAsync($"/api/v1/relevamientos/{relevamientoId}/radio", new AjustarRadioRequest(50m));
        radio.IsSuccessStatusCode.Should().BeTrue();

        var det = await cliente.PostAsync($"/api/v1/relevamientos/{relevamientoId}/conflictos/deteccion", null);
        det.StatusCode.Should().Be(HttpStatusCode.OK);
        (await det.Content.ReadFromJsonAsync<List<ConflictoDetectadoDto>>())!.Should().ContainSingle();

        var pendientes = (await (await cliente.GetAsync($"/api/v1/relevamientos/{relevamientoId}/conflictos"))
            .Content.ReadFromJsonAsync<List<ConflictoPendienteDto>>())!;
        var conflicto = pendientes.Should().ContainSingle().Subject;
        conflicto.Tipo.Should().Be(1); // marcadores en un mismo radio
        conflicto.MarcadorA.Should().NotBeNull();
        conflicto.MarcadorB.Should().NotBeNull();
        return conflicto;
    }

    [Fact] // E2E CU-11/CU-12: detectar dos marcadores en radio y unificarlos
    public async Task Detectar_y_unificar_conflicto_por_radio()
    {
        var esc = await EscenarioE2E.SembrarAsync(_factory, radioMetros: 15m);
        var cliente = await EscenarioE2E.ClienteAutenticadoAsync(_factory, esc.Usuario, esc.Clave);

        var conflicto = await PrepararConflictoAsync(cliente, esc.RelevamientoId);

        // Unificar en el marcador A
        var resol = await cliente.PostAsJsonAsync(
            $"/api/v1/conflictos/{conflicto.ConflictoSyncId}/resolucion",
            new ResolverConflictoRequest(1, conflicto.MarcadorA));
        resol.IsSuccessStatusCode.Should().BeTrue();

        // Ya no hay pendientes y el relevamiento quedó con un único marcador
        var pendientes = (await (await cliente.GetAsync($"/api/v1/relevamientos/{esc.RelevamientoId}/conflictos"))
            .Content.ReadFromJsonAsync<List<ConflictoPendienteDto>>())!;
        pendientes.Should().BeEmpty();

        var rev = (await (await cliente.GetAsync($"/api/v1/relevamientos/{esc.RelevamientoId}/revision"))
            .Content.ReadFromJsonAsync<RevisionRelevamientoDto>())!;
        rev.Marcadores.Should().ContainSingle().Which.MarcadorId.Should().Be(conflicto.MarcadorA!.Value);
    }

    [Fact] // E2E CU-12: detectar y mantener separados conserva ambos marcadores
    public async Task Detectar_y_mantener_separados()
    {
        var esc = await EscenarioE2E.SembrarAsync(_factory, radioMetros: 15m);
        var cliente = await EscenarioE2E.ClienteAutenticadoAsync(_factory, esc.Usuario, esc.Clave);

        var conflicto = await PrepararConflictoAsync(cliente, esc.RelevamientoId);

        // Mantener separados
        var resol = await cliente.PostAsJsonAsync(
            $"/api/v1/conflictos/{conflicto.ConflictoSyncId}/resolucion",
            new ResolverConflictoRequest(2, null));
        resol.IsSuccessStatusCode.Should().BeTrue();

        // No quedan pendientes y ambos marcadores se conservan
        var pendientes = (await (await cliente.GetAsync($"/api/v1/relevamientos/{esc.RelevamientoId}/conflictos"))
            .Content.ReadFromJsonAsync<List<ConflictoPendienteDto>>())!;
        pendientes.Should().BeEmpty();

        var rev = (await (await cliente.GetAsync($"/api/v1/relevamientos/{esc.RelevamientoId}/revision"))
            .Content.ReadFromJsonAsync<RevisionRelevamientoDto>())!;
        rev.Marcadores.Select(m => m.MarcadorId)
            .Should().Contain(new[] { conflicto.MarcadorA!.Value, conflicto.MarcadorB!.Value });
    }
}
