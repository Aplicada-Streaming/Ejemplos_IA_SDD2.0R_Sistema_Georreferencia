using FluentAssertions;
using GeoVial.Application.Observabilidad;
using Xunit;

namespace GeoVial.IntegrationTests;

/// <summary>
/// Observabilidad (S69): el backend acompaña cada respuesta con un id de correlación (cabecera
/// <c>X-Correlation-ID</c>), reusando el que envíe el cliente o generando uno, para rastrear la petición.
/// </summary>
public class ObservabilidadTests : IClassFixture<FabricaPruebas>
{
    private readonly FabricaPruebas _factory;

    public ObservabilidadTests(FabricaPruebas factory) => _factory = factory;

    [Fact] // sin cabecera entrante: la respuesta trae un id de correlación generado (no vacío)
    public async Task Responde_con_un_id_de_correlacion_generado()
    {
        var cliente = _factory.CreateClient();

        var resp = await cliente.GetAsync("/health");

        resp.Headers.TryGetValues(Correlacion.Cabecera, out var valores).Should().BeTrue();
        valores!.Single().Should().NotBeNullOrWhiteSpace();
    }

    [Fact] // con cabecera entrante válida: se reusa (eco), para correlacionar a través de saltos
    public async Task Reusa_el_id_de_correlacion_entrante()
    {
        var cliente = _factory.CreateClient();
        var idEntrante = "mi-correlacion-123";
        var pedido = new HttpRequestMessage(HttpMethod.Get, "/health");
        pedido.Headers.Add(Correlacion.Cabecera, idEntrante);

        var resp = await cliente.SendAsync(pedido);

        resp.Headers.GetValues(Correlacion.Cabecera).Single().Should().Be(idEntrante);
    }
}
