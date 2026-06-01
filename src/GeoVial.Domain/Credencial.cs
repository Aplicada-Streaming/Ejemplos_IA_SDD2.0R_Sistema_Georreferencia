namespace GeoVial.Domain;

/// <summary>
/// Credencial de acceso de un usuario para el flujo ROPC (ADR-03). El hash de la clave nunca sale
/// del backend. No forma parte de las 12 entidades del modelo conceptual: es soporte técnico de
/// autenticación, ligado 1:1 a un Usuario.
/// </summary>
public sealed class Credencial
{
    public Guid CredencialId { get; private set; }
    public Guid UsuarioId { get; private set; }
    public string NombreUsuario { get; private set; }
    public string HashClave { get; private set; }

    // ctor para materialización del ORM
    private Credencial()
    {
        NombreUsuario = string.Empty;
        HashClave = string.Empty;
    }

    public Credencial(Guid usuarioId, string nombreUsuario, string hashClave)
    {
        CredencialId = Guid.NewGuid();
        UsuarioId = usuarioId;
        NombreUsuario = nombreUsuario;
        HashClave = hashClave;
    }
}
