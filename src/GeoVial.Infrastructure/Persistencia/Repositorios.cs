using GeoVial.Application.Abstracciones;
using GeoVial.Domain;
using Microsoft.EntityFrameworkCore;

namespace GeoVial.Infrastructure.Persistencia;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly GeoVialDbContext _db;

    public UsuarioRepository(GeoVialDbContext db) => _db = db;

    public Task<Usuario?> ObtenerPorIdAsync(Guid usuarioId, CancellationToken ct = default) =>
        _db.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == usuarioId, ct);

    public async Task<IReadOnlyList<Usuario>> ListarTodosAsync(CancellationToken ct = default) =>
        await _db.Usuarios.AsNoTracking().ToListAsync(ct);

    public async Task AgregarAsync(Usuario usuario, CancellationToken ct = default) =>
        await _db.Usuarios.AddAsync(usuario, ct);

    public Task GuardarCambiosAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}

public sealed class AreaRepository : IAreaRepository
{
    private readonly GeoVialDbContext _db;

    public AreaRepository(GeoVialDbContext db) => _db = db;

    public Task<bool> ExisteAsync(Guid areaId, CancellationToken ct = default) =>
        _db.Areas.AnyAsync(a => a.AreaId == areaId, ct);

    public Task<Area?> ObtenerPorIdAsync(Guid areaId, CancellationToken ct = default) =>
        _db.Areas.FirstOrDefaultAsync(a => a.AreaId == areaId, ct);
}

public sealed class RelevamientoRepository : IRelevamientoRepository
{
    private readonly GeoVialDbContext _db;

    public RelevamientoRepository(GeoVialDbContext db) => _db = db;

    public Task<Relevamiento?> ObtenerPorIdAsync(Guid relevamientoId, CancellationToken ct = default) =>
        _db.Relevamientos.Include(r => r.Asignaciones).FirstOrDefaultAsync(r => r.RelevamientoId == relevamientoId, ct);

    public async Task<IReadOnlyList<Relevamiento>> ListarTodosAsync(CancellationToken ct = default) =>
        await _db.Relevamientos.Include(r => r.Asignaciones).AsNoTracking().ToListAsync(ct);

    public async Task AgregarAsync(Relevamiento relevamiento, CancellationToken ct = default) =>
        await _db.Relevamientos.AddAsync(relevamiento, ct);

    public Task GuardarCambiosAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}

public sealed class CredencialRepository : ICredencialRepository
{
    private readonly GeoVialDbContext _db;

    public CredencialRepository(GeoVialDbContext db) => _db = db;

    public Task<Credencial?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default) =>
        _db.Credenciales.AsNoTracking().FirstOrDefaultAsync(c => c.NombreUsuario == nombreUsuario, ct);
}

/// <summary>
/// Persiste el asiento de auditoría inmutable (RN-07). Devuelve false ante cualquier fallo de
/// persistencia para que el llamador rechace la acción con ACCION_NO_AUDITADA (CU-03).
/// </summary>
public sealed class ServicioAuditoria : IServicioAuditoria
{
    private readonly GeoVialDbContext _db;
    private readonly IRelojUtc _reloj;

    public ServicioAuditoria(GeoVialDbContext db, IRelojUtc reloj)
    {
        _db = db;
        _reloj = reloj;
    }

    public async Task<bool> RegistrarAsync(Guid autorUsuarioId, string operacion, string recursoAfectado, CancellationToken ct = default)
    {
        try
        {
            var registro = new RegistroAuditoria(autorUsuarioId, _reloj.AhoraUtc, operacion, recursoAfectado);
            await _db.RegistrosAuditoria.AddAsync(registro, ct);
            await _db.SaveChangesAsync(ct);
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }
}
