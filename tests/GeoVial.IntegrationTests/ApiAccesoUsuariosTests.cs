using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using GeoVial.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GeoVial.IntegrationTests;

/// <summary>
/// Pruebas de integración de la API (WebApplicationFactory) sobre el slice del Sprint 01.
/// Usan el proveedor en memoria (sin SQL Server) y el seed del usuario raíz (BT-10).
/// Verifican el camino end-to-end de CU-02 (login), CU-03 (alta jerárquica) y CU-14 (autorización).
/// </summary>
public class ApiAccesoUsuariosTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiAccesoUsuariosTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(b => b.UseEnvironment("Development"));
    }

    private async Task<string> LoginRaizAsync(HttpClient cliente)
    {
        var resp = await cliente.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("raiz", "GeoVial.Raiz.2026"));
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var token = await resp.Content.ReadFromJsonAsync<TokenResponse>();
        return token!.AccessToken;
    }

    [Fact] // CU-02: login del usuario raíz sembrado devuelve un JWT
    public async Task Login_raiz_devuelve_token()
    {
        var cliente = _factory.CreateClient();
        var token = await LoginRaizAsync(cliente);
        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact] // CU-02: credenciales inválidas → 401
    public async Task Login_invalido_devuelve_401()
    {
        var cliente = _factory.CreateClient();
        var resp = await cliente.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("raiz", "mala"));
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact] // CU-03 CA-01: el raíz da de alta al jefe general (nivel inmediato inferior) → 201
    public async Task Alta_jefe_general_por_raiz_devuelve_201()
    {
        var cliente = _factory.CreateClient();
        var token = await LoginRaizAsync(cliente);
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await cliente.PostAsJsonAsync("/api/v1/usuarios", new AltaUsuarioRequest("Jefe General", 2, null));

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var creado = await resp.Content.ReadFromJsonAsync<UsuarioDto>();
        creado!.Rol.Should().Be(2);
        creado.Vigente.Should().BeTrue();
    }

    [Fact] // CU-14 / RN-01: el raíz no puede crear un agente (no es el nivel inmediato inferior) → 403
    public async Task Alta_agente_por_raiz_devuelve_403()
    {
        var cliente = _factory.CreateClient();
        var token = await LoginRaizAsync(cliente);
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await cliente.PostAsJsonAsync(
            "/api/v1/usuarios", new AltaUsuarioRequest("Agente", 4, Guid.NewGuid()));

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact] // CU-14: acceso sin token → 401
    public async Task Listar_sin_token_devuelve_401()
    {
        var cliente = _factory.CreateClient();
        var resp = await cliente.GetAsync("/api/v1/usuarios");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact] // CU-01 / RN-01: el raíz no es jefe de área, no puede crear relevamientos → 403
    public async Task Crear_relevamiento_por_raiz_devuelve_403()
    {
        var cliente = _factory.CreateClient();
        var token = await LoginRaizAsync(cliente);
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await cliente.PostAsJsonAsync(
            "/api/v1/relevamientos", new CrearRelevamientoRequest("Puente Río 12", 15m));

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact] // CU-01: el listado de relevamientos responde 200 con el token del raíz
    public async Task Listar_relevamientos_con_token_devuelve_200()
    {
        var cliente = _factory.CreateClient();
        var token = await LoginRaizAsync(cliente);
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await cliente.GetAsync("/api/v1/relevamientos");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact] // CU-01: relevamientos sin token → 401
    public async Task Listar_relevamientos_sin_token_devuelve_401()
    {
        var cliente = _factory.CreateClient();
        var resp = await cliente.GetAsync("/api/v1/relevamientos");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact] // CU-04: captura sin token → 401
    public async Task Capturar_observacion_sin_token_devuelve_401()
    {
        var cliente = _factory.CreateClient();
        var resp = await cliente.PostAsJsonAsync(
            $"/api/v1/relevamientos/{Guid.NewGuid()}/observaciones",
            new CapturarObservacionRequest("foto.jpg", -34.6m, -58.4m));
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact] // CU-05: ubicación manual sin token → 401
    public async Task Ubicar_manual_sin_token_devuelve_401()
    {
        var cliente = _factory.CreateClient();
        var resp = await cliente.PostAsJsonAsync(
            $"/api/v1/observaciones/{Guid.NewGuid()}/ubicacion",
            new UbicarManualRequest(-34.6m, -58.4m));
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact] // CU-11: detección de conflictos sin token → 401
    public async Task Detectar_conflictos_sin_token_devuelve_401()
    {
        var cliente = _factory.CreateClient();
        var resp = await cliente.PostAsync($"/api/v1/relevamientos/{Guid.NewGuid()}/conflictos/deteccion", null);
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact] // CU-12: resolución de conflicto sin token → 401
    public async Task Resolver_conflicto_sin_token_devuelve_401()
    {
        var cliente = _factory.CreateClient();
        var resp = await cliente.PostAsJsonAsync(
            $"/api/v1/conflictos/{Guid.NewGuid()}/resolucion", new ResolverConflictoRequest(2, null));
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact] // CU-11 / RN-01: el raíz no es jefe de área, no detecta conflictos de un relevamiento inexistente → 403/404
    public async Task Detectar_conflictos_por_raiz_no_es_exito()
    {
        var cliente = _factory.CreateClient();
        var token = await LoginRaizAsync(cliente);
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await cliente.PostAsync($"/api/v1/relevamientos/{Guid.NewGuid()}/conflictos/deteccion", null);

        resp.IsSuccessStatusCode.Should().BeFalse();
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact] // CU-12: resolver un conflicto inexistente con token del raíz → 404
    public async Task Resolver_conflicto_inexistente_devuelve_404()
    {
        var cliente = _factory.CreateClient();
        var token = await LoginRaizAsync(cliente);
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await cliente.PostAsJsonAsync(
            $"/api/v1/conflictos/{Guid.NewGuid()}/resolucion", new ResolverConflictoRequest(2, null));

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
