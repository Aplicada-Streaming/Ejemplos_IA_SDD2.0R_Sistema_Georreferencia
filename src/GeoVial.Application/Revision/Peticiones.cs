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

/// <summary>
/// Revisión del relevamiento (US-21). Si <paramref name="Etiquetas"/> no está vacío, filtra fotos y
/// comentarios por esas etiquetas y descarta los marcadores sin contenido coincidente (US-23, CU-08 §5.C).
/// </summary>
public sealed record RevisarRelevamientoQuery(Guid SolicitanteId, Guid RelevamientoId, IReadOnlyList<string> Etiquetas)
    : IPeticion<RevisionRelevamiento?>
{
    public RevisarRelevamientoQuery(Guid solicitanteId, Guid relevamientoId)
        : this(solicitanteId, relevamientoId, Array.Empty<string>())
    {
    }
}

// --- Proyección de la revisión consolidada (US-21) ---

public sealed record RevisionRelevamiento(
    Guid RelevamientoId,
    int Estado,
    IReadOnlyList<RevisionMarcador> Marcadores,
    IReadOnlyList<Guid> ObservacionesSinGeorreferenciar,
    IReadOnlyList<ObservacionSinGeo> Bandeja);

/// <summary>
/// Entrada de la bandeja sin georreferenciar (RN-03): una observación cuya foto no traía GPS y espera
/// ubicación manual (CU-05). Lleva el momento de captura y la referencia de la foto para que el agente la
/// reconozca en la app (S50). Enriquece a <see cref="RevisionRelevamiento.ObservacionesSinGeorreferenciar"/>
/// (que sólo lleva los IDs, conservado por compatibilidad).
/// </summary>
public sealed record ObservacionSinGeo(Guid ObservacionId, DateTime MomentoCaptura, string? ReferenciaArchivo);

public sealed record RevisionMarcador(
    Guid MarcadorId,
    decimal Latitud,
    decimal Longitud,
    bool EnConflicto,
    IReadOnlyList<RevisionFoto> Fotos,
    IReadOnlyList<RevisionComentario> Comentarios);

public sealed record RevisionFoto(Guid FotoId, string ReferenciaArchivo, IReadOnlyList<string> Etiquetas);

public sealed record RevisionComentario(Guid ComentarioId, string Texto, Guid? FotoId, IReadOnlyList<string> Etiquetas);
