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

    [Fact] // F-M-05: sin selección, el relevamiento activo se resuelve por defecto (el único disponible)
    public async Task Relevamiento_activo_por_defecto_es_el_unico_disponible()
    {
        var id = Guid.NewGuid();
        var handler = new RutasHandler(req =>
            req.RequestUri!.AbsolutePath.EndsWith("/auth/login")
                ? Json(HttpStatusCode.OK, """{"accessToken":"tok-123"}""")
                : Json(HttpStatusCode.OK, $$"""[{"relevamientoId":"{{id}}","identificacionObra":"Obra","estado":1,"agentesVigentes":[]}]"""));
        var sesion = new ServicioSesion(new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5080/") });
        await sesion.IngresarAsync("raiz", "GeoVial.Raiz.2026");

        var r = await sesion.RelevamientoActivoAsync();

        r.Should().Be(id);
    }

    [Fact] // F-M-05: una vez elegido un relevamiento, queda como activo (y sirve offline sin re-consultar)
    public async Task Seleccionar_relevamiento_lo_fija_como_activo()
    {
        var elegido = Guid.NewGuid();
        var handler = new RutasHandler(_ => Json(HttpStatusCode.OK, """{"accessToken":"tok-123"}"""));
        var sesion = new ServicioSesion(new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5080/") });
        await sesion.IngresarAsync("raiz", "GeoVial.Raiz.2026");

        sesion.SeleccionarRelevamiento(elegido);

        sesion.RelevamientoActivoId.Should().Be(elegido);
        (await sesion.RelevamientoActivoAsync()).Should().Be(elegido); // no vuelve a consultar el backend
    }

    [Fact] // F-M-04: el listado marca como asignado el relevamiento cuyo agente vigente es el usuario en sesión
    public async Task Listar_relevamientos_marca_los_asignados_al_usuario()
    {
        // sub del usuario en sesión (claim del JWT)
        var usuarioId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var token = JwtConSub(usuarioId);
        var asignado = Guid.NewGuid();
        var otro = Guid.NewGuid();
        var handler = new RutasHandler(req =>
            req.RequestUri!.AbsolutePath.EndsWith("/auth/login")
                ? Json(HttpStatusCode.OK, $$"""{"accessToken":"{{token}}"}""")
                : Json(HttpStatusCode.OK, $$"""
                    [{"relevamientoId":"{{otro}}","identificacionObra":"Zeta","estado":1,"agentesVigentes":[]},
                     {"relevamientoId":"{{asignado}}","identificacionObra":"Alfa","estado":1,"agentesVigentes":["{{usuarioId}}"]}]
                    """));
        var sesion = new ServicioSesion(new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5080/") });
        await sesion.IngresarAsync("pedro", "x");

        var lista = await sesion.ListarRelevamientosAsync();

        lista[0].RelevamientoId.Should().Be(asignado, "los asignados van primero");
        lista[0].Asignado.Should().BeTrue();
        lista.Single(r => r.RelevamientoId == otro).Asignado.Should().BeFalse();
        sesion.UsuarioId.Should().Be(usuarioId);
    }

    [Fact] // RN-06: el reingreso en terreno con método presente re-autentica y asienta el token (sin clave)
    public async Task Reingreso_con_metodo_presente_asienta_el_token()
    {
        var handler = new RutasHandler(req =>
            req.RequestUri!.AbsolutePath.EndsWith("/auth/reingreso")
                ? Json(HttpStatusCode.OK, """{"accessToken":"tok-reingreso"}""")
                : Json(HttpStatusCode.OK, "{}"));
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5080/") };
        var sesion = new ServicioSesion(http);

        var r = await sesion.ReingresarAsync("pedro", metodoPresente: true);

        r.Exito.Should().BeTrue();
        sesion.Autenticado.Should().BeTrue();
        http.DefaultRequestHeaders.Authorization!.Parameter.Should().Be("tok-reingreso");
    }

    [Fact] // RN-06: sin el método de seguridad presente, el reingreso ni siquiera llama al backend
    public async Task Reingreso_sin_metodo_no_llama_al_backend()
    {
        var handler = new RutasHandler(_ => Json(HttpStatusCode.OK, "{}"));
        var sesion = new ServicioSesion(new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5080/") });

        var r = await sesion.ReingresarAsync("pedro", metodoPresente: false);

        r.Exito.Should().BeFalse();
        r.Mensaje.Should().Contain("método de seguridad");
        handler.Llamadas.Should().Be(0);
    }

    [Fact] // RN-06: si el método no está configurado en el backend (409), el reingreso da un mensaje claro
    public async Task Reingreso_sin_metodo_configurado_devuelve_mensaje()
    {
        var handler = new RutasHandler(_ => Json(HttpStatusCode.Conflict, """{"codigo":"REINGRESO_SIN_METODO_SEGURIDAD"}"""));
        var sesion = new ServicioSesion(new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5080/") });

        var r = await sesion.ReingresarAsync("pedro", metodoPresente: true);

        r.Exito.Should().BeFalse();
        r.Mensaje.Should().Contain("usuario y clave");
        sesion.Autenticado.Should().BeFalse();
    }

    [Fact] // RN-06: configurar el método de seguridad (204) habilita luego el modo offline (204)
    public async Task Configurar_metodo_y_habilitar_offline_ok()
    {
        var handler = new RutasHandler(_ => new HttpResponseMessage(HttpStatusCode.NoContent));
        var sesion = new ServicioSesion(new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5080/") });

        (await sesion.ConfigurarMetodoSeguridadAsync()).Should().BeTrue();
        (await sesion.HabilitarOfflineAsync()).Should().BeTrue();
    }

    [Fact] // RN-06: si el backend rechaza habilitar offline (409, falta método), devuelve false sin lanzar
    public async Task Habilitar_offline_sin_metodo_devuelve_false()
    {
        var handler = new RutasHandler(_ => Json(HttpStatusCode.Conflict, """{"codigo":"OFFLINE_NO_HABILITADO"}"""));
        var sesion = new ServicioSesion(new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5080/") });

        (await sesion.HabilitarOfflineAsync()).Should().BeFalse();
    }

    // Arma un JWT de prueba (header.payload.firma) con el claim sub indicado; sólo el payload importa.
    private static string JwtConSub(Guid sub)
    {
        static string B64Url(string s) =>
            Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(s)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        return $"{B64Url("{\"alg\":\"HS256\"}")}.{B64Url($"{{\"sub\":\"{sub}\"}}")}.firma";
    }
}
