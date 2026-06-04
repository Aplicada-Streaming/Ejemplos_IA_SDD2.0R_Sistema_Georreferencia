using System.Net;
using System.Text;
using FluentAssertions;
using GeoVial.Sync;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Sesión única del cliente móvil (US-40): login centralizado que asienta el token
/// en el HttpClient compartido y no tumba la app ante credenciales o red en falla.
/// </summary>
public class ServicioSesionTests
{
    private sealed class RutasHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        public RutasHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;

        public int Llamadas { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Llamadas++;
            return Task.FromResult(_responder(request));
        }
    }

    private static HttpResponseMessage Json(HttpStatusCode codigo, string cuerpo) =>
        new(codigo) { Content = new StringContent(cuerpo, Encoding.UTF8, "application/json") };

    [Fact] // US-40: el login exitoso asienta el Bearer en el HttpClient compartido
    public async Task Login_exitoso_asienta_el_token_y_marca_autenticado()
    {
        var handler = new RutasHandler(_ => Json(HttpStatusCode.OK, """{"accessToken":"tok-123"}"""));
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5080/") };
        var sesion = new ServicioSesion(http);

        var r = await sesion.IngresarAsync("raiz", "GeoVial.Raiz.2026");

        r.Exito.Should().BeTrue();
        sesion.Autenticado.Should().BeTrue();
        sesion.Usuario.Should().Be("raiz");
        http.DefaultRequestHeaders.Authorization!.Scheme.Should().Be("Bearer");
        http.DefaultRequestHeaders.Authorization.Parameter.Should().Be("tok-123");
    }

    [Fact] // US-40: credenciales vacías no llegan a tocar la red (validación previa)
    public async Task Credenciales_vacias_no_llaman_al_backend()
    {
        var handler = new RutasHandler(_ => Json(HttpStatusCode.OK, "{}"));
        var sesion = new ServicioSesion(new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5080/") });

        var r = await sesion.IngresarAsync("   ", "");

        r.Exito.Should().BeFalse();
        r.Mensaje.Should().Contain("usuario y clave");
        handler.Llamadas.Should().Be(0);
        sesion.Autenticado.Should().BeFalse();
    }

    [Fact] // US-40: 401 del backend da un mensaje claro y no autentica (sin tumbar la app)
    public async Task Login_con_credenciales_invalidas_devuelve_fallo_claro()
    {
        var handler = new RutasHandler(_ => Json(HttpStatusCode.Unauthorized, "{}"));
        var sesion = new ServicioSesion(new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5080/") });

        var r = await sesion.IngresarAsync("raiz", "mala");

        r.Exito.Should().BeFalse();
        r.Mensaje.Should().Contain("incorrect");
        sesion.Autenticado.Should().BeFalse();
    }

    [Fact] // US-40: una excepción de red se traduce en mensaje, no en crash
    public async Task Falla_de_red_no_lanza_y_devuelve_mensaje()
    {
        var handler = new RutasHandler(_ => throw new HttpRequestException("conexión rechazada"));
        var sesion = new ServicioSesion(new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5080/") });

        var r = await sesion.IngresarAsync("raiz", "GeoVial.Raiz.2026");

        r.Exito.Should().BeFalse();
        r.Mensaje.Should().Contain("backend");
        sesion.Autenticado.Should().BeFalse();
    }

    [Fact] // US-40: cerrar sesión limpia el token del HttpClient compartido
    public async Task Salir_limpia_el_token()
    {
        var handler = new RutasHandler(_ => Json(HttpStatusCode.OK, """{"accessToken":"tok-123"}"""));
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5080/") };
        var sesion = new ServicioSesion(http);
        await sesion.IngresarAsync("raiz", "GeoVial.Raiz.2026");

        sesion.Salir();

        sesion.Autenticado.Should().BeFalse();
        sesion.Usuario.Should().BeNull();
        http.DefaultRequestHeaders.Authorization.Should().BeNull();
    }

    [Fact] // US-40: el primer relevamiento es el destino por defecto del cliente
    public async Task Primer_relevamiento_devuelve_el_primero_del_backend()
    {
        var id = Guid.NewGuid();
        var handler = new RutasHandler(req =>
            req.RequestUri!.AbsolutePath.EndsWith("/auth/login")
                ? Json(HttpStatusCode.OK, """{"accessToken":"tok-123"}""")
                : Json(HttpStatusCode.OK, $$"""[{"relevamientoId":"{{id}}"}]"""));
        var sesion = new ServicioSesion(new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5080/") });
        await sesion.IngresarAsync("raiz", "GeoVial.Raiz.2026");

        var r = await sesion.PrimerRelevamientoAsync();

        r.Should().Be(id);
    }
}
