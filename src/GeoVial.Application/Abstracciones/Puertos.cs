using GeoVial.Domain;

namespace GeoVial.Application.Abstracciones;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorIdAsync(Guid usuarioId, CancellationToken ct = default);
    Task<IReadOnlyList<Usuario>> ListarTodosAsync(CancellationToken ct = default);
    Task AgregarAsync(Usuario usuario, CancellationToken ct = default);
    Task GuardarCambiosAsync(CancellationToken ct = default);
}

public interface IAreaRepository
{
    Task<bool> ExisteAsync(Guid areaId, CancellationToken ct = default);
    Task<Area?> ObtenerPorIdAsync(Guid areaId, CancellationToken ct = default);
}

public interface IRelevamientoRepository
{
    Task<Relevamiento?> ObtenerPorIdAsync(Guid relevamientoId, CancellationToken ct = default);
    Task<IReadOnlyList<Relevamiento>> ListarTodosAsync(CancellationToken ct = default);
    Task AgregarAsync(Relevamiento relevamiento, CancellationToken ct = default);
    Task GuardarCambiosAsync(CancellationToken ct = default);
}

public interface IMarcadorRepository
{
    Task<Marcador?> ObtenerPorIdAsync(Guid marcadorId, CancellationToken ct = default);
    Task<IReadOnlyList<Marcador>> ListarPorRelevamientoAsync(Guid relevamientoId, CancellationToken ct = default);
    Task AgregarAsync(Marcador marcador, CancellationToken ct = default);
}

public interface IObservacionRepository
{
    Task<Observacion?> ObtenerPorIdAsync(Guid observacionId, CancellationToken ct = default);
    Task<IReadOnlyList<Observacion>> ListarPorRelevamientoAsync(Guid relevamientoId, CancellationToken ct = default);
    Task AgregarAsync(Observacion observacion, CancellationToken ct = default);
    Task GuardarCambiosAsync(CancellationToken ct = default);
}

public interface IFotoRepository
{
    Task<Foto?> ObtenerPorObservacionAsync(Guid observacionId, CancellationToken ct = default);
    Task<Foto?> ObtenerPorIdAsync(Guid fotoId, CancellationToken ct = default);
    Task<IReadOnlyList<Foto>> ListarPorMarcadorAsync(Guid marcadorId, CancellationToken ct = default);
    Task AgregarAsync(Foto foto, CancellationToken ct = default);
}

public interface IComentarioRepository
{
    Task<Comentario?> ObtenerPorIdAsync(Guid comentarioId, CancellationToken ct = default);
    Task<IReadOnlyList<Comentario>> ListarPorMarcadorAsync(Guid marcadorId, CancellationToken ct = default);
    Task AgregarAsync(Comentario comentario, CancellationToken ct = default);
    Task GuardarCambiosAsync(CancellationToken ct = default);
}

public interface IEtiquetaRepository
{
    Task<Etiqueta?> ObtenerPorNombreAsync(string nombre, CancellationToken ct = default);
    Task AgregarAsync(Etiqueta etiqueta, CancellationToken ct = default);
    Task AgregarFotoEtiquetaAsync(FotoEtiqueta union, CancellationToken ct = default);
    Task AgregarComentarioEtiquetaAsync(ComentarioEtiqueta union, CancellationToken ct = default);
    Task<bool> ExisteFotoEtiquetaAsync(Guid fotoId, Guid etiquetaId, CancellationToken ct = default);
    Task<bool> ExisteComentarioEtiquetaAsync(Guid comentarioId, Guid etiquetaId, CancellationToken ct = default);
    Task<IReadOnlyList<string>> ListarNombresDeFotoAsync(Guid fotoId, CancellationToken ct = default);
    Task<IReadOnlyList<string>> ListarNombresDeComentarioAsync(Guid comentarioId, CancellationToken ct = default);
    Task GuardarCambiosAsync(CancellationToken ct = default);
}

public interface ICredencialRepository
{
    Task<Credencial?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default);
    Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default);
    Task<Credencial?> ObtenerPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
    Task AgregarAsync(Credencial credencial, CancellationToken ct = default);
    Task GuardarCambiosAsync(CancellationToken ct = default);
}

/// <summary>
/// Registra una acción en la auditoría inmutable (RN-07). Devuelve true si el asiento se persistió;
/// false dispara <see cref="CodigosError.AccionNoAuditada"/> para no dejar acciones sin trazabilidad.
/// </summary>
public interface IServicioAuditoria
{
    Task<bool> RegistrarAsync(Guid autorUsuarioId, string operacion, string recursoAfectado, CancellationToken ct = default);
}

public interface IHasherClave
{
    string Hash(string clave);
    bool Verificar(string clave, string hash);
}

public interface IServicioToken
{
    string GenerarAccessToken(Usuario usuario, out int expiraEnSegundos);
    string GenerarRefreshToken();
}

public interface IRelojUtc
{
    DateTime AhoraUtc { get; }
}
