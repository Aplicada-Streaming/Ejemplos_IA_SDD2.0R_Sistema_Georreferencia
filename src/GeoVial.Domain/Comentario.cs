namespace GeoVial.Domain;

/// <summary>
/// Comentario de un marcador (modelo-datos-logico §1.8). Pertenece a un marcador y se liga a cero o una
/// foto (RC-04). Puede llevar varias etiquetas. Lleva la marca de última edición para la consolidación
/// last-write-wins de la sincronización (RN-04).
/// </summary>
public sealed class Comentario
{
    public Guid ComentarioId { get; private set; }
    public Guid MarcadorId { get; private set; }
    public Guid? FotoId { get; private set; }
    public Guid AutorUsuarioId { get; private set; }
    public string Texto { get; private set; }
    public DateTime Momento { get; private set; }
    public DateTime MarcaUltimaEdicion { get; private set; }

    // ctor para materialización del ORM
    private Comentario()
    {
        Texto = string.Empty;
    }

    private Comentario(Guid comentarioId, Guid marcadorId, Guid? fotoId, Guid autorUsuarioId, string texto, DateTime momento)
    {
        ComentarioId = comentarioId;
        MarcadorId = marcadorId;
        FotoId = fotoId;
        AutorUsuarioId = autorUsuarioId;
        Texto = texto;
        Momento = momento;
        MarcaUltimaEdicion = momento;
    }

    public static Resultado<Comentario> Crear(Guid marcadorId, Guid? fotoId, Guid autorUsuarioId, string texto, DateTime momento)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return Resultado<Comentario>.Fallo(CodigosError.TextoRequerido);
        }

        return Resultado<Comentario>.Exito(new Comentario(Guid.NewGuid(), marcadorId, fotoId, autorUsuarioId, texto.Trim(), momento));
    }

    /// <summary>
    /// Reconstruye un comentario con un identificador dado al sincronizarlo desde la app de campo (CU-07).
    /// El identificador estable lo genera el cliente (RC-03) y permite que las ediciones posteriores lo referencien.
    /// </summary>
    public static Resultado<Comentario> Sincronizar(Guid comentarioId, Guid marcadorId, Guid? fotoId, Guid autorUsuarioId, string texto, DateTime momento)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return Resultado<Comentario>.Fallo(CodigosError.TextoRequerido);
        }

        return Resultado<Comentario>.Exito(new Comentario(comentarioId, marcadorId, fotoId, autorUsuarioId, texto.Trim(), momento));
    }

    /// <summary>Indica si el comentario fue editado después de su creación (hay una escritura central que compite).</summary>
    public bool FueEditado => MarcaUltimaEdicion > Momento;

    /// <summary>Aplica una edición con su marca temporal (consolidación last-write-wins, RN-04).</summary>
    public void AplicarEdicion(string texto, DateTime marca)
    {
        Texto = texto.Trim();
        MarcaUltimaEdicion = marca;
    }

    /// <summary>Reasigna el comentario a otro marcador al unificar marcadores en conflicto (CU-12 §5.A).</summary>
    public void ReasignarMarcador(Guid marcadorId) => MarcadorId = marcadorId;

    /// <summary>
    /// Desliga el comentario de su foto al quitar esa foto del marcador (US-15/CU-09 §5.A): el comentario
    /// sobrevive a nivel marcador (conserva su texto y etiquetas) en vez de borrarse junto con la foto.
    /// </summary>
    public void DesvincularFoto() => FotoId = null;
}
