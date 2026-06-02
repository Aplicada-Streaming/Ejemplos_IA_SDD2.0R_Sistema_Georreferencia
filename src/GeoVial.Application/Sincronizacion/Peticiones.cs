using GeoVial.Application.Cqrs;
using GeoVial.Domain;

namespace GeoVial.Application.Sincronizacion;

/// <summary>Sincronización de cambios de campo: subir/consolidar y bajar actualizaciones (CU-07, US-18).</summary>

/// <summary>Operación de un cambio encolado (RC-03): crear o actualizar un recurso.</summary>
public enum OperacionSync
{
    Crear = 1,
    Actualizar = 2,
}

/// <summary>
/// Un cambio de comentario encolado en la app de campo. <paramref name="CambioId"/> es la clave de
/// idempotencia (RC-03); <paramref name="MarcaTemporal"/> gobierna la consolidación last-write-wins (RN-04).
/// </summary>
public sealed record CambioComentario(
    Guid CambioId,
    OperacionSync Operacion,
    Guid ComentarioId,
    Guid MarcadorId,
    Guid? FotoId,
    Guid AutorUsuarioId,
    string Texto,
    DateTime MarcaTemporal);

/// <summary>
/// US-18 / CU-07: sube los cambios locales del agente, los consolida (last-write-wins + marca de conflicto)
/// de forma idempotente y baja las actualizaciones del relevamiento posteriores a <paramref name="Desde"/>.
/// </summary>
public sealed record SincronizarCommand(
    Guid AgenteId,
    Guid RelevamientoId,
    IReadOnlyList<CambioComentario> Cambios,
    DateTime? Desde)
    : IPeticion<Resultado<ResultadoSincronizacion>>;

public sealed record ResultadoSincronizacion(
    IReadOnlyList<Guid> Confirmados,
    IReadOnlyList<ConflictoSincronizado> Conflictos,
    IReadOnlyList<ActualizacionComentario> Actualizaciones);

public sealed record ConflictoSincronizado(Guid ConflictoSyncId, int Tipo, string RecursosInvolucrados);

public sealed record ActualizacionComentario(Guid ComentarioId, Guid MarcadorId, string Texto, DateTime MarcaTemporal);
