using System.Net;
using System.Text;
using FluentAssertions;
using GeoVial.Sync;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Reingreso en terreno con verificación biométrica nativa (RN-06, S53): antes de re-autenticar sin clave,
/// el teléfono debe verificar la identidad. Sólo si la verificación tiene éxito se llama al backend.
/// </summary>
public class CoordinadorReingresoTests
{
    private sealed class BiometricoFake : IAutenticadorBiometrico
    {
        private readonly ResultadoBiometrico _resultado;
        private readonly bool _disponible;
        public BiometricoFake(ResultadoBiometrico resultado, bool disponible = true)
        {
            _resultado = resultado;
            _disponible = disponible;
        }
        public int Pedidos { get; private set; }
        public Task<bool> HayMetodoDisponibleAsync() => Task.FromResult(_disponible);
        public Task<ResultadoBiometrico> AutenticarAsync(string motivo, CancellationToken ct = default)
        {
            Pedidos++;
            return Task.FromResult(_resultado);
        }
    }

    private sealed class RutasHandler : HttpMessageHandler
    {
        public int Llamadas { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Llamadas++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"accessToken":"tok-reingreso"}""", Encoding.UTF8, "application/json"),
            });
        }
    }

    private static (CoordinadorReingreso Coord, ServicioSesion Sesion, RutasHandler Handler) Armar(BiometricoFake bio)
    {
        var handler = new RutasHandler();
        var sesion = new ServicioSesion(new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5080/") });
        return (new CoordinadorReingreso(sesion, bio), sesion, handler);
    }

    [Fact] // RN-06: verificación biométrica OK → reingresa contra el backend y asienta el token
    public async Task Biometrico_ok_reingresa()
    {
        var bio = new BiometricoFake(ResultadoBiometrico.Exito);
        var (coord, sesion, handler) = Armar(bio);

        var r = await coord.ReingresarAsync("campo");

        r.Exito.Should().BeTrue();
        sesion.Autenticado.Should().BeTrue();
        bio.Pedidos.Should().Be(1);
        handler.Llamadas.Should().Be(1); // se llamó al /auth/reingreso
    }

    [Theory] // RN-06: si la verificación no tiene éxito, NO se llama al backend y se explica por qué
    [InlineData(ResultadoBiometrico.Fallo, "verificar tu identidad")]
    [InlineData(ResultadoBiometrico.Cancelado, "cancelado")]
    [InlineData(ResultadoBiometrico.NoDisponible, "huella")]
    public async Task Biometrico_no_exitoso_no_reingresa(ResultadoBiometrico resultado, string fragmentoMensaje)
    {
        var bio = new BiometricoFake(resultado);
        var (coord, sesion, handler) = Armar(bio);

        var r = await coord.ReingresarAsync("campo");

        r.Exito.Should().BeFalse();
        r.Mensaje.Should().Contain(fragmentoMensaje);
        sesion.Autenticado.Should().BeFalse();
        handler.Llamadas.Should().Be(0); // nunca tocó el backend
    }

    [Fact] // sin usuario recordado, ni siquiera se pide la verificación
    public async Task Sin_usuario_no_pide_biometrico()
    {
        var bio = new BiometricoFake(ResultadoBiometrico.Exito);
        var (coord, _, handler) = Armar(bio);

        var r = await coord.ReingresarAsync("   ");

        r.Exito.Should().BeFalse();
        r.Mensaje.Should().Contain("usuario recordado");
        bio.Pedidos.Should().Be(0);
        handler.Llamadas.Should().Be(0);
    }

    [Fact] // la disponibilidad del método se delega al autenticador nativo
    public async Task HayMetodoDisponible_delega_en_el_autenticador()
    {
        var (coordSi, _, _) = Armar(new BiometricoFake(ResultadoBiometrico.Exito, disponible: true));
        var (coordNo, _, _) = Armar(new BiometricoFake(ResultadoBiometrico.Exito, disponible: false));

        (await coordSi.HayMetodoDisponibleAsync()).Should().BeTrue();
        (await coordNo.HayMetodoDisponibleAsync()).Should().BeFalse();
    }
}
