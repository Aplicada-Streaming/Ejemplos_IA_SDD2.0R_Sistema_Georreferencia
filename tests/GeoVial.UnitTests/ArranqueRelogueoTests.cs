using System.Net;
using System.Text;
using FluentAssertions;
using GeoVial.Sync;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Decisión de arranque/relogueo del móvil (S55): al reabrir con sesión persistida se pide el método del
/// teléfono; la vuelta de la cámara no lo re-pide; "cerrar sesión" lleva a usuario/clave.
/// </summary>
public class PoliticaArranqueTests
{
    [Fact] // sin sesión persistida: siempre usuario y clave
    public void Sin_token_pide_clave()
    {
        PoliticaArranque.Decidir(hayTokenPersistido: false, hayMetodoDispositivo: true, volviendoDeCamara: false)
            .Should().Be(DecisionArranque.PedirClave);
        PoliticaArranque.Decidir(false, false, true).Should().Be(DecisionArranque.PedirClave);
    }

    [Fact] // volviendo de la cámara (proceso recreado): entra sin re-pedir el método
    public void Token_y_camara_entra_sin_metodo()
    {
        PoliticaArranque.Decidir(hayTokenPersistido: true, hayMetodoDispositivo: true, volviendoDeCamara: true)
            .Should().Be(DecisionArranque.Entrar);
        PoliticaArranque.Decidir(true, false, true).Should().Be(DecisionArranque.Entrar);
    }

    [Fact] // reapertura normal con sesión + método del teléfono: pide el método
    public void Token_y_metodo_pide_biometrico()
    {
        PoliticaArranque.Decidir(hayTokenPersistido: true, hayMetodoDispositivo: true, volviendoDeCamara: false)
            .Should().Be(DecisionArranque.PedirBiometrico);
    }

    [Fact] // sesión persistida pero sin método del teléfono: cae a usuario y clave
    public void Token_sin_metodo_pide_clave()
    {
        PoliticaArranque.Decidir(hayTokenPersistido: true, hayMetodoDispositivo: false, volviendoDeCamara: false)
            .Should().Be(DecisionArranque.PedirClave);
    }

    [Theory] // traducción del resultado del método del teléfono
    [InlineData(ResultadoBiometrico.Exito, DecisionArranque.Entrar)]
    [InlineData(ResultadoBiometrico.NoDisponible, DecisionArranque.PedirClave)]
    [InlineData(ResultadoBiometrico.Cancelado, DecisionArranque.Bloqueado)]
    [InlineData(ResultadoBiometrico.Fallo, DecisionArranque.Bloqueado)]
    public void TrasBiometrico_traduce(ResultadoBiometrico r, DecisionArranque esperado) =>
        PoliticaArranque.TrasBiometrico(r).Should().Be(esperado);
}

public class CoordinadorArranqueTests
{
    private sealed class BiometricoFake : IAutenticadorBiometrico
    {
        private readonly ResultadoBiometrico _resultado;
        private readonly bool _disponible;
        public BiometricoFake(ResultadoBiometrico resultado = ResultadoBiometrico.Exito, bool disponible = true)
        {
            _resultado = resultado; _disponible = disponible;
        }
        public int Pedidos { get; private set; }
        public Task<bool> HayMetodoDisponibleAsync() => Task.FromResult(_disponible);
        public Task<ResultadoBiometrico> AutenticarAsync(string motivo, CancellationToken ct = default)
        {
            Pedidos++; return Task.FromResult(_resultado);
        }
    }

    private sealed class AlmacenFake : IAlmacenTokenSesion
    {
        private (string Token, string Usuario)? _g;
        public AlmacenFake(string? token = null) { if (token is not null) _g = (token, "campo1"); }
        public Task GuardarAsync(string token, string usuario) { _g = (token, usuario); return Task.CompletedTask; }
        public Task<(string Token, string Usuario)?> LeerAsync() => Task.FromResult(_g);
        public Task LimpiarAsync() { _g = null; return Task.CompletedTask; }
    }

    private sealed class MarcadorCapturaFake : IMarcadorCaptura
    {
        public bool EnCurso { get; set; }
        public Task MarcarEnCursoAsync() { EnCurso = true; return Task.CompletedTask; }
        public Task<bool> EstaEnCursoAsync() => Task.FromResult(EnCurso);
        public Task LimpiarAsync() { EnCurso = false; return Task.CompletedTask; }
    }

    private sealed class RutasHandler : HttpMessageHandler
    {
        public int Llamadas { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Llamadas++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            { Content = new StringContent("{}", Encoding.UTF8, "application/json") });
        }
    }

    private static (CoordinadorArranque Coord, ServicioSesion Sesion, RutasHandler Handler, BiometricoFake Bio, MarcadorCapturaFake Cam)
        Armar(string? tokenPersistido, ResultadoBiometrico bio = ResultadoBiometrico.Exito, bool metodo = true, bool enCamara = false)
    {
        var handler = new RutasHandler();
        var sesion = new ServicioSesion(new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5080/") }, new AlmacenFake(tokenPersistido));
        var bioFake = new BiometricoFake(bio, metodo);
        var cam = new MarcadorCapturaFake { EnCurso = enCamara };
        return (new CoordinadorArranque(sesion, bioFake, cam), sesion, handler, bioFake, cam);
    }

    [Fact] // vuelta de cámara: entra restaurando, sin pedir el método y limpiando la marca
    public async Task Vuelta_de_camara_entra_y_limpia()
    {
        var (coord, sesion, handler, bio, cam) = Armar("tok", enCamara: true);

        var d = await coord.DecidirInicialAsync();

        d.Should().Be(DecisionArranque.Entrar);
        sesion.Autenticado.Should().BeTrue();
        cam.EnCurso.Should().BeFalse();
        bio.Pedidos.Should().Be(0);
        handler.Llamadas.Should().Be(0); // restauración offline, sin red
    }

    [Fact] // reapertura normal con sesión + método: pide el método (aún no entra)
    public async Task Reapertura_con_metodo_pide_biometrico()
    {
        var (coord, sesion, _, _, _) = Armar("tok", metodo: true, enCamara: false);

        var d = await coord.DecidirInicialAsync();

        d.Should().Be(DecisionArranque.PedirBiometrico);
        sesion.Autenticado.Should().BeFalse();
    }

    [Fact] // sin sesión persistida: pide usuario y clave, sin pedir el método
    public async Task Sin_token_pide_clave()
    {
        var (coord, _, _, bio, _) = Armar(tokenPersistido: null);

        (await coord.DecidirInicialAsync()).Should().Be(DecisionArranque.PedirClave);
        bio.Pedidos.Should().Be(0);
    }

    [Fact] // desbloqueo con método OK: entra restaurando la sesión sin tocar la red
    public async Task Desbloqueo_ok_entra_sin_red()
    {
        var (coord, sesion, handler, _, _) = Armar("tok", bio: ResultadoBiometrico.Exito);

        var d = await coord.DesbloquearConBiometricoAsync();

        d.Should().Be(DecisionArranque.Entrar);
        sesion.Autenticado.Should().BeTrue();
        handler.Llamadas.Should().Be(0);
    }

    [Fact] // desbloqueo cancelado: queda bloqueado, no entra
    public async Task Desbloqueo_cancelado_bloquea()
    {
        var (coord, sesion, _, _, _) = Armar("tok", bio: ResultadoBiometrico.Cancelado);

        (await coord.DesbloquearConBiometricoAsync()).Should().Be(DecisionArranque.Bloqueado);
        sesion.Autenticado.Should().BeFalse();
    }
}
