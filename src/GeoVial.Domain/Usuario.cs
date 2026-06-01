namespace GeoVial.Domain;

/// <summary>
/// Persona que opera el sistema en un nivel de la jerarquía (modelo-datos-logico §1.1).
/// Aplica la invariante RC-05: los roles jefe de área y agente de campo requieren área.
/// </summary>
public sealed class Usuario
{
    public Guid UsuarioId { get; private set; }
    public string Nombre { get; private set; }
    public RolJerarquico Rol { get; private set; }
    public Guid? AreaId { get; private set; }
    public bool EstadoVigencia { get; private set; }
    public bool MetodoSeguridadConfigurado { get; private set; }

    // ctor para materialización del ORM
    private Usuario()
    {
        Nombre = string.Empty;
    }

    private Usuario(Guid usuarioId, string nombre, RolJerarquico rol, Guid? areaId)
    {
        UsuarioId = usuarioId;
        Nombre = nombre;
        Rol = rol;
        AreaId = areaId;
        EstadoVigencia = true;
        MetodoSeguridadConfigurado = false;
    }

    /// <summary>
    /// Crea un usuario validando la invariante RC-05 (área obligatoria para jefe de área y agente).
    /// </summary>
    public static Resultado<Usuario> Crear(string nombre, RolJerarquico rol, Guid? areaId)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return Resultado<Usuario>.Fallo(CodigosError.NombreRequerido);
        }

        if ((rol == RolJerarquico.JefeArea || rol == RolJerarquico.AgenteCampo) && areaId is null)
        {
            return Resultado<Usuario>.Fallo(CodigosError.AreaRequerida);
        }

        return Resultado<Usuario>.Exito(new Usuario(Guid.NewGuid(), nombre.Trim(), rol, areaId));
    }

    /// <summary>Baja lógica: conserva datos e información producida (RN-08), solo marca no vigente (CU-03 §5.A).</summary>
    public void DarDeBaja() => EstadoVigencia = false;

    /// <summary>Configura el método de seguridad del teléfono, precondición del modo sin conexión (RN-06).</summary>
    public void ConfigurarMetodoSeguridad() => MetodoSeguridadConfigurado = true;

    public void AsociarArea(Guid areaId) => AreaId = areaId;
}
