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

    public Task AgregarAsync(Foto foto, CancellationToken ct = default)
    {
        _datos.Add(foto);
        return Task.CompletedTask;
    }
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
