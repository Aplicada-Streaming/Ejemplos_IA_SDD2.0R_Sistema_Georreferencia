namespace GeoVial.Domain;

/// <summary>
/// Ámbito administrativo de autorización (modelo-datos-logico §1.2). No es tenant (multi_tenant=false):
/// acota qué recursos ve y modifica cada usuario, sin segmentar físicamente la base.
/// </summary>
public sealed class Area
{
    public Guid AreaId { get; private set; }
    public string Nombre { get; private set; }
    public Guid? JefeAreaUsuarioId { get; private set; }

    // ctor para materialización del ORM
    private Area()
    {
        Nombre = string.Empty;
    }

    private Area(Guid areaId, string nombre)
    {
        AreaId = areaId;
        Nombre = nombre;
    }

    public static Area Crear(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del área es obligatorio.", nameof(nombre));
        }

        return new Area(Guid.NewGuid(), nombre.Trim());
    }

    public void AsignarJefe(Guid jefeAreaUsuarioId) => JefeAreaUsuarioId = jefeAreaUsuarioId;
}
