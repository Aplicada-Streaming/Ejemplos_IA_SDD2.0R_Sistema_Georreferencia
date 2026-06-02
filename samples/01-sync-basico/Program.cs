using GeoVial.Sync;
using Microsoft.Data.Sqlite;

// Demo básica de consumo de la librería publicada GeoVial.Sync (guía de publicación §3): ejercita la
// superficie pública (IChangeQueue + ISyncEngine) contra un backend en memoria, mostrando la cola pasar
// de pendiente a sincronizado. Es el consumidor de verificación de que la API es consumible.

Console.WriteLine("GeoVial.Sync — demo básica de sincronización");

var ruta = Path.Combine(Path.GetTempPath(), $"sync-basico-{Guid.NewGuid():N}.db");
IChangeQueue cola = new ColaCambiosSqlite($"Data Source={ruta}");
ISyncBackendClient backend = new BackendEnMemoria();
ISyncEngine motor = new MotorSincronizacion(cola, backend);
var relevamiento = Guid.NewGuid();

for (var i = 1; i <= 3; i++)
{
    var observacion = new ObservacionCapturada(
        Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), $"Observación {i}", DateTime.UtcNow);
    await cola.EnqueueAsync(ChangeRecordFactory.Comentario(observacion));
}

Console.WriteLine($"Pendientes antes de sincronizar: {await cola.PendingCountAsync()}");

var resultado = await motor.SynchronizeAsync(relevamiento);

Console.WriteLine($"Confirmados: {resultado.Confirmed.Count} · Conflictos: {resultado.Conflicts.Count}");
Console.WriteLine($"Pendientes después de sincronizar: {await cola.PendingCountAsync()}");

SqliteConnection.ClearAllPools(); // libera el archivo de la cola antes de borrarlo
File.Delete(ruta);

/// <summary>Backend de sincronización en memoria mínimo: confirma todos los cambios subidos.</summary>
internal sealed class BackendEnMemoria : ISyncBackendClient
{
    public Task<SyncResult> UploadAsync(Guid relevamientoId, IReadOnlyList<ChangeRecord> changes, DateTime? since, CancellationToken ct = default) =>
        Task.FromResult(new SyncResult(
            changes.Select(c => c.ChangeId).ToList(),
            Array.Empty<ConflictInfo>(),
            Array.Empty<string>()));
}
