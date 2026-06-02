using Microsoft.Data.Sqlite;

namespace GeoVial.Sync;

/// <summary>
/// Cola local de cambios sobre SQLite (US-17, CU-06; ADR-05). Persiste los cambios encolados sin conexión
/// y los conserva hasta que se confirman como sincronizados. La idempotencia se garantiza con la clave
/// primaria <c>ChangeId</c>: re-encolar el mismo cambio no lo duplica.
/// </summary>
public sealed class ColaCambiosSqlite : IChangeQueue
{
    private readonly string _cadenaConexion;

    public ColaCambiosSqlite(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
        InicializarEsquema();
    }

    private void InicializarEsquema()
    {
        using var conexion = Abrir();
        using var comando = conexion.CreateCommand();
        comando.CommandText =
            """
            CREATE TABLE IF NOT EXISTS CambioPendiente (
                ChangeId TEXT PRIMARY KEY,
                OperationType INTEGER NOT NULL,
                Entity TEXT NOT NULL,
                EntityRef TEXT NOT NULL,
                Timestamp TEXT NOT NULL,
                Payload TEXT NOT NULL
            );
            """;
        comando.ExecuteNonQuery();
    }

    public async Task EnqueueAsync(ChangeRecord change, CancellationToken ct = default)
    {
        await using var conexion = Abrir();
        await using var comando = conexion.CreateCommand();
        // INSERT OR IGNORE: el ChangeId repetido no se encola dos veces (RC-03).
        comando.CommandText =
            """
            INSERT OR IGNORE INTO CambioPendiente (ChangeId, OperationType, Entity, EntityRef, Timestamp, Payload)
            VALUES ($id, $op, $entity, $ref, $ts, $payload);
            """;
        comando.Parameters.AddWithValue("$id", change.ChangeId.ToString());
        comando.Parameters.AddWithValue("$op", (int)change.OperationType);
        comando.Parameters.AddWithValue("$entity", change.Entity);
        comando.Parameters.AddWithValue("$ref", change.EntityRef.ToString());
        comando.Parameters.AddWithValue("$ts", change.Timestamp.ToString("O"));
        comando.Parameters.AddWithValue("$payload", change.Payload);
        try
        {
            await comando.ExecuteNonQueryAsync(ct);
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == SqliteFull)
        {
            // US-16 CA-03: disco lleno; se conserva lo ya guardado y se informa con el error tipado.
            throw new AlmacenamientoLocalInsuficienteException("No hay espacio de almacenamiento local para encolar el cambio.", ex);
        }
    }

    // SQLITE_FULL: el disco/base local se quedó sin espacio.
    private const int SqliteFull = 13;

    public async Task<IReadOnlyList<ChangeRecord>> ReadPendingAsync(int max, CancellationToken ct = default)
    {
        await using var conexion = Abrir();
        await using var comando = conexion.CreateCommand();
        comando.CommandText =
            """
            SELECT ChangeId, OperationType, Entity, EntityRef, Timestamp, Payload
            FROM CambioPendiente
            ORDER BY Timestamp ASC
            LIMIT $max;
            """;
        comando.Parameters.AddWithValue("$max", max);

        var pendientes = new List<ChangeRecord>();
        await using var lector = await comando.ExecuteReaderAsync(ct);
        while (await lector.ReadAsync(ct))
        {
            pendientes.Add(new ChangeRecord(
                Guid.Parse(lector.GetString(0)),
                (OperationType)lector.GetInt32(1),
                lector.GetString(2),
                Guid.Parse(lector.GetString(3)),
                DateTime.Parse(lector.GetString(4), null, System.Globalization.DateTimeStyles.RoundtripKind),
                lector.GetString(5)));
        }

        return pendientes;
    }

    public async Task MarkConfirmedAsync(IEnumerable<Guid> changeIds, CancellationToken ct = default)
    {
        var ids = changeIds.ToList();
        if (ids.Count == 0)
        {
            return;
        }

        await using var conexion = Abrir();
        await using var transaccion = conexion.BeginTransaction();
        foreach (var id in ids)
        {
            await using var comando = conexion.CreateCommand();
            comando.Transaction = transaccion;
            comando.CommandText = "DELETE FROM CambioPendiente WHERE ChangeId = $id;";
            comando.Parameters.AddWithValue("$id", id.ToString());
            await comando.ExecuteNonQueryAsync(ct);
        }

        await transaccion.CommitAsync(ct);
    }

    public async Task<int> PendingCountAsync(CancellationToken ct = default)
    {
        await using var conexion = Abrir();
        await using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT COUNT(*) FROM CambioPendiente;";
        var resultado = await comando.ExecuteScalarAsync(ct);
        return Convert.ToInt32(resultado);
    }

    private SqliteConnection Abrir()
    {
        var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();
        return conexion;
    }
}
