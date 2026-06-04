using FluentAssertions;
using GeoVial.Sync;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Captura offline real (US-16, CU-06/CU-07): el motor que drena la cola de capturas (foto + coordenada)
/// subiéndolas al backend, y la cola SQLite que las conserva sin conexión. Cierra la brecha F-M-12/13 de
/// la revisión funcional (la captura ya no postea directo; se encola y se sube al sincronizar).
/// </summary>
public class MotorCapturasTests
{
    private sealed class ColaFake : IColaCapturas
    {
        public readonly List<CapturaPendiente> Items = new();

        public Task EncolarAsync(CapturaPendiente captura, CancellationToken ct = default)
        {
            if (Items.All(x => x.CapturaId != captura.CapturaId))
            {
                Items.Add(captura);
            }

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<CapturaPendiente>> LeerPendientesAsync(int max, CancellationToken ct = default) =>
            Task.FromResult((IReadOnlyList<CapturaPendiente>)Items.OrderBy(x => x.Momento).Take(max).ToList());

        public Task MarcarSubidasAsync(IEnumerable<Guid> capturaIds, CancellationToken ct = default)
        {
            var ids = capturaIds.ToHashSet();
            Items.RemoveAll(x => ids.Contains(x.CapturaId));
            return Task.CompletedTask;
        }

        public Task<int> PendientesAsync(CancellationToken ct = default) => Task.FromResult(Items.Count);
    }

    private sealed class BackendFake : ICapturaBackendClient
    {
        public Func<CapturaPendiente, bool> Responder { get; set; } = _ => true;
        public List<Guid> Subidas { get; } = new();

        public Task<bool> SubirAsync(CapturaPendiente captura, CancellationToken ct = default)
        {
            var ok = Responder(captura);
            if (ok)
            {
                Subidas.Add(captura.CapturaId);
            }

            return Task.FromResult(ok);
        }
    }

    private static CapturaPendiente Captura(int orden) => new(
        Guid.NewGuid(), Guid.NewGuid(), $"foto{orden}.jpg", -34.6m, -58.4m, new byte[] { 1, 2, 3 },
        new DateTime(2026, 6, 1, 10, orden, 0, DateTimeKind.Utc));

    [Fact] // F-M-12/13: drena todas las capturas pendientes y las saca de la cola
    public async Task Sube_todas_las_capturas_y_vacia_la_cola()
    {
        var cola = new ColaFake();
        await cola.EncolarAsync(Captura(1));
        await cola.EncolarAsync(Captura(2));
        var backend = new BackendFake();
        var motor = new MotorCapturas(cola, backend);

        var r = await motor.SincronizarAsync();

        r.Subidas.Should().HaveCount(2);
        backend.Subidas.Should().HaveCount(2);
        (await cola.PendientesAsync()).Should().Be(0);
    }

    [Fact] // CU-07: las capturas se suben en orden por marca temporal
    public async Task Sube_en_orden_por_momento()
    {
        var cola = new ColaFake();
        var tarde = Captura(5);
        var temprano = Captura(1);
        await cola.EncolarAsync(tarde);
        await cola.EncolarAsync(temprano);
        var backend = new BackendFake();

        await new MotorCapturas(cola, backend).SincronizarAsync();

        backend.Subidas[0].Should().Be(temprano.CapturaId);
        backend.Subidas[1].Should().Be(tarde.CapturaId);
    }

    [Fact] // CU-07 §5.A: un corte de conexión conserva lo no subido y marca lo ya subido
    public async Task Corte_de_conexion_conserva_lo_pendiente_y_lanza()
    {
        var cola = new ColaFake();
        var primera = Captura(1);
        var segunda = Captura(2);
        await cola.EncolarAsync(primera);
        await cola.EncolarAsync(segunda);
        var backend = new BackendFake
        {
            Responder = c => c.CapturaId == primera.CapturaId
                ? true
                : throw new HttpRequestException("conexión perdida"),
        };
        var motor = new MotorCapturas(cola, backend);

        var acto = async () => await motor.SincronizarAsync();

        await acto.Should().ThrowAsync<SyncInterruptedException>();
        var pendientes = await cola.LeerPendientesAsync(10);
        pendientes.Should().ContainSingle().Which.CapturaId.Should().Be(segunda.CapturaId);
    }

    [Fact] // El backend rechaza una captura (4xx): queda en la cola y no se cicla
    public async Task Captura_rechazada_queda_en_la_cola()
    {
        var cola = new ColaFake();
        await cola.EncolarAsync(Captura(1));
        var backend = new BackendFake { Responder = _ => false };
        var motor = new MotorCapturas(cola, backend);

        var r = await motor.SincronizarAsync();

        r.Subidas.Should().BeEmpty();
        (await cola.PendientesAsync()).Should().Be(1);
    }
}

/// <summary>Cola de capturas sobre SQLite: conserva la foto (BLOB) y la coordenada, idempotente por CapturaId.</summary>
public sealed class ColaCapturasSqliteTests : IDisposable
{
    private readonly string _archivo = Path.Combine(Path.GetTempPath(), $"geovial-capturas-{Guid.NewGuid():N}.db");

    private ColaCapturasSqlite Cola() => new($"Data Source={_archivo}");

    [Fact] // US-16: encolar conserva la foto y la coordenada; leer las devuelve intactas
    public async Task Encolar_y_leer_conserva_la_foto_y_la_coordenada()
    {
        var cola = Cola();
        var foto = new byte[] { 9, 8, 7, 6, 5 };
        var captura = new CapturaPendiente(
            Guid.NewGuid(), Guid.NewGuid(), "obra.jpg", -34.61m, -58.42m, foto, new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc));

        await cola.EncolarAsync(captura);
        var pendientes = await cola.LeerPendientesAsync(10);

        pendientes.Should().ContainSingle();
        var leida = pendientes[0];
        leida.CapturaId.Should().Be(captura.CapturaId);
        leida.ReferenciaArchivo.Should().Be("obra.jpg");
        leida.LatitudExif.Should().Be(-34.61m);
        leida.LongitudExif.Should().Be(-58.42m);
        leida.Foto.Should().Equal(foto);
    }

    [Fact] // US-16: sin coordenada EXIF, la captura igual se encola (irá a la bandeja al subir)
    public async Task Encola_captura_sin_coordenada()
    {
        var cola = Cola();
        var captura = new CapturaPendiente(
            Guid.NewGuid(), Guid.NewGuid(), "s.jpg", null, null, new byte[] { 1 }, DateTime.UtcNow);

        await cola.EncolarAsync(captura);
        var leida = (await cola.LeerPendientesAsync(10)).Single();

        leida.LatitudExif.Should().BeNull();
        leida.LongitudExif.Should().BeNull();
    }

    [Fact] // idempotencia local: re-encolar la misma CapturaId no la duplica
    public async Task Reencolar_misma_captura_no_duplica()
    {
        var cola = Cola();
        var captura = new CapturaPendiente(Guid.NewGuid(), Guid.NewGuid(), "f.jpg", null, null, new byte[] { 1 }, DateTime.UtcNow);

        await cola.EncolarAsync(captura);
        await cola.EncolarAsync(captura);

        (await cola.PendientesAsync()).Should().Be(1);
    }

    [Fact] // marcar subida saca la captura de la cola
    public async Task Marcar_subida_vacia_la_cola()
    {
        var cola = Cola();
        var captura = new CapturaPendiente(Guid.NewGuid(), Guid.NewGuid(), "f.jpg", null, null, new byte[] { 1 }, DateTime.UtcNow);
        await cola.EncolarAsync(captura);

        await cola.MarcarSubidasAsync(new[] { captura.CapturaId });

        (await cola.PendientesAsync()).Should().Be(0);
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
