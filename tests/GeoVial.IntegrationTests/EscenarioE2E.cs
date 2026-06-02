using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using GeoVial.Application.Abstracciones;
using GeoVial.Domain;
using GeoVial.Infrastructure.Persistencia;
using GeoVial.Shared;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GeoVial.IntegrationTests;

/// <summary>
/// Helper de escenario completo para pruebas E2E (Sprint 23). Siembra su propia área, un agente de campo con
/// credencial y un relevamiento asignado, listo para capturar, y autentica por HTTP. Sembrar su propia área
/// desacopla las pruebas del seed de Development (acción de la retro del Sprint 20).
/// </summary>
internal static class EscenarioE2E
{
    public const string Clave = "Clave.E2E.2026";

    public sealed record Datos(Guid RelevamientoId, Guid AreaId, Guid AgenteId, string Usuario, string Clave);

    public static async Task<Datos> SembrarAsync(WebApplicationFactory<Program> factory, decimal radioMetros = 15m)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GeoVialDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IHasherClave>();

        var area = Area.Crear($"Área E2E {Guid.NewGuid():N}");
        await db.Areas.AddAsync(area);

        var agente = Usuario.Crear("Agente E2E", RolJerarquico.AgenteCampo, area.AreaId).Valor!;
        await db.Usuarios.AddAsync(agente);
        var usuario = $"agente.e2e.{Guid.NewGuid():N}";
        await db.Credenciales.AddAsync(new Credencial(agente.UsuarioId, usuario, hasher.Hash(Clave)));

        var rel = Relevamiento.Crear("Obra E2E", radioMetros, area.AreaId).Valor!;
        rel.AsignarAgente(agente);
        await db.Relevamientos.AddAsync(rel);
        await db.SaveChangesAsync();

        return new Datos(rel.RelevamientoId, area.AreaId, agente.UsuarioId, usuario, Clave);
    }

    public static async Task<HttpClient> ClienteAutenticadoAsync(WebApplicationFactory<Program> factory, string usuario, string clave)
    {
        var cliente = factory.CreateClient();
        var login = await cliente.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(usuario, clave));
        login.StatusCode.Should().Be(HttpStatusCode.OK);
        var token = await login.Content.ReadFromJsonAsync<TokenResponse>();
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.AccessToken);
        return cliente;
    }
}
