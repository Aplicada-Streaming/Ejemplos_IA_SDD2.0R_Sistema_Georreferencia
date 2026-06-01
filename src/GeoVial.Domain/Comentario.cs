namespace GeoVial.Domain;

/// <summary>
/// Comentario de un marcador (modelo-datos-logico §1.8). Pertenece a un marcador y se liga a cero o una
/// foto (RC-04). Puede llevar varias etiquetas.
/// </summary>
public sealed class Comentario
{
    public Guid ComentarioId { get; private set; }
    public Guid MarcadorId { get; private set; }
    public Guid? FotoId { get; private set; }
    public Guid AutorUsuarioId { get; private set; }
    public string Texto { get; private set; }
    public DateTime Momento { get; private set; }

    // ctor para materialización del ORM
    private Comentario()
    {
        Texto = string.Empty;
    }

    private Comentario(Guid marcadorId, Guid? fotoId, Guid autorUsuarioId, string texto, DateTime momento)
    {
        ComentarioId = Guid.NewGuid();
        MarcadorId = marcadorId;
        FotoId = fotoId;
        AutorUsuarioId = autorUsuarioId;
        Texto = texto;
        Momento = momento;
    }

    public static Resultado<Comentario> Crear(Guid marcadorId, Guid? fotoId, Guid autorUsuarioId, string texto, DateTime momento)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return Resultado<Comentario>.Fallo(CodigosError.TextoRequerido);
        }

        return Resultado<Comentario>.Exito(new Comentario(marcadorId, fotoId, autorUsuarioId, texto.Trim(), momento));
    }
}
