using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoVial.Shared;
using Xunit;

namespace GeoVial.IntegrationTests;

/// <summary>
/// E2E del resumen de actividad del relevamiento (S72, reporting): GET /api/v1/relevamientos/{id}/resumen
/// agrega marcadores, observaciones y productividad por agente, autorizado por área.
/// </summary>
public class ResumenRelevamientoE2ETests : IClassFixture<FabricaPruebas>
{
    private readonly FabricaPruebas _factory;

    public ResumenRelevamientoE2ETests(FabricaPruebas factory) => _factory = factory;

    [Fact] // capturar dos observaciones georreferenciadas y pedir el resumen: 2 marcadores, 2 observaciones, 1 agente
    public async Task Resumen_agrega_marcadores_observaciones_y_productividad()
    {
        var esc = await EscenarioE2E.SembrarAsync(_factory);
        var cliente = await EscenarioE2E.ClienteAutenticadoAsync(_factory, esc.Usuario, esc.Clave);
        await CapturarAsync(cliente, esc.RelevamientoId, -34.60m, -58.40m);
        await CapturarAsync(cliente, esc.RelevamientoId, -34.70m, -58.50m); // lejos → otro marcador

        var resp = await cliente.GetAsync($"/api/v1/relevamientos/{esc.RelevamientoId}/resumen");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var resumen = await resp.Content.ReadFromJsonAsync<ResumenRelevamientoDto>();
        resumen!.RelevamientoId.Should().Be(esc.RelevamientoId);
        resumen.Observaciones.Should().Be(2);
        resumen.Marcadores.Should().Be(2);
        resumen.ObservacionesSinGeorreferenciar.Should().Be(0);
        resumen.Productividad.Should().ContainSingle()
            .Which.Observaciones.Should().Be(2);
    }

    [Fact] // sin token → 401
    public async Task Resumen_sin_token_devuelve_401()
    {
        var cliente = _factory.CreateClient();

        var resp = await cliente.GetAsync($"/api/v1/relevamientos/{Guid.NewGuid()}/resumen");

        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact] // relevamiento de otra área (inexistente para el solicitante) → 404, sin filtrar información
    public async Task Resumen_de_relevamiento_inaccesible_devuelve_404()
    {
        var esc = await EscenarioE2E.SembrarAsync(_factory);
        var cliente = await EscenarioE2E.ClienteAutenticadoAsync(_factory, esc.Usuario, esc.Clave);

        var resp = await cliente.GetAsync($"/api/v1/relevamientos/{Guid.NewGuid()}/resumen");

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task CapturarAsync(HttpClient cliente, Guid relevamientoId, decimal lat, decimal lon)
    {
        var resp = await cliente.PostAsJsonAsync(
            $"/api/v1/relevamientos/{relevamientoId}/observaciones",
            new CapturarObservacionRequest("foto.jpg", lat, lon));
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
