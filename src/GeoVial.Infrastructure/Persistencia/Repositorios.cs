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

public sealed class MarcadorRepository : IMarcadorRepository
{
    private readonly GeoVialDbContext _db;

    public MarcadorRepository(GeoVialDbContext db) => _db = db;

    public Task<Marcador?> ObtenerPorIdAsync(Guid marcadorId, CancellationToken ct = default) =>
        _db.Marcadores.AsNoTracking().FirstOrDefaultAsync(m => m.MarcadorId == marcadorId, ct);

    public async Task<IReadOnlyList<Marcador>> ListarPorRelevamientoAsync(Guid relevamientoId, CancellationToken ct = default) =>
        await _db.Marcadores.AsNoTracking().Where(m => m.RelevamientoId == relevamientoId).ToListAsync(ct);

    public async Task AgregarAsync(Marcador marcador, CancellationToken ct = default) =>
        await _db.Marcadores.AddAsync(marcador, ct);

    public Task<Marcador?> ObtenerParaEdicionAsync(Guid marcadorId, CancellationToken ct = default) =>
        _db.Marcadores.FirstOrDefaultAsync(m => m.MarcadorId == marcadorId, ct);

    public Task EliminarAsync(Marcador marcador, CancellationToken ct = default)
    {
        _db.Marcadores.Remove(marcador);
        return Task.CompletedTask;
    }

    public Task GuardarCambiosAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}

public sealed class ObservacionRepository : IObservacionRepository
{
    private readonly GeoVialDbContext _db;

    public ObservacionRepository(GeoVialDbContext db) => _db = db;

    public Task<Observacion?> ObtenerPorIdAsync(Guid observacionId, CancellationToken ct = default) =>
        _db.Observaciones.FirstOrDefaultAsync(o => o.ObservacionId == observacionId, ct);

    public async Task<IReadOnlyList<Observacion>> ListarPorRelevamientoAsync(Guid relevamientoId, CancellationToken ct = default) =>
        await _db.Observaciones.AsNoTracking().Where(o => o.RelevamientoId == relevamientoId).ToListAsync(ct);

    public async Task<IReadOnlyList<Observacion>> ListarPorMarcadorParaEdicionAsync(Guid marcadorId, CancellationToken ct = default) =>
        await _db.Observaciones.Where(o => o.MarcadorId == marcadorId).ToListAsync(ct);

    public async Task AgregarAsync(Observacion observacion, CancellationToken ct = default) =>
        await _db.Observaciones.AddAsync(observacion, ct);

    public Task GuardarCambiosAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}

public sealed class FotoRepository : IFotoRepository
{
    private readonly GeoVialDbContext _db;

    public FotoRepository(GeoVialDbContext db) => _db = db;

    public Task<Foto?> ObtenerPorObservacionAsync(Guid observacionId, CancellationToken ct = default) =>
        _db.Fotos.FirstOrDefaultAsync(f => f.ObservacionId == observacionId, ct);

    public Task<Foto?> ObtenerPorIdAsync(Guid fotoId, CancellationToken ct = default) =>
        _db.Fotos.AsNoTracking().FirstOrDefaultAsync(f => f.FotoId == fotoId, ct);

    public async Task<IReadOnlyList<Foto>> ListarPorMarcadorAsync(Guid marcadorId, CancellationToken ct = default) =>
        await _db.Fotos.AsNoTracking().Where(f => f.MarcadorId == marcadorId).ToListAsync(ct);

    public async Task<IReadOnlyList<Foto>> ListarPorMarcadorParaEdicionAsync(Guid marcadorId, CancellationToken ct = default) =>
        await _db.Fotos.Where(f => f.MarcadorId == marcadorId).ToListAsync(ct);

    public async Task AgregarAsync(Foto foto, CancellationToken ct = default) =>
        await _db.Fotos.AddAsync(foto, ct);
}

public sealed class CredencialRepository : ICredencialRepository
{
    private readonly GeoVialDbContext _db;

    public CredencialRepository(GeoVialDbContext db) => _db = db;

    public Task<Credencial?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default) =>
        _db.Credenciales.AsNoTracking().FirstOrDefaultAsync(c => c.NombreUsuario == nombreUsuario, ct);

    public Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default) =>
        _db.Credenciales.AnyAsync(c => c.NombreUsuario == nombreUsuario, ct);

    public Task<Credencial?> ObtenerPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default) =>
        _db.Credenciales.AsNoTracking().FirstOrDefaultAsync(c => c.UsuarioId == usuarioId, ct);

    public async Task AgregarAsync(Credencial credencial, CancellationToken ct = default) =>
        await _db.Credenciales.AddAsync(credencial, ct);

    public Task GuardarCambiosAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}

public sealed class ComentarioRepository : IComentarioRepository
{
    private readonly GeoVialDbContext _db;

    public ComentarioRepository(GeoVialDbContext db) => _db = db;

    public Task<Comentario?> ObtenerPorIdAsync(Guid comentarioId, CancellationToken ct = default) =>
        _db.Comentarios.FirstOrDefaultAsync(c => c.ComentarioId == comentarioId, ct);

    public async Task<IReadOnlyList<Comentario>> ListarPorMarcadorAsync(Guid marcadorId, CancellationToken ct = default) =>
        await _db.Comentarios.AsNoTracking().Where(c => c.MarcadorId == marcadorId).ToListAsync(ct);

    public async Task<IReadOnlyList<Comentario>> ListarPorMarcadorParaEdicionAsync(Guid marcadorId, CancellationToken ct = default) =>
        await _db.Comentarios.Where(c => c.MarcadorId == marcadorId).ToListAsync(ct);

    public async Task AgregarAsync(Comentario comentario, CancellationToken ct = default) =>
        await _db.Comentarios.AddAsync(comentario, ct);

    public Task GuardarCambiosAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}

public sealed class ConflictoRepository : IConflictoRepository
{
    private readonly GeoVialDbContext _db;

    public ConflictoRepository(GeoVialDbContext db) => _db = db;

    public Task<ConflictoSync?> ObtenerPorIdAsync(Guid conflictoSyncId, CancellationToken ct = default) =>
        _db.ConflictosSync.FirstOrDefaultAsync(c => c.ConflictoSyncId == conflictoSyncId, ct);

    public async Task<IReadOnlyList<ConflictoSync>> ListarPendientesPorRelevamientoAsync(Guid relevamientoId, CancellationToken ct = default) =>
        await _db.ConflictosSync.AsNoTracking()
            .Where(c => c.RelevamientoId == relevamientoId && c.EstadoResolucion == EstadoResolucionConflicto.Pendiente)
            .ToListAsync(ct);

    public Task<bool> ExistePendienteAsync(Guid relevamientoId, string recursosInvolucrados, CancellationToken ct = default) =>
        _db.ConflictosSync.AnyAsync(
            c => c.RelevamientoId == relevamientoId
                 && c.RecursosInvolucrados == recursosInvolucrados
                 && c.EstadoResolucion == EstadoResolucionConflicto.Pendiente,
            ct);

    public async Task AgregarAsync(ConflictoSync conflicto, CancellationToken ct = default) =>
        await _db.ConflictosSync.AddAsync(conflicto, ct);

    public Task GuardarCambiosAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}

public sealed class EtiquetaRepository : IEtiquetaRepository
{
    private readonly GeoVialDbContext _db;

    public EtiquetaRepository(GeoVialDbContext db) => _db = db;

    public Task<Etiqueta?> ObtenerPorNombreAsync(string nombre, CancellationToken ct = default) =>
        _db.Etiquetas.FirstOrDefaultAsync(e => e.Nombre == nombre, ct);

    public async Task AgregarAsync(Etiqueta etiqueta, CancellationToken ct = default) =>
        await _db.Etiquetas.AddAsync(etiqueta, ct);

    public async Task AgregarFotoEtiquetaAsync(FotoEtiqueta union, CancellationToken ct = default) =>
        await _db.FotoEtiquetas.AddAsync(union, ct);

    public async Task AgregarComentarioEtiquetaAsync(ComentarioEtiqueta union, CancellationToken ct = default) =>
        await _db.ComentarioEtiquetas.AddAsync(union, ct);

    public Task<bool> ExisteFotoEtiquetaAsync(Guid fotoId, Guid etiquetaId, CancellationToken ct = default) =>
        _db.FotoEtiquetas.AnyAsync(u => u.FotoId == fotoId && u.EtiquetaId == etiquetaId, ct);

    public Task<bool> ExisteComentarioEtiquetaAsync(Guid comentarioId, Guid etiquetaId, CancellationToken ct = default) =>
        _db.ComentarioEtiquetas.AnyAsync(u => u.ComentarioId == comentarioId && u.EtiquetaId == etiquetaId, ct);

    public async Task<IReadOnlyList<string>> ListarNombresDeFotoAsync(Guid fotoId, CancellationToken ct = default) =>
        await _db.FotoEtiquetas.Where(u => u.FotoId == fotoId)
            .Join(_db.Etiquetas, u => u.EtiquetaId, e => e.EtiquetaId, (u, e) => e.Nombre)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<string>> ListarNombresDeComentarioAsync(Guid comentarioId, CancellationToken ct = default) =>
        await _db.ComentarioEtiquetas.Where(u => u.ComentarioId == comentarioId)
            .Join(_db.Etiquetas, u => u.EtiquetaId, e => e.EtiquetaId, (u, e) => e.Nombre)
            .ToListAsync(ct);

    public Task GuardarCambiosAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
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
