using GeoVial.Application.Cqrs;
using GeoVial.Domain;

namespace GeoVial.Application.Revision;

/// <summary>Commands y query del módulo de revisión sobre mapa y gestión de marcador (CU-08, CU-09).</summary>

public sealed record AgregarComentarioCommand(Guid UsuarioId, Guid MarcadorId, Guid? FotoId, string Texto)
    : IPeticion<Resultado>;

public sealed record EtiquetarFotoCommand(Guid UsuarioId, Guid FotoId, string Etiqueta)
    : IPeticion<Resultado>;

public sealed record EtiquetarComentarioCommand(Guid UsuarioId, Guid ComentarioId, string Etiqueta)
    : IPeticion<Resultado>;

public sealed record RevisarRelevamientoQuery(Guid SolicitanteId, Guid RelevamientoId)
    : IPeticion<RevisionRelevamiento?>;

// --- Proyección de la revisión consolidada (US-21) ---

public sealed record RevisionRelevamiento(
    Guid RelevamientoId,
    int Estado,
    IReadOnlyList<RevisionMarcador> Marcadores,
    IReadOnlyList<Guid> ObservacionesSinGeorreferenciar);

public sealed record RevisionMarcador(
    Guid MarcadorId,
    decimal Latitud,
    decimal Longitud,
    bool EnConflicto,
    IReadOnlyList<RevisionFoto> Fotos,
    IReadOnlyList<RevisionComentario> Comentarios);

public sealed record RevisionFoto(Guid FotoId, string ReferenciaArchivo, IReadOnlyList<string> Etiquetas);

public sealed record RevisionComentario(Guid ComentarioId, string Texto, Guid? FotoId, IReadOnlyList<string> Etiquetas);
