using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GeoVial.IntegrationTests;

/// <summary>
/// Hardening de producción (S68): health checks (liveness/readiness) y los guardrails de arranque que impiden
/// levantar el backend con configuración insegura en producción (base en memoria o clave JWT de desarrollo).
/// </summary>
public class SaludEArranqueTests : IClassFixture<FabricaPruebas>
{
    private readonly FabricaPruebas _factory;

    public SaludEArranqueTests(FabricaPruebas factory) => _factory = factory;

    [Fact] // liveness: el proceso responde (sin chequear dependencias), anónimo
    public async Task Health_liveness_devuelve_200()
    {
        var cliente = _factory.CreateClient();

        var resp = await cliente.GetAsync("/health");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact] // readiness: la base es alcanzable (InMemory en pruebas → CanConnect true), anónimo
    public async Task Health_readiness_devuelve_200_con_base_alcanzable()
    {
        var cliente = _factory.CreateClient();

        var resp = await cliente.GetAsync("/health/ready");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact] // guardrail: en producción sin cadena de conexión ni clave JWT propia, el arranque falla rápido
    public void Arranque_en_produccion_sin_configuracion_segura_falla()
    {
        using var factoryProd = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b.UseEnvironment("Production"));

        // El servidor se construye perezosamente; al pedir el cliente se dispara el arranque, que debe fallar
        // por los guardrails (no hay ConnectionStrings:GeoVial ni Jwt:ClaveSecreta en producción).
        var arrancar = () => factoryProd.CreateClient();

        arrancar.Should().Throw<Exception>();
    }
}
