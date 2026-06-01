namespace GeoVial.Domain;

/// <summary>
/// Etiqueta aplicable a fotos y comentarios para su posterior filtrado (modelo-datos-logico §1.9).
/// La relación con fotos y comentarios es muchos a muchos (RC-04), vía las uniones explícitas.
/// </summary>
public sealed class Etiqueta
{
    public Guid EtiquetaId { get; private set; }
    public string Nombre { get; private set; }

    // ctor para materialización del ORM
    private Etiqueta()
    {
        Nombre = string.Empty;
    }

    private Etiqueta(string nombre)
    {
        EtiquetaId = Guid.NewGuid();
        Nombre = nombre;
    }

    public static Resultado<Etiqueta> Crear(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return Resultado<Etiqueta>.Fallo(CodigosError.EtiquetaRequerida);
        }

        return Resultado<Etiqueta>.Exito(new Etiqueta(nombre.Trim()));
    }
}

/// <summary>Unión muchos a muchos entre Foto y Etiqueta (modelo-datos-logico §1.10, RC-04).</summary>
public sealed class FotoEtiqueta
{
    public Guid FotoId { get; private set; }
    public Guid EtiquetaId { get; private set; }

    private FotoEtiqueta()
    {
    }

    public FotoEtiqueta(Guid fotoId, Guid etiquetaId)
    {
        FotoId = fotoId;
        EtiquetaId = etiquetaId;
    }
}

/// <summary>Unión muchos a muchos entre Comentario y Etiqueta (modelo-datos-logico §1.10, RC-04).</summary>
public sealed class ComentarioEtiqueta
{
    public Guid ComentarioId { get; private set; }
    public Guid EtiquetaId { get; private set; }

    private ComentarioEtiqueta()
    {
    }

    public ComentarioEtiqueta(Guid comentarioId, Guid etiquetaId)
    {
        ComentarioId = comentarioId;
        EtiquetaId = etiquetaId;
    }
}
