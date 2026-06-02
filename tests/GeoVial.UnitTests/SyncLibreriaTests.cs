using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using GeoVial.Sync;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Cliente REST de sincronización contra /sync (ClienteSyncHttp, BT-15).</summary>
public class ClienteSyncHttpTests
{
    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly string _respuesta;

        public StubHandler(string respuesta) => _respuesta = respuesta;

        public HttpRequestMessage? Capturada { get; private set; }
        public string? CuerpoEnviado { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Capturada = request;
            CuerpoEnviado = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_respuesta, Encoding.UTF8, "application/json"),
            };
        }
    }

    [Fact] // BT-15: el cliente postea al endpoint /sync y parsea la respuesta (confirmados/conflictos)
    public async Task Sube_al_endpoint_sync_y_parsea_respuesta()
    {
        var changeId = Guid.NewGuid();
        var web = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var payload = JsonSerializer.Serialize(
            new { marcadorId = Guid.NewGuid(), fotoId = (Guid?)null, autorUsuarioId = Guid.NewGuid(), texto = "fisura" }, web);
        var cambio = new ChangeRecord(changeId, OperationType.Create, "comentario", Guid.NewGuid(), new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc), payload);

        var respuesta = JsonSerializer.Serialize(new
        {
            confirmados = new[] { changeId },
            conflictos = new[] { new { conflictoSyncId = Guid.NewGuid(), tipo = 2, recursosInvolucrados = "a;b" } },
            actualizaciones = Array.Empty<object>(),
        }, web);

        var handler = new StubHandler(respuesta);
        var cliente = new ClienteSyncHttp(new HttpClient(handler) { BaseAddress = new Uri("https://geovial/") });

        var relevamientoId = Guid.NewGuid();
        var r = await cliente.UploadAsync(relevamientoId, new[] { cambio }, null);

        r.Confirmed.Should().ContainSingle().Which.Should().Be(changeId);
        r.Conflicts.Should().ContainSingle().Which.Kind.Should().Be(ConflictKind.MarkersWithinRadius);
        handler.Capturada!.Method.Should().Be(HttpMethod.Post);
        handler.Capturada.RequestUri!.AbsolutePath.Should().Be($"/api/v1/relevamientos/{relevamientoId}/sync");
        handler.CuerpoEnviado.Should().Contain("fisura"); // el texto del payload viajó al backend
    }
}

/// <summary>Cola local de cambios sobre SQLite (US-17, BT-15).</summary>
public sealed class ColaCambiosSqliteTests : IDisposable
{
    private readonly string _archivo = Path.Combine(Path.GetTempPath(), $"geovial-cola-{Guid.NewGuid():N}.db");

    private ColaCambiosSqlite Cola() => new($"Data Source={_archivo}");

    private static ChangeRecord Cambio(Guid id, DateTime ts, string texto = "x") =>
        new(id, OperationType.Create, "comentario", Guid.NewGuid(), ts, $"{{\"texto\":\"{texto}\"}}");

    [Fact] // US-17: encolar y leer pendiente
    public async Task Encolar_y_leer_pendiente()
    {
        var cola = Cola();
        var c = Cambio(Guid.NewGuid(), new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc));

        await cola.EnqueueAsync(c);

        (await cola.PendingCountAsync()).Should().Be(1);
        (await cola.ReadPendingAsync(10)).Should().ContainSingle().Which.ChangeId.Should().Be(c.ChangeId);
    }

    [Fact] // RC-03: re-encolar el mismo ChangeId no duplica
    public async Task Encolar_mismo_id_no_duplica()
    {
        var cola = Cola();
        var c = Cambio(Guid.NewGuid(), new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc));

        await cola.EnqueueAsync(c);
        await cola.EnqueueAsync(c);

        (await cola.PendingCountAsync()).Should().Be(1);
    }

    [Fact] // CU-07 §4.2: los pendientes se leen ordenados por marca temporal
    public async Task Pendientes_ordenados_por_marca_temporal()
    {
        var cola = Cola();
        var t0 = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var nuevo = Cambio(Guid.NewGuid(), t0.AddMinutes(10), "nuevo");
        var viejo = Cambio(Guid.NewGuid(), t0, "viejo");
        await cola.EnqueueAsync(nuevo);
        await cola.EnqueueAsync(viejo);

        var pendientes = await cola.ReadPendingAsync(10);

        pendientes.Select(p => p.ChangeId).Should().ContainInOrder(viejo.ChangeId, nuevo.ChangeId);
    }

    [Fact] // confirmar vacía de la cola lo sincronizado
    public async Task Confirmar_vacia_lo_sincronizado()
    {
        var cola = Cola();
        var c1 = Cambio(Guid.NewGuid(), new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc));
        var c2 = Cambio(Guid.NewGuid(), new DateTime(2026, 6, 1, 11, 0, 0, DateTimeKind.Utc));
        await cola.EnqueueAsync(c1);
        await cola.EnqueueAsync(c2);

        await cola.MarkConfirmedAsync(new[] { c1.ChangeId });

        (await cola.PendingCountAsync()).Should().Be(1);
        (await cola.ReadPendingAsync(10)).Should().ContainSingle().Which.ChangeId.Should().Be(c2.ChangeId);
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

/// <summary>Motor de sincronización: subir, confirmar, reportar conflictos, reanudar (ISyncEngine, CU-07).</summary>
public class MotorSincronizacionTests
{
    private static readonly Guid Relevamiento = Guid.NewGuid();

    private sealed class FakeColaMemoria : IChangeQueue
    {
        private readonly List<ChangeRecord> _datos = new();

        public FakeColaMemoria(params ChangeRecord[] iniciales) => _datos.AddRange(iniciales);

        public Task EnqueueAsync(ChangeRecord change, CancellationToken ct = default)
        {
            if (_datos.All(c => c.ChangeId != change.ChangeId))
            {
                _datos.Add(change);
            }

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<ChangeRecord>> ReadPendingAsync(int max, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<ChangeRecord>>(_datos.OrderBy(c => c.Timestamp).Take(max).ToList());

        public Task MarkConfirmedAsync(IEnumerable<Guid> changeIds, CancellationToken ct = default)
        {
            var ids = changeIds.ToHashSet();
            _datos.RemoveAll(c => ids.Contains(c.ChangeId));
            return Task.CompletedTask;
        }

        public Task<int> PendingCountAsync(CancellationToken ct = default) => Task.FromResult(_datos.Count);
    }

    private sealed class FakeBackend : ISyncBackendClient
    {
        private readonly bool _falla;
        private readonly IReadOnlyList<ConflictInfo> _conflictos;

        public FakeBackend(bool falla = false, IReadOnlyList<ConflictInfo>? conflictos = null)
        {
            _falla = falla;
            _conflictos = conflictos ?? Array.Empty<ConflictInfo>();
        }

        public int Subidas { get; private set; }

        public Task<SyncResult> UploadAsync(Guid relevamientoId, IReadOnlyList<ChangeRecord> changes, DateTime? since, CancellationToken ct = default)
        {
            Subidas++;
            if (_falla)
            {
                throw new HttpRequestException("sin conexión");
            }

            // Confirma todos los cambios subidos.
            return Task.FromResult(new SyncResult(changes.Select(c => c.ChangeId).ToList(), _conflictos, Array.Empty<string>()));
        }
    }

    private static ChangeRecord Cambio(DateTime ts) =>
        new(Guid.NewGuid(), OperationType.Create, "comentario", Guid.NewGuid(), ts, "{}");

    [Fact] // CU-07 CA-01: sube los pendientes y los vacía de la cola
    public async Task Sincronizar_sube_y_vacia_la_cola()
    {
        var t0 = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var cola = new FakeColaMemoria(Cambio(t0), Cambio(t0.AddMinutes(1)));
        var motor = new MotorSincronizacion(cola, new FakeBackend());

        var r = await motor.SynchronizeAsync(Relevamiento);

        r.Confirmed.Should().HaveCount(2);
        (await cola.PendingCountAsync()).Should().Be(0);
    }

    [Fact] // los conflictos del backend se devuelven y se reportan
    public async Task Sincronizar_reporta_conflictos()
    {
        var conflicto = new ConflictInfo(Guid.NewGuid(), ConflictKind.FieldConflict, new[] { "comentario=x" });
        var reporter = new FakeReporter();
        var cola = new FakeColaMemoria(Cambio(new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc)));
        var motor = new MotorSincronizacion(cola, new FakeBackend(conflictos: new[] { conflicto }), reporter);

        var r = await motor.SynchronizeAsync(Relevamiento);

        r.Conflicts.Should().ContainSingle();
        reporter.Reportados.Should().ContainSingle();
    }

    [Fact] // CU-07 §5.A: ante un corte, lanza SyncInterrupted y conserva los pendientes
    public async Task Sincronizar_interrumpida_conserva_pendientes()
    {
        var cola = new FakeColaMemoria(Cambio(new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc)));
        var motor = new MotorSincronizacion(cola, new FakeBackend(falla: true));

        var accion = async () => await motor.SynchronizeAsync(Relevamiento);

        await accion.Should().ThrowAsync<SyncInterruptedException>();
        (await cola.PendingCountAsync()).Should().Be(1); // nada se vació
    }

    private sealed class FakeReporter : IConflictReporter
    {
        public List<ConflictInfo> Reportados { get; } = new();

        public Task ReportAsync(IReadOnlyList<ConflictInfo> conflicts, CancellationToken ct = default)
        {
            Reportados.AddRange(conflicts);
            return Task.CompletedTask;
        }
    }
}
