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

    // S49: motor con resultado configurable para verificar los conteos del evento de fin de sincronización.
    private sealed class MotorConResultadoFake : ISyncEngine
    {
        private readonly SyncResult _resultado;
        public MotorConResultadoFake(int confirmados, int conflictos)
        {
            _resultado = new SyncResult(
                Enumerable.Range(0, confirmados).Select(_ => Guid.NewGuid()).ToList(),
                Enumerable.Range(0, conflictos).Select(_ => new ConflictInfo(Guid.NewGuid(), ConflictKind.FieldConflict, Array.Empty<string>())).ToList(),
                Array.Empty<string>());
        }
        public Task<SyncResult> SynchronizeAsync(Guid relevamientoId, DateTime? since = null, CancellationToken ct = default) =>
            Task.FromResult(_resultado);
    }

    private sealed class MotorQueFallaFake : ISyncEngine
    {
        public Task<SyncResult> SynchronizeAsync(Guid relevamientoId, DateTime? since = null, CancellationToken ct = default) =>
            throw new SyncInterruptedException("corte simulado", new IOException("red caída"));
    }

    [Fact] // S49: al terminar con éxito emite SincronizacionCompletada con los conteos y capturas subidas
    public async Task Completada_al_exito_lleva_conteos_y_capturas()
    {
        var colaCapturas = new ColaCapturasFake();
        colaCapturas.Items.Add(Captura());
        colaCapturas.Items.Add(Captura());
        var capturas = new MotorCapturas(colaCapturas, new BackendCapturasFake());
        using var coord = new CoordinadorAutoSync(new ConectividadFake(), new MotorConResultadoFake(confirmados: 3, conflictos: 1),
            new ColaCambiosFake(), capturas, proveedorRelevamiento: () => Guid.NewGuid());
        ResultadoAutoSync? recibido = null;
        coord.SincronizacionCompletada += (_, r) => recibido = r;

        await coord.SincronizarSiCorrespondeAsync();

        recibido.Should().NotBeNull();
        recibido!.Confirmados.Should().Be(3);
        recibido.Conflictos.Should().Be(1);
        recibido.CapturasSubidas.Should().Be(2);
        recibido.HuboError.Should().BeFalse();
    }

    [Fact] // S49: si el motor falla, emite el evento con HuboError y relanza (se reintentará)
    public async Task Completada_al_fallar_marca_error_y_relanza()
    {
        using var coord = new CoordinadorAutoSync(new ConectividadFake(), new MotorQueFallaFake(), new ColaCambiosFake(),
            proveedorRelevamiento: () => Guid.NewGuid());
        ResultadoAutoSync? recibido = null;
        coord.SincronizacionCompletada += (_, r) => recibido = r;

        var acto = async () => await coord.SincronizarSiCorrespondeAsync();

        await acto.Should().ThrowAsync<SyncInterruptedException>();
        recibido.Should().NotBeNull();
        recibido!.HuboError.Should().BeTrue();
    }

    [Fact] // S49: sin relevamiento no hay sincronización ni evento de fin
    public async Task Sin_relevamiento_no_emite_completada()
    {
        using var coord = new CoordinadorAutoSync(new ConectividadFake(), new MotorFake(), new ColaCambiosFake(),
            proveedorRelevamiento: () => null);
        var emitido = false;
        coord.SincronizacionCompletada += (_, _) => emitido = true;

        await coord.SincronizarSiCorrespondeAsync();

        emitido.Should().BeFalse();
    }
}
