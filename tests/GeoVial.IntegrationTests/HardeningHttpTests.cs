using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoVial.Application.Configuracion;
using GeoVial.Shared;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace GeoVial.IntegrationTests;

/// <summary>
/// Hardening HTTP (S70): manejador global de errores (ProblemDetails 500 sin filtrar detalles) y cabeceras de
/// seguridad en toda respuesta.
/// </summary>
public class HardeningHttpTests : IClassFixture<FabricaPruebas>
{
    private readonly FabricaPruebas _factory;

    public HardeningHttpTests(FabricaPruebas factory) => _factory = factory;

    [Fact] // una excepción no controlada → 500 ProblemDetails genérico, sin filtrar el mensaje/stack de la excepción
    public async Task Excepcion_no_controlada_devuelve_problemdetails_generico()
    {
        var cliente = _factory.CreateClient();

        var resp = await cliente.GetAsync("/_diagnostico/excepcion");

        resp.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        resp.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        var cuerpo = await resp.Content.ReadAsStringAsync();
        cuerpo.Should().Contain("Error interno del servidor");
        cuerpo.Should().NotContain("excepción de prueba");            // no se filtra el mensaje de la excepción
        cuerpo.Should().NotContain("InvalidOperationException");       // ni el tipo / stack
    }

    [Theory] // toda respuesta trae las cabeceras de seguridad
    [InlineData("X-Content-Type-Options")]
    [InlineData("X-Frame-Options")]
    [InlineData("Referrer-Policy")]
    [InlineData("Cross-Origin-Resource-Policy")]
    public async Task Las_respuestas_traen_cabeceras_de_seguridad(string cabecera)
    {
        var cliente = _factory.CreateClient();

        var resp = await cliente.GetAsync("/health");

        resp.Headers.GetValues(cabecera).Single().Should().Be(CabecerasSeguridad.Predeterminadas[cabecera]);
    }
}

/// <summary>Fábrica con un límite de rate limiting muy bajo, para verificar el 429 de forma determinista.</summary>
public sealed class FabricaPruebasLimiteRapido : FabricaPruebas
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("RateLimit:Auth:PermitLimit", "3");
        builder.UseSetting("RateLimit:Auth:VentanaSegundos", "60");
        base.ConfigureWebHost(builder);
    }
}

/// <summary>Rate limiting (S70): los endpoints de autenticación rechazan con 429 al superar el límite por ventana.</summary>
public class RateLimitTests : IClassFixture<FabricaPruebasLimiteRapido>
{
    private readonly FabricaPruebasLimiteRapido _factory;

    public RateLimitTests(FabricaPruebasLimiteRapido factory) => _factory = factory;

    [Fact] // con límite 3, el 4.º intento de login (aunque las credenciales sean inválidas) devuelve 429
    public async Task Login_supera_el_limite_y_devuelve_429()
    {
        var cliente = _factory.CreateClient();
        var codigos = new List<HttpStatusCode>();

        for (var i = 0; i < 6; i++)
        {
            var resp = await cliente.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("quien-sea", "mala"));
            codigos.Add(resp.StatusCode);
        }

        codigos.Should().Contain(HttpStatusCode.TooManyRequests); // el rate limiter cortó antes de validar las credenciales
    }
}
