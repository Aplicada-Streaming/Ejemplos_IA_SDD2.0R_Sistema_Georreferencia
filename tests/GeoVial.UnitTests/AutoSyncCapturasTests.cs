using FluentAssertions;
using GeoVial.Sync;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Auto-sincronización al recuperar conexión (US-19, F-M-14): el coordinador toma el relevamiento activo de
/// la sesión (proveedor) y, además de sincronizar comentarios, drena la cola de capturas encoladas (S42).
/// </summary>
public class AutoSyncCapturasTests
{
    private sealed class ConectividadFake : IConnectivityMonitor
    {
        public bool IsOnline { get; set; } = true;
        public event EventHandler? ConnectivityRestored;
        public void Reconectar() => ConnectivityRestored?.Invoke(this, EventArgs.Empty);
    }

    private sealed class MotorFake : ISyncEngine
    {
        public Guid? Sincronizado { get; private set; }
        public Task<SyncResult> SynchronizeAsync(Guid relevamientoId, DateTime? since = null, CancellationToken ct = default)
        {
            Sincronizado = relevamientoId;
            return Task.FromResult(SyncResult.Empty);
        }
    }

    private sealed class ColaCambiosFake : IChangeQueue
    {
        private readonly int _pendientes;
        public ColaCambiosFake(int pendientes = 0) => _pendientes = pendientes;
        public Task EnqueueAsync(ChangeRecord change, CancellationToken ct = default) => Task.CompletedTask;
        public Task<IReadOnlyList<ChangeRecord>> ReadPendingAsync(int max, CancellationToken ct = default) =>
            Task.FromResult((IReadOnlyList<ChangeRecord>)Array.Empty<ChangeRecord>());
        public Task MarkConfirmedAsync(IEnumerable<Guid> changeIds, CancellationToken ct = default) => Task.CompletedTask;
        public Task<int> PendingCountAsync(CancellationToken ct = default) => Task.FromResult(_pendientes);
    }

    private sealed class ColaCapturasFake : IColaCapturas
    {
        public readonly List<CapturaPendiente> Items = new();
        public Task EncolarAsync(CapturaPendiente c, CancellationToken ct = default) { Items.Add(c); return Task.CompletedTask; }
        public Task<IReadOnlyList<CapturaPendiente>> LeerPendientesAsync(int max, CancellationToken ct = default) =>
            Task.FromResult((IReadOnlyList<CapturaPendiente>)Items.Take(max).ToList());
        public Task MarcarSubidasAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
        {
            var s = ids.ToHashSet();
            Items.RemoveAll(x => s.Contains(x.CapturaId));
            return Task.CompletedTask;
        }
        public Task<int> PendientesAsync(CancellationToken ct = default) => Task.FromResult(Items.Count);
    }

    private sealed class BackendCapturasFake : ICapturaBackendClient
    {
        public Task<bool> SubirAsync(CapturaPendiente captura, CancellationToken ct = default) => Task.FromResult(true);
    }

    private static CapturaPendiente Captura() =>
        new(Guid.NewGuid(), Guid.NewGuid(), "f.jpg", null, null, new byte[] { 1 }, DateTime.UtcNow);

    [Fact] // F-M-14: sin relevamiento fijado a mano, usa el del proveedor (la sesión)
    public async Task Usa_el_relevamiento_del_proveedor()
    {
        var rel = Guid.NewGuid();
        var motor = new MotorFake();
        using var coord = new CoordinadorAutoSync(new ConectividadFake(), motor, new ColaCambiosFake(),
            proveedorRelevamiento: () => rel);

        await coord.SincronizarSiCorrespondeAsync();

        motor.Sincronizado.Should().Be(rel);
    }

    [Fact] // F-M-14: al sincronizar también se drenan las capturas encoladas (S42)
    public async Task Drena_las_capturas_al_sincronizar()
    {
        var colaCapturas = new ColaCapturasFake();
        colaCapturas.Items.Add(Captura());
        colaCapturas.Items.Add(Captura());
        var capturas = new MotorCapturas(colaCapturas, new BackendCapturasFake());
        using var coord = new CoordinadorAutoSync(new ConectividadFake(), new MotorFake(), new ColaCambiosFake(),
            capturas, proveedorRelevamiento: () => Guid.NewGuid());

        await coord.SincronizarSiCorrespondeAsync();

        (await colaCapturas.PendientesAsync()).Should().Be(0);
    }

    [Fact] // sin relevamiento (ni fijado ni del proveedor) no sincroniza ni drena
    public async Task Sin_relevamiento_no_hace_nada()
    {
        var colaCapturas = new ColaCapturasFake();
        colaCapturas.Items.Add(Captura());
        var capturas = new MotorCapturas(colaCapturas, new BackendCapturasFake());
        var motor = new MotorFake();
        using var coord = new CoordinadorAutoSync(new ConectividadFake(), motor, new ColaCambiosFake(),
            capturas, proveedorRelevamiento: () => null);

        await coord.SincronizarSiCorrespondeAsync();

        motor.Sincronizado.Should().BeNull();
        (await colaCapturas.PendientesAsync()).Should().Be(1, "sin relevamiento no se drenan las capturas");
    }

    [Fact] // sin conexión no sincroniza aunque haya relevamiento
    public async Task Sin_conexion_no_sincroniza()
    {
        var motor = new MotorFake();
        using var coord = new CoordinadorAutoSync(new ConectividadFake { IsOnline = false }, motor, new ColaCambiosFake(),
            proveedorRelevamiento: () => Guid.NewGuid());

        await coord.SincronizarSiCorrespondeAsync();

        motor.Sincronizado.Should().BeNull();
    }
}
