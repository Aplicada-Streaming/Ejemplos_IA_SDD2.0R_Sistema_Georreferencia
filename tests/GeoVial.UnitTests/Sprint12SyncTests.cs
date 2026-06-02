using System.Text.Json;
using FluentAssertions;
using GeoVial.Sync;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Recolección de observaciones sin conexión: encolado local (US-16, CU-06).</summary>
public sealed class ColectorOfflineTests : IDisposable
{
    private readonly string _archivo = Path.Combine(Path.GetTempPath(), $"geovial-captura-{Guid.NewGuid():N}.db");

    private static ObservacionCapturada Obs(string texto = "fisura") =>
        new(Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), texto, new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc));

    [Fact] // US-16 CA-01: una observación capturada sin señal se guarda localmente (encolada)
    public async Task Recolectar_encola_la_observacion()
    {
        var cola = new ColaCambiosSqlite($"Data Source={_archivo}");
        var colector = new ColectorOffline(cola);
        var obs = Obs();

        await colector.RecolectarAsync(obs);

        var pendientes = await cola.ReadPendingAsync(10);
        pendientes.Should().ContainSingle();
        pendientes[0].Entity.Should().Be("comentario");
        pendientes[0].EntityRef.Should().Be(obs.ComentarioId);
        var payload = JsonSerializer.Deserialize<PayloadComentario>(pendientes[0].Payload, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        payload!.Texto.Should().Be("fisura");
    }

    [Fact] // US-16 CA-02: 100 capturas sin señal se conservan sin pérdida
    public async Task Cien_capturas_se_conservan()
    {
        var cola = new ColaCambiosSqlite($"Data Source={_archivo}");
        var colector = new ColectorOffline(cola);

        for (var i = 0; i < 100; i++)
        {
            await colector.RecolectarAsync(Obs($"obs-{i}"));
        }

        (await cola.PendingCountAsync()).Should().Be(100);
    }

    [Fact] // US-16 CA-03: sin espacio local, responde con el error tipado y conserva lo guardado
    public async Task Sin_espacio_propaga_almacenamiento_insuficiente()
    {
        var colector = new ColectorOffline(new ColaQueSeQuedaSinEspacio());

        var accion = async () => await colector.RecolectarAsync(Obs());

        await accion.Should().ThrowAsync<AlmacenamientoLocalInsuficienteException>();
    }

    private sealed class ColaQueSeQuedaSinEspacio : IChangeQueue
    {
        public Task EnqueueAsync(ChangeRecord change, CancellationToken ct = default) =>
            throw new AlmacenamientoLocalInsuficienteException("disco lleno");

        public Task<IReadOnlyList<ChangeRecord>> ReadPendingAsync(int max, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<ChangeRecord>>(Array.Empty<ChangeRecord>());

        public Task MarkConfirmedAsync(IEnumerable<Guid> changeIds, CancellationToken ct = default) => Task.CompletedTask;

        public Task<int> PendingCountAsync(CancellationToken ct = default) => Task.FromResult(0);
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

/// <summary>Sincronización automática al recuperar conexión (US-19, CU-07 §1).</summary>
public class CoordinadorAutoSyncTests
{
    private static readonly Guid Relevamiento = Guid.NewGuid();

    private sealed class FakeConectividad : IConnectivityMonitor
    {
        public bool IsOnline { get; set; } = true;

        public event EventHandler ConnectivityRestored = delegate { };

        public void RecuperarSenal() => ConnectivityRestored.Invoke(this, EventArgs.Empty);
    }

    private sealed class FakeMotor : ISyncEngine
    {
        private readonly TaskCompletionSource _disparado = new();

        public int Llamadas { get; private set; }
        public Guid? UltimoRelevamiento { get; private set; }
        public Task Disparado => _disparado.Task;

        public Task<SyncResult> SynchronizeAsync(Guid relevamientoId, DateTime? since = null, CancellationToken ct = default)
        {
            Llamadas++;
            UltimoRelevamiento = relevamientoId;
            _disparado.TrySetResult();
            return Task.FromResult(SyncResult.Empty);
        }
    }

    private sealed class FakeCola : IChangeQueue
    {
        private int _pendientes;

        public FakeCola(int pendientes = 0) => _pendientes = pendientes;

        public Task EnqueueAsync(ChangeRecord change, CancellationToken ct = default)
        {
            _pendientes++;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<ChangeRecord>> ReadPendingAsync(int max, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<ChangeRecord>>(Array.Empty<ChangeRecord>());

        public Task MarkConfirmedAsync(IEnumerable<Guid> changeIds, CancellationToken ct = default) => Task.CompletedTask;

        public Task<int> PendingCountAsync(CancellationToken ct = default) => Task.FromResult(_pendientes);
    }

    [Fact] // US-19 CA-01: con cambios pendientes, al recuperar señal dispara la sync y notifica que había pendientes
    public async Task Con_pendientes_dispara_sync_y_notifica()
    {
        var motor = new FakeMotor();
        using var coord = new CoordinadorAutoSync(new FakeConectividad(), motor, new FakeCola(pendientes: 3)) { RelevamientoActivo = Relevamiento };
        var notificado = 0;
        coord.CambiosPendientesDetectados += (_, n) => notificado = n;

        var r = await coord.SincronizarSiCorrespondeAsync();

        motor.Llamadas.Should().Be(1);
        motor.UltimoRelevamiento.Should().Be(Relevamiento);
        notificado.Should().Be(3);
    }

    [Fact] // US-19 CA-02: sin pendientes, sincroniza (baja actualizaciones) pero no notifica pendientes
    public async Task Sin_pendientes_sincroniza_sin_notificar()
    {
        var motor = new FakeMotor();
        using var coord = new CoordinadorAutoSync(new FakeConectividad(), motor, new FakeCola(pendientes: 0)) { RelevamientoActivo = Relevamiento };
        var notificado = false;
        coord.CambiosPendientesDetectados += (_, _) => notificado = true;

        await coord.SincronizarSiCorrespondeAsync();

        motor.Llamadas.Should().Be(1); // baja actualizaciones
        notificado.Should().BeFalse();
    }

    [Fact] // sin conexión no sincroniza
    public async Task Sin_conexion_no_sincroniza()
    {
        var motor = new FakeMotor();
        using var coord = new CoordinadorAutoSync(new FakeConectividad { IsOnline = false }, motor, new FakeCola(pendientes: 5)) { RelevamientoActivo = Relevamiento };

        await coord.SincronizarSiCorrespondeAsync();

        motor.Llamadas.Should().Be(0);
    }

    [Fact] // sin relevamiento activo no sincroniza
    public async Task Sin_relevamiento_activo_no_sincroniza()
    {
        var motor = new FakeMotor();
        using var coord = new CoordinadorAutoSync(new FakeConectividad(), motor, new FakeCola(pendientes: 5));

        await coord.SincronizarSiCorrespondeAsync();

        motor.Llamadas.Should().Be(0);
    }

    [Fact] // US-19: el evento de recuperación de señal dispara la sincronización automáticamente
    public async Task El_evento_de_conectividad_dispara_la_sync()
    {
        var conectividad = new FakeConectividad();
        var motor = new FakeMotor();
        using var coord = new CoordinadorAutoSync(conectividad, motor, new FakeCola(pendientes: 1)) { RelevamientoActivo = Relevamiento };

        conectividad.RecuperarSenal();

        await Task.WhenAny(motor.Disparado, Task.Delay(2000));
        motor.Llamadas.Should().Be(1);
    }
}
