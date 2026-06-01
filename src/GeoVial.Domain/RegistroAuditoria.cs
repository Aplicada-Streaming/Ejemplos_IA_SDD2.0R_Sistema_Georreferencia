namespace GeoVial.Domain;

/// <summary>
/// Asiento inalterable de auditoría (modelo-datos-logico §1.12). Sin UPDATE/DELETE dentro de la
/// retención (RN-07, ADR-14). Cada acción administrativa o de acceso a datos personales deja un asiento.
/// </summary>
public sealed class RegistroAuditoria
{
    public Guid RegistroAuditoriaId { get; private set; }
    public Guid AutorUsuarioId { get; private set; }
    public DateTime Momento { get; private set; }
    public string Operacion { get; private set; }
    public string RecursoAfectado { get; private set; }

    // ctor para materialización del ORM
    private RegistroAuditoria()
    {
        Operacion = string.Empty;
        RecursoAfectado = string.Empty;
    }

    public RegistroAuditoria(Guid autorUsuarioId, DateTime momentoUtc, string operacion, string recursoAfectado)
    {
        RegistroAuditoriaId = Guid.NewGuid();
        AutorUsuarioId = autorUsuarioId;
        Momento = momentoUtc;
        Operacion = operacion;
        RecursoAfectado = recursoAfectado;
    }
}
