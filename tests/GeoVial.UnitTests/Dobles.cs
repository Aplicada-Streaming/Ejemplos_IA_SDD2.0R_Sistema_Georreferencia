using GeoVial.Application.Abstracciones;
using GeoVial.Domain;

namespace GeoVial.UnitTests;

internal sealed class FakeUsuarioRepository : IUsuarioRepository
{
    private readonly Dictionary<Guid, Usuario> _datos = new();

    public FakeUsuarioRepository(params Usuario[] iniciales)
    {
        foreach (var u in iniciales)
        {
            _datos[u.UsuarioId] = u;
        }
    }

    public Task<Usuario?> ObtenerPorIdAsync(Guid usuarioId, CancellationToken ct = default) =>
        Task.FromResult(_datos.GetValueOrDefault(usuarioId));

    public Task<IReadOnlyList<Usuario>> ListarTodosAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Usuario>>(_datos.Values.ToList());

    public Task AgregarAsync(Usuario usuario, CancellationToken ct = default)
    {
        _datos[usuario.UsuarioId] = usuario;
        return Task.CompletedTask;
    }

    public Task GuardarCambiosAsync(CancellationToken ct = default) => Task.CompletedTask;
}

internal sealed class FakeAreaRepository : IAreaRepository
{
    private readonly HashSet<Guid> _areas;

    public FakeAreaRepository(params Guid[] areas) => _areas = new HashSet<Guid>(areas);

    public Task<bool> ExisteAsync(Guid areaId, CancellationToken ct = default) => Task.FromResult(_areas.Contains(areaId));

    public Task<Area?> ObtenerPorIdAsync(Guid areaId, CancellationToken ct = default) => Task.FromResult<Area?>(null);
}

internal sealed class FakeRelevamientoRepository : IRelevamientoRepository
{
    private readonly Dictionary<Guid, Relevamiento> _datos = new();

    public FakeRelevamientoRepository(params Relevamiento[] iniciales)
    {
        foreach (var r in iniciales)
        {
            _datos[r.RelevamientoId] = r;
        }
    }

    public Task<Relevamiento?> ObtenerPorIdAsync(Guid relevamientoId, CancellationToken ct = default) =>
        Task.FromResult(_datos.GetValueOrDefault(relevamientoId));

    public Task<IReadOnlyList<Relevamiento>> ListarTodosAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Relevamiento>>(_datos.Values.ToList());

    public Task AgregarAsync(Relevamiento relevamiento, CancellationToken ct = default)
    {
        _datos[relevamiento.RelevamientoId] = relevamiento;
        return Task.CompletedTask;
    }

    public Task GuardarCambiosAsync(CancellationToken ct = default) => Task.CompletedTask;
}

internal sealed class FakeMarcadorRepository : IMarcadorRepository
{
    private readonly List<Marcador> _datos;

    public FakeMarcadorRepository(params Marcador[] iniciales) => _datos = iniciales.ToList();

    public Task<Marcador?> ObtenerPorIdAsync(Guid marcadorId, CancellationToken ct = default) =>
        Task.FromResult(_datos.FirstOrDefault(m => m.MarcadorId == marcadorId));

    public Task<IReadOnlyList<Marcador>> ListarPorRelevamientoAsync(Guid relevamientoId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Marcador>>(_datos.Where(m => m.RelevamientoId == relevamientoId).ToList());

    public Task AgregarAsync(Marcador marcador, CancellationToken ct = default)
    {
        _datos.Add(marcador);
        return Task.CompletedTask;
    }
}

internal sealed class FakeObservacionRepository : IObservacionRepository
{
    private readonly Dictionary<Guid, Observacion> _datos = new();

    public FakeObservacionRepository(params Observacion[] iniciales)
    {
        foreach (var o in iniciales)
        {
            _datos[o.ObservacionId] = o;
        }
    }

    public Task<Observacion?> ObtenerPorIdAsync(Guid observacionId, CancellationToken ct = default) =>
        Task.FromResult(_datos.GetValueOrDefault(observacionId));

    public Task<IReadOnlyList<Observacion>> ListarPorRelevamientoAsync(Guid relevamientoId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Observacion>>(_datos.Values.Where(o => o.RelevamientoId == relevamientoId).ToList());

    public Task AgregarAsync(Observacion observacion, CancellationToken ct = default)
    {
        _datos[observacion.ObservacionId] = observacion;
        return Task.CompletedTask;
    }

    public Task GuardarCambiosAsync(CancellationToken ct = default) => Task.CompletedTask;
}

internal sealed class FakeFotoRepository : IFotoRepository
{
    private readonly List<Foto> _datos;

    public FakeFotoRepository(params Foto[] iniciales) => _datos = iniciales.ToList();

    public Task<Foto?> ObtenerPorObservacionAsync(Guid observacionId, CancellationToken ct = default) =>
        Task.FromResult<Foto?>(_datos.FirstOrDefault(f => f.ObservacionId == observacionId));

    public Task<Foto?> ObtenerPorIdAsync(Guid fotoId, CancellationToken ct = default) =>
        Task.FromResult<Foto?>(_datos.FirstOrDefault(f => f.FotoId == fotoId));

    public Task<IReadOnlyList<Foto>> ListarPorMarcadorAsync(Guid marcadorId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Foto>>(_datos.Where(f => f.MarcadorId == marcadorId).ToList());

    public Task AgregarAsync(Foto foto, CancellationToken ct = default)
    {
        _datos.Add(foto);
        return Task.CompletedTask;
    }
}

internal sealed class FakeComentarioRepository : IComentarioRepository
{
    private readonly Dictionary<Guid, Comentario> _datos = new();

    public FakeComentarioRepository(params Comentario[] iniciales)
    {
        foreach (var c in iniciales)
        {
            _datos[c.ComentarioId] = c;
        }
    }

    public Task<Comentario?> ObtenerPorIdAsync(Guid comentarioId, CancellationToken ct = default) =>
        Task.FromResult(_datos.GetValueOrDefault(comentarioId));

    public Task<IReadOnlyList<Comentario>> ListarPorMarcadorAsync(Guid marcadorId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Comentario>>(_datos.Values.Where(c => c.MarcadorId == marcadorId).ToList());

    public Task AgregarAsync(Comentario comentario, CancellationToken ct = default)
    {
        _datos[comentario.ComentarioId] = comentario;
        return Task.CompletedTask;
    }

    public Task GuardarCambiosAsync(CancellationToken ct = default) => Task.CompletedTask;
}

internal sealed class FakeEtiquetaRepository : IEtiquetaRepository
{
    private readonly Dictionary<string, Etiqueta> _etiquetas = new();
    private readonly List<FotoEtiqueta> _fotoEtiquetas = new();
    private readonly List<ComentarioEtiqueta> _comentarioEtiquetas = new();

    public Task<Etiqueta?> ObtenerPorNombreAsync(string nombre, CancellationToken ct = default) =>
        Task.FromResult(_etiquetas.GetValueOrDefault(nombre));

    public Task AgregarAsync(Etiqueta etiqueta, CancellationToken ct = default)
    {
        _etiquetas[etiqueta.Nombre] = etiqueta;
        return Task.CompletedTask;
    }

    public Task AgregarFotoEtiquetaAsync(FotoEtiqueta union, CancellationToken ct = default)
    {
        _fotoEtiquetas.Add(union);
        return Task.CompletedTask;
    }

    public Task AgregarComentarioEtiquetaAsync(ComentarioEtiqueta union, CancellationToken ct = default)
    {
        _comentarioEtiquetas.Add(union);
        return Task.CompletedTask;
    }

    public Task<bool> ExisteFotoEtiquetaAsync(Guid fotoId, Guid etiquetaId, CancellationToken ct = default) =>
        Task.FromResult(_fotoEtiquetas.Any(u => u.FotoId == fotoId && u.EtiquetaId == etiquetaId));

    public Task<bool> ExisteComentarioEtiquetaAsync(Guid comentarioId, Guid etiquetaId, CancellationToken ct = default) =>
        Task.FromResult(_comentarioEtiquetas.Any(u => u.ComentarioId == comentarioId && u.EtiquetaId == etiquetaId));

    public Task<IReadOnlyList<string>> ListarNombresDeFotoAsync(Guid fotoId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<string>>(
            _fotoEtiquetas.Where(u => u.FotoId == fotoId)
                .Select(u => _etiquetas.Values.First(e => e.EtiquetaId == u.EtiquetaId).Nombre).ToList());

    public Task<IReadOnlyList<string>> ListarNombresDeComentarioAsync(Guid comentarioId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<string>>(
            _comentarioEtiquetas.Where(u => u.ComentarioId == comentarioId)
                .Select(u => _etiquetas.Values.First(e => e.EtiquetaId == u.EtiquetaId).Nombre).ToList());

    public Task GuardarCambiosAsync(CancellationToken ct = default) => Task.CompletedTask;
}

internal sealed class FakeReloj : IRelojUtc
{
    public DateTime AhoraUtc => new(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
}

internal sealed class FakeCredencialRepository : ICredencialRepository
{
    private readonly Dictionary<string, Credencial> _datos;

    public FakeCredencialRepository(params Credencial[] credenciales) =>
        _datos = credenciales.ToDictionary(c => c.NombreUsuario);

    public Task<Credencial?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default) =>
        Task.FromResult(_datos.GetValueOrDefault(nombreUsuario));

    public Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default) =>
        Task.FromResult(_datos.ContainsKey(nombreUsuario));

    public Task<Credencial?> ObtenerPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default) =>
        Task.FromResult<Credencial?>(_datos.Values.FirstOrDefault(c => c.UsuarioId == usuarioId));

    public Task AgregarAsync(Credencial credencial, CancellationToken ct = default)
    {
        _datos[credencial.NombreUsuario] = credencial;
        return Task.CompletedTask;
    }

    public Task GuardarCambiosAsync(CancellationToken ct = default) => Task.CompletedTask;
}

internal sealed class FakeAuditoria : IServicioAuditoria
{
    private readonly bool _exito;

    public FakeAuditoria(bool exito = true) => _exito = exito;

    public List<string> Registros { get; } = new();

    public Task<bool> RegistrarAsync(Guid autorUsuarioId, string operacion, string recursoAfectado, CancellationToken ct = default)
    {
        if (_exito)
        {
            Registros.Add($"{operacion}:{recursoAfectado}");
        }

        return Task.FromResult(_exito);
    }
}

internal sealed class FakeHasher : IHasherClave
{
    public string Hash(string clave) => $"h:{clave}";

    public bool Verificar(string clave, string hash) => hash == $"h:{clave}";
}

internal sealed class FakeToken : IServicioToken
{
    public string GenerarAccessToken(Usuario usuario, out int expiraEnSegundos)
    {
        expiraEnSegundos = 3600;
        return $"access-{usuario.UsuarioId}";
    }

    public string GenerarRefreshToken() => "refresh";
}
