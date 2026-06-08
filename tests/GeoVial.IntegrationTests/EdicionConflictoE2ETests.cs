using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoVial.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GeoVial.IntegrationTests;

/// <summary>
/// Prueba E2E del ciclo de edición en conflicto (Sprint 24, CU-07/CU-12). Sobre la API real: sincronizar la
/// creación y dos ediciones del mismo comentario (colisión) hace que el last-write-wins consolide y marque
/// EdicionEnConflicto (RN-04); luego se lista y se confirma. Cierra la última deuda de E2E de conflictos.
/// </summary>
public class EdicionConflictoE2ETests : IClassFixture<FabricaPruebas>
{
    private readonly WebApplicationFactory<Program> _factory;

    public EdicionConflictoE2ETests(FabricaPruebas factory) =>
        _factory = factory.WithWebHostBuilder(b => b.UseEnvironment("Development"));

    private static readonly DateTime T0 = new(2027, 5, 4, 10, 0, 0, DateTimeKind.Utc);

    private static async Task<Guid> CapturarMarcadorAsync(HttpClient cliente, Guid relevamientoId)
    {
        var resp = await cliente.PostAsJsonAsync(
            $"/api/v1/relevamientos/{relevamientoId}/observaciones",
            new CapturarObservacionRequest("foto.jpg", -34.6m, -58.4m));
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await resp.Content.ReadFromJsonAsync<CapturaResponse>())!.MarcadorId!.Value;
    }

    private static async Task<SincronizarResponse> SincronizarAsync(HttpClient cliente, Guid relevamientoId, CambioSyncDto cambio)
    {
        var resp = await cliente.PostAsJsonAsync(
            $"/api/v1/relevamientos/{relevamientoId}/sync",
            new SincronizarRequest(null, new[] { cambio }));
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await resp.Content.ReadFromJsonAsync<SincronizarResponse>())!;
    }

    [Fact] // E2E CU-07/CU-12: sincronizar con colisión marca EdicionEnConflicto; se lista y se confirma
    public async Task Sincronizar_con_colision_y_confirmar_edicion()
    {
        var esc = await EscenarioE2E.SembrarAsync(_factory);
        var cliente = await EscenarioE2E.ClienteAutenticadoAsync(_factory, esc.Usuario, esc.Clave);
        var marcador = await CapturarMarcadorAsync(cliente, esc.RelevamientoId);
        var comentario = Guid.NewGuid();

        // 1) Crear el comentario por sync (T0)
        var crear = await SincronizarAsync(cliente, esc.RelevamientoId,
            new CambioSyncDto(Guid.NewGuid(), 1, comentario, marcador, null, esc.AgenteId, "fisura observada", T0));
        crear.Confirmados.Should().HaveCount(1);
        crear.Conflictos.Should().BeEmpty();

        // 2) Primera edición (T1 > T0): aún no hay edición previa que compita
        var edicion1 = await SincronizarAsync(cliente, esc.RelevamientoId,
            new CambioSyncDto(Guid.NewGuid(), 2, comentario, marcador, null, esc.AgenteId, "fisura de 2mm", T0.AddMinutes(1)));
        edicion1.Confirmados.Should().HaveCount(1);
        edicion1.Conflictos.Should().BeEmpty();

        // 3) Segunda edición (T2): el comentario ya estaba editado → EdicionEnConflicto (RN-04)
        var edicion2 = await SincronizarAsync(cliente, esc.RelevamientoId,
            new CambioSyncDto(Guid.NewGuid(), 2, comentario, marcador, null, esc.AgenteId, "fisura de 3mm", T0.AddMinutes(2)));
        edicion2.Confirmados.Should().HaveCount(1);
        edicion2.Conflictos.Should().Contain(c => c.Tipo == 2); // edición en conflicto

        // Listar los conflictos pendientes: aparece el de edición, con su recurso (el comentario)
        var pendientes = (await (await cliente.GetAsync($"/api/v1/relevamientos/{esc.RelevamientoId}/conflictos"))
            .Content.ReadFromJsonAsync<List<ConflictoPendienteDto>>())!;
        var conflicto = pendientes.Should().ContainSingle(c => c.Tipo == 2).Subject;
        conflicto.Recurso.Should().Be(comentario);

        // Confirmar la edición (decisión 3 = ConfirmarEdicion)
        var resol = await cliente.PostAsJsonAsync(
            $"/api/v1/conflictos/{conflicto.ConflictoSyncId}/resolucion",
            new ResolverConflictoRequest(3, null));
        resol.IsSuccessStatusCode.Should().BeTrue();

        // Ya no queda el conflicto de edición pendiente
        var post = (await (await cliente.GetAsync($"/api/v1/relevamientos/{esc.RelevamientoId}/conflictos"))
            .Content.ReadFromJsonAsync<List<ConflictoPendienteDto>>())!;
        post.Should().NotContain(c => c.ConflictoSyncId == conflicto.ConflictoSyncId);
    }

    [Fact] // RC-03: reenviar un CambioId ya aplicado se confirma sin duplicar ni reabrir conflicto
    public async Task Sincronizar_es_idempotente_por_cambio_id()
    {
        var esc = await EscenarioE2E.SembrarAsync(_factory);
        var cliente = await EscenarioE2E.ClienteAutenticadoAsync(_factory, esc.Usuario, esc.Clave);
        var marcador = await CapturarMarcadorAsync(cliente, esc.RelevamientoId);
        var comentario = Guid.NewGuid();
        var cambioId = Guid.NewGuid();

        var dto = new CambioSyncDto(cambioId, 1, comentario, marcador, null, esc.AgenteId, "observacion", T0);
        var primera = await SincronizarAsync(cliente, esc.RelevamientoId, dto);
        var reintento = await SincronizarAsync(cliente, esc.RelevamientoId, dto); // mismo CambioId

        primera.Confirmados.Should().Contain(cambioId);
        reintento.Confirmados.Should().Contain(cambioId); // se confirma de nuevo, sin reaplicar
        reintento.Conflictos.Should().BeEmpty();
    }
}
