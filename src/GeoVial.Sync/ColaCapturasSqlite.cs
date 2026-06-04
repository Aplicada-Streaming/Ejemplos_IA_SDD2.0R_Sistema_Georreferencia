using Microsoft.Data.Sqlite;

namespace GeoVial.Sync;

/// <summary>
/// Cola local de capturas de campo sobre SQLite (US-16, US-17, CU-06; ADR-05). Persiste la foto (BLOB) y
/// la coordenada de cada captura tomada sin conexión y las conserva hasta subirlas. La idempotencia local
/// se garantiza con la clave primaria <c>CapturaId</c>: re-encolar la misma captura no la duplica.
/// </summary>
public sealed class ColaCapturasSqlite : IColaCapturas
{
    // SQLITE_FULL: el disco/base local se quedó sin espacio.
    private const int SqliteFull = 13;

    private readonly string _cadenaConexion;

    public ColaCapturasSqlite(string cadenaConexion)
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
            CREATE TABLE IF NOT EXISTS CapturaPendiente (
                CapturaId TEXT PRIMARY KEY,
                RelevamientoId TEXT NOT NULL,
                ReferenciaArchivo TEXT NOT NULL,
                LatitudExif TEXT NULL,
                LongitudExif TEXT NULL,
                Foto BLOB NOT NULL,
                Momento TEXT NOT NULL
            );
            """;
        comando.ExecuteNonQuery();
    }

    public async Task EncolarAsync(CapturaPendiente captura, CancellationToken ct = default)
    {
        await using var conexion = Abrir();
        await using var comando = conexion.CreateCommand();
        // INSERT OR IGNORE: la CapturaId repetida no se encola dos veces (idempotencia local).
        comando.CommandText =
            """
            INSERT OR IGNORE INTO CapturaPendiente
                (CapturaId, RelevamientoId, ReferenciaArchivo, LatitudExif, LongitudExif, Foto, Momento)
            VALUES ($id, $rel, $ref, $lat, $lon, $foto, $ts);
            """;
        comando.Parameters.AddWithValue("$id", captura.CapturaId.ToString());
        comando.Parameters.AddWithValue("$rel", captura.RelevamientoId.ToString());
        comando.Parameters.AddWithValue("$ref", captura.ReferenciaArchivo);
        comando.Parameters.AddWithValue("$lat", (object?)captura.LatitudExif?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? DBNull.Value);
        comando.Parameters.AddWithValue("$lon", (object?)captura.LongitudExif?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? DBNull.Value);
        comando.Parameters.AddWithValue("$foto", captura.Foto);
        comando.Parameters.AddWithValue("$ts", captura.Momento.ToString("O"));
        try
        {
            await comando.ExecuteNonQueryAsync(ct);
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == SqliteFull)
        {
            // US-16 CA-03: disco lleno; se conserva lo ya guardado y se informa con el error tipado.
            throw new AlmacenamientoLocalInsuficienteException("No hay espacio de almacenamiento local para encolar la captura.", ex);
        }
    }

    public async Task<IReadOnlyList<CapturaPendiente>> LeerPendientesAsync(int max, CancellationToken ct = default)
    {
        await using var conexion = Abrir();
        await using var comando = conexion.CreateCommand();
        comando.CommandText =
            """
            SELECT CapturaId, RelevamientoId, ReferenciaArchivo, LatitudExif, LongitudExif, Foto, Momento
            FROM CapturaPendiente
            ORDER BY Momento ASC
            LIMIT $max;
            """;
        comando.Parameters.AddWithValue("$max", max);

        var pendientes = new List<CapturaPendiente>();
        await using var lector = await comando.ExecuteReaderAsync(ct);
        while (await lector.ReadAsync(ct))
        {
            pendientes.Add(new CapturaPendiente(
                Guid.Parse(lector.GetString(0)),
                Guid.Parse(lector.GetString(1)),
                lector.GetString(2),
                lector.IsDBNull(3) ? null : decimal.Parse(lector.GetString(3), System.Globalization.CultureInfo.InvariantCulture),
                lector.IsDBNull(4) ? null : decimal.Parse(lector.GetString(4), System.Globalization.CultureInfo.InvariantCulture),
                (byte[])lector[5],
                DateTime.Parse(lector.GetString(6), null, System.Globalization.DateTimeStyles.RoundtripKind)));
        }

        return pendientes;
    }

    public async Task MarcarSubidasAsync(IEnumerable<Guid> capturaIds, CancellationToken ct = default)
    {
        var ids = capturaIds.ToList();
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
            comando.CommandText = "DELETE FROM CapturaPendiente WHERE CapturaId = $id;";
            comando.Parameters.AddWithValue("$id", id.ToString());
            await comando.ExecuteNonQueryAsync(ct);
        }

        await transaccion.CommitAsync(ct);
    }

    public async Task<int> PendientesAsync(CancellationToken ct = default)
    {
        await using var conexion = Abrir();
        await using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT COUNT(*) FROM CapturaPendiente;";
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
