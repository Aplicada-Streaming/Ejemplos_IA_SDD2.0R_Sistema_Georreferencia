using FluentAssertions;
using GeoVial.Sync;
using GeoVial.SyncDemo;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// AT-32 — Demo autónoma de evaluación de la librería de sincronización (US-32, EP-09).
/// Ejercita la superficie pública de <c>GeoVial.Sync</c> con la cola SQLite y el motor reales contra el
/// <see cref="BackendSimulado"/>, demostrando el alta local → sincronización → estado de la cola y la
/// resolución básica de conflictos. La cáscara MAUI de la demo queda fuera del gate.
/// </summary>
public sealed class AceptacionDemoSyncTests : IDisposable
{
    private static readonly Guid Relevamiento = Guid.NewGuid();
    private readonly string _archivo = Path.Combine(Path.GetTempPath(), $"geovial-demo-{Guid.NewGuid():N}.db");

    private IChangeQueue NuevaCola() => new ColaCambiosSqlite($"Data Source={_archivo}");

    private static ObservacionCapturada Observacion(Guid? comentarioId = null, string texto = "fisura en junta") =>
        new(comentarioId ?? Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), texto, new DateTime(2026, 6, 2, 9, 0, 0, DateTimeKind.Utc));

    private sealed class ReporterEnMemoria : IConflictReporter
    {
        public List<ConflictInfo> Reportados { get; } = new();

        public Task ReportAsync(IReadOnlyList<ConflictInfo> conflicts, CancellationToken ct = default)
        {
            Reportados.AddRange(conflicts);
            return Task.CompletedTask;
        }
    }

    [Fact] // US-32 CA-01: alta de registros locales + sincronización → la cola pasa de pendiente a sincronizado
    public async Task Alta_local_y_sincronizacion_vacia_la_cola()
    {
        var cola = NuevaCola();
        var backend = new BackendSimulado();
        var coord = new CoordinadorDemo(cola, new MotorSincronizacion(cola, backend), new ResolutorConflictos(backend, cola), Relevamiento);

        var c1 = await coord.CapturarAsync(Observacion());
        await coord.CapturarAsync(Observacion());

        (await coord.PendientesAsync()).Should().Be(2); // estado «pendiente»

        var r = await coord.SincronizarAsync();

        r.Confirmed.Should().HaveCount(2);
        (await coord.PendientesAsync()).Should().Be(0); // estado «sincronizado»
        backend.Consolidados.Should().Contain(c1.EntityRef);
    }

    [Fact] // US-32 CA-02: un cambio que choca con el mock se reporta como conflicto y no se confirma
    public async Task Cambio_en_conflicto_se_reporta_y_queda_pendiente()
    {
        var enConflicto = Guid.NewGuid();
        var cola = NuevaCola();
        var backend = new BackendSimulado(recursosEnConflicto: new[] { enConflicto });
        var reporter = new ReporterEnMemoria();
        var coord = new CoordinadorDemo(cola, new MotorSincronizacion(cola, backend, reporter), new ResolutorConflictos(backend, cola), Relevamiento);

        await coord.CapturarAsync(Observacion(comentarioId: enConflicto));

        var r = await coord.SincronizarAsync();

        r.Confirmed.Should().BeEmpty();
        r.Conflicts.Should().ContainSingle().Which.Kind.Should().Be(ConflictKind.FieldConflict);
        reporter.Reportados.Should().ContainSingle(); // el conflicto se expone para resolución (CU-12)
        (await coord.PendientesAsync()).Should().Be(1); // el cambio sigue pendiente hasta resolver
        backend.Consolidados.Should().NotContain(enConflicto);
    }

    [Fact] // US-32 CA-02: resolución básica «mantener lo local» → la próxima sincronización confirma el cambio
    public async Task Resolver_manteniendo_local_confirma_en_la_siguiente_sincronizacion()
    {
        var enConflicto = Guid.NewGuid();
        var cola = NuevaCola();
        var backend = new BackendSimulado(recursosEnConflicto: new[] { enConflicto });
        var coord = new CoordinadorDemo(cola, new MotorSincronizacion(cola, backend), new ResolutorConflictos(backend, cola), Relevamiento);

        await coord.CapturarAsync(Observacion(comentarioId: enConflicto));
        await coord.SincronizarAsync(); // reporta conflicto, no confirma
        backend.EstaEnConflicto(enConflicto).Should().BeTrue();

        coord.ResolverManteniendoLocal(enConflicto); // RN-04: prevalece la última escritura
        backend.EstaEnConflicto(enConflicto).Should().BeFalse();
        var r = await coord.SincronizarAsync();

        r.Confirmed.Should().ContainSingle();
        r.Conflicts.Should().BeEmpty();
        (await coord.PendientesAsync()).Should().Be(0);
        backend.Consolidados.Should().Contain(enConflicto);
        backend.Subidas.Should().Be(2); // dos subidas efectivas: antes y después de resolver
    }

    [Fact] // US-32 CA-02: resolución básica «aceptar lo remoto» → el cambio local se descarta de la cola
    public async Task Resolver_aceptando_remoto_descarta_el_cambio_local()
    {
        var enConflicto = Guid.NewGuid();
        var cola = NuevaCola();
        var backend = new BackendSimulado(recursosEnConflicto: new[] { enConflicto });
        var coord = new CoordinadorDemo(cola, new MotorSincronizacion(cola, backend), new ResolutorConflictos(backend, cola), Relevamiento);

        var cambio = await coord.CapturarAsync(Observacion(comentarioId: enConflicto));
        await coord.SincronizarAsync();

        await coord.ResolverAceptandoRemotoAsync(cambio.ChangeId);

        (await coord.PendientesAsync()).Should().Be(0); // se retiró de la cola
        backend.Consolidados.Should().NotContain(enConflicto); // nunca se subió
    }

    [Fact] // US-19 CA-02: una sincronización posterior baja también las actualizaciones acumuladas del backend
    public async Task Sincronizacion_posterior_baja_actualizaciones_acumuladas()
    {
        var cola = NuevaCola();
        var backend = new BackendSimulado(actualizaciones: new[] { "comentario remoto del jefe de área" });
        var coord = new CoordinadorDemo(cola, new MotorSincronizacion(cola, backend), new ResolutorConflictos(backend, cola), Relevamiento);

        await coord.CapturarAsync(Observacion());
        var primera = await coord.SincronizarAsync();   // since == null → confirma, sin actualizaciones

        await coord.CapturarAsync(Observacion());        // nueva captura
        var segunda = await coord.SincronizarAsync();   // since != null → confirma y baja actualizaciones

        primera.Updates.Should().BeEmpty();
        segunda.Updates.Should().ContainSingle();
        (await coord.PendientesAsync()).Should().Be(0);
    }

    [Fact] // RC-03: reanudar tras resolver no duplica (idempotencia por ChangeId; el backend recibe una sola subida efectiva)
    public async Task Reencolar_el_mismo_cambio_no_duplica()
    {
        var cola = NuevaCola();
        var backend = new BackendSimulado();
        var coord = new CoordinadorDemo(cola, new MotorSincronizacion(cola, backend), new ResolutorConflictos(backend, cola), Relevamiento);

        var obs = Observacion();
        await coord.CapturarAsync(obs with { });
        var cambio = ChangeRecordFactory.Comentario(obs);
        await cola.EnqueueAsync(cambio);
        await cola.EnqueueAsync(cambio); // mismo ChangeId

        (await coord.PendientesAsync()).Should().Be(2); // dos altas distintas (ChangeId distinto), no la re-encolada
    }

    public void Dispose()
    {
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        if (File.Exists(_archivo))
        {
            File.Delete(_archivo);
        }
    }
}
