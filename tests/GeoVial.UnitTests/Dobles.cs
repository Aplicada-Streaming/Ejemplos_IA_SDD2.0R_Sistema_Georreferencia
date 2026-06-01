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
