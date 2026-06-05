using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoVial.Domain;
using GeoVial.Infrastructure.Persistencia;
using GeoVial.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GeoVial.IntegrationTests;

/// <summary>
/// E2E del endpoint "asignados a mí" (S47, F-M-04/05): GET /api/v1/relevamientos/mios devuelve sólo los
/// relevamientos con asignación vigente del agente, no los del área a los que no está asignado. Cierra la
/// fuga por la que el dispositivo del agente recibía relevamientos ajenos (el listado de área los traía todos).
/// </summary>
public class RelevamientosMiosE2ETests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public RelevamientosMiosE2ETests(WebApplicationFactory<Program> factory) =>
        _factory = factory.WithWebHostBuilder(b => b.UseEnvironment("Development"));

    [Fact] // E2E / S47: /mios trae el asignado y NO un relevamiento no asignado de la misma área
    public async Task Mios_devuelve_solo_los_asignados_al_agente()
    {
        var esc = await EscenarioE2E.SembrarAsync(_factory);

        // Otro relevamiento en la MISMA área pero sin asignar al agente: no debe aparecer en /mios.
        Guid ajenoId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GeoVialDbContext>();
            var ajeno = Relevamiento.Crear("Obra ajena (misma área)", 15m, esc.AreaId).Valor!;
            await db.Relevamientos.AddAsync(ajeno);
            await db.SaveChangesAsync();
            ajenoId = ajeno.RelevamientoId;
        }

        var cliente = await EscenarioE2E.ClienteAutenticadoAsync(_factory, esc.Usuario, esc.Clave);

        var mios = (await cliente.GetFromJsonAsync<List<RelevamientoDto>>("/api/v1/relevamientos/mios"))!;

        mios.Select(r => r.RelevamientoId).Should().Contain(esc.RelevamientoId).And.NotContain(ajenoId);
    }

    [Fact] // E2E / RN-01: /mios sin token responde 401
    public async Task Mios_sin_token_devuelve_401()
    {
        var cliente = _factory.CreateClient();

        var resp = await cliente.GetAsync("/api/v1/relevamientos/mios");

        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
