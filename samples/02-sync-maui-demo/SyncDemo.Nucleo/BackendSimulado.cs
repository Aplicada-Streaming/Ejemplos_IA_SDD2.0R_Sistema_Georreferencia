using GeoVial.Sync;

namespace GeoVial.SyncDemo;

/// <summary>
/// Backend de sincronización simulado en memoria (US-32): implementa el mismo contrato <see cref="ISyncBackendClient"/>
/// que el cliente REST real, de modo que un integrador externo evalúe la librería sin depender de GeoVial.
/// Consolida los cambios subidos; para un conjunto configurable de recursos en conflicto devuelve un
/// <see cref="ConflictInfo"/> sin confirmar el cambio (queda pendiente hasta resolver, RN-04). A partir de la
/// segunda sincronización (<c>since != null</c>) ofrece las actualizaciones acumuladas, demostrando la bajada
/// de cambios sin subir nada (US-19 CA-02).
/// </summary>
public sealed class BackendSimulado : ISyncBackendClient
{
    private readonly HashSet<Guid> _enConflicto;
    private readonly IReadOnlyList<string> _actualizaciones;

    public BackendSimulado(IEnumerable<Guid>? recursosEnConflicto = null, IEnumerable<string>? actualizaciones = null)
    {
        _enConflicto = recursosEnConflicto?.ToHashSet() ?? new HashSet<Guid>();
        _actualizaciones = actualizaciones?.ToList() ?? (IReadOnlyList<string>)Array.Empty<string>();
    }

    /// <summary>Recursos ya consolidados (aplicados) en el backend simulado.</summary>
    public HashSet<Guid> Consolidados { get; } = new();

    /// <summary>Cantidad de subidas recibidas (para verificar la reanudación sin duplicar).</summary>
    public int Subidas { get; private set; }

    /// <summary>
    /// Resuelve un conflicto a favor del cambio local (RN-04, última escritura): el recurso deja de chocar,
    /// de modo que la próxima subida del mismo cambio lo confirma.
    /// </summary>
    public void DespejarConflicto(Guid recurso) => _enConflicto.Remove(recurso);

    /// <summary>Indica si el recurso aún choca con el estado del backend.</summary>
    public bool EstaEnConflicto(Guid recurso) => _enConflicto.Contains(recurso);

    public Task<SyncResult> UploadAsync(Guid relevamientoId, IReadOnlyList<ChangeRecord> changes, DateTime? since, CancellationToken ct = default)
    {
        Subidas++;
        var confirmados = new List<Guid>();
        var conflictos = new List<ConflictInfo>();

        foreach (var cambio in changes)
        {
            if (_enConflicto.Contains(cambio.EntityRef))
            {
                // RN-04: el recurso choca con el estado del backend; se reporta y no se confirma hasta resolver.
                conflictos.Add(new ConflictInfo(
                    Guid.NewGuid(),
                    ConflictKind.FieldConflict,
                    new[] { $"{cambio.Entity}={cambio.EntityRef}" }));
                continue;
            }

            Consolidados.Add(cambio.EntityRef);
            confirmados.Add(cambio.ChangeId);
        }

        // since != null → no es la primera sincronización: el backend ofrece las actualizaciones acumuladas.
        var actualizaciones = since is null ? Array.Empty<string>() : _actualizaciones.ToArray();
        return Task.FromResult(new SyncResult(confirmados, conflictos, actualizaciones));
    }
}
