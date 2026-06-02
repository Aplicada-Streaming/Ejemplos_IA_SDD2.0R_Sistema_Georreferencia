namespace GeoVial.Shared;

/// <summary>DTOs del contrato REST del slice de jerarquía y acceso (contratos-rest §, ADR-02).</summary>

// --- Acceso (CU-02, US-04/US-05) ---

public sealed record LoginRequest(string NombreUsuario, string Clave);

public sealed record TokenResponse(string AccessToken, string RefreshToken, int ExpiraEnSegundos);

/// <summary>Reingreso en terreno: el cliente declara si el método de seguridad del teléfono está presente (RN-06).</summary>
public sealed record ReingresoRequest(string NombreUsuario, bool MetodoSeguridadPresente);

/// <summary>Refresh condicionado al método de seguridad del teléfono (ADR-03).</summary>
public sealed record RefreshRequest(string RefreshToken, bool MetodoSeguridadPresente);

// --- Gestión de usuarios (CU-03, US-01/US-02) ---

public sealed record AltaUsuarioRequest(string Nombre, int Rol, Guid? AreaId);

public sealed record AsociarAreaRequest(Guid AreaId);

public sealed record UsuarioDto(
    Guid UsuarioId,
    string Nombre,
    int Rol,
    Guid? AreaId,
    bool Vigente,
    bool MetodoSeguridadConfigurado);

// --- Relevamientos (CU-01, CU-10; US-06/07/08/09/10) ---

public sealed record CrearRelevamientoRequest(string IdentificacionObra, decimal RadioAgrupacionMetros);

public sealed record AsignarAgentesRequest(IReadOnlyList<Guid> AgentesIds);

public sealed record TransicionRequest(int EstadoDestino);

public sealed record RelevamientoDto(
    Guid RelevamientoId,
    string IdentificacionObra,
    int Estado,
    decimal RadioAgrupacionMetros,
    Guid AreaId,
    IReadOnlyList<Guid> AgentesVigentes);

// --- Captura y georreferenciación (CU-04, CU-05; US-11/12/13/14) ---

public sealed record CapturarObservacionRequest(string ReferenciaArchivo, decimal? LatitudExif, decimal? LongitudExif);

public sealed record UbicarManualRequest(decimal Latitud, decimal Longitud);

public sealed record CapturaResponse(Guid ObservacionId, Guid? MarcadorId, bool SinGeorreferenciar);

public sealed record ObservacionDto(
    Guid ObservacionId,
    Guid RelevamientoId,
    Guid? MarcadorId,
    Guid AgenteUsuarioId,
    bool SinGeorreferenciar);

// --- Provisión de credenciales (BT-23) ---

public sealed record EstablecerCredencialRequest(string NombreUsuario, string Clave);

// --- Revisión sobre mapa y gestión de marcador (CU-08, CU-09; US-15/21/22) ---

public sealed record AgregarComentarioRequest(Guid? FotoId, string Texto);

public sealed record EtiquetarRequest(string Etiqueta);

public sealed record RevisionFotoDto(Guid FotoId, string ReferenciaArchivo, IReadOnlyList<string> Etiquetas);

public sealed record RevisionComentarioDto(Guid ComentarioId, string Texto, Guid? FotoId, IReadOnlyList<string> Etiquetas);

public sealed record RevisionMarcadorDto(
    Guid MarcadorId,
    decimal Latitud,
    decimal Longitud,
    bool EnConflicto,
    IReadOnlyList<RevisionFotoDto> Fotos,
    IReadOnlyList<RevisionComentarioDto> Comentarios);

public sealed record RevisionRelevamientoDto(
    Guid RelevamientoId,
    int Estado,
    IReadOnlyList<RevisionMarcadorDto> Marcadores,
    IReadOnlyList<Guid> ObservacionesSinGeorreferenciar);

// --- Detección y resolución de conflictos por radio (CU-11, CU-12; US-25/26) ---

public sealed record AjustarRadioRequest(decimal RadioMetros);

/// <summary>Decisión: 1 = unificar (requiere MarcadorResultanteId), 2 = mantener separados.</summary>
public sealed record ResolverConflictoRequest(int Decision, Guid? MarcadorResultanteId);

public sealed record ConflictoDetectadoDto(Guid ConflictoSyncId, Guid MarcadorA, Guid MarcadorB, double DistanciaMetros);

public sealed record ConflictoPendienteDto(Guid ConflictoSyncId, int Tipo, Guid MarcadorA, Guid MarcadorB);

// --- Sincronización de cambios de campo (CU-07; US-18) ---

/// <summary>Lote de cambios encolados a subir + la marca temporal desde la cual se piden actualizaciones.</summary>
public sealed record SincronizarRequest(DateTime? Desde, IReadOnlyList<CambioSyncDto> Cambios);

/// <summary>Cambio de comentario encolado. Operacion: 1 = crear, 2 = actualizar. CambioId es la clave de idempotencia.</summary>
public sealed record CambioSyncDto(
    Guid CambioId, int Operacion, Guid ComentarioId, Guid MarcadorId, Guid? FotoId, Guid AutorUsuarioId, string Texto, DateTime MarcaTemporal);

public sealed record SincronizarResponse(
    IReadOnlyList<Guid> Confirmados,
    IReadOnlyList<ConflictoSyncDto> Conflictos,
    IReadOnlyList<ActualizacionComentarioDto> Actualizaciones);

public sealed record ConflictoSyncDto(Guid ConflictoSyncId, int Tipo, string RecursosInvolucrados);

public sealed record ActualizacionComentarioDto(Guid ComentarioId, Guid MarcadorId, string Texto, DateTime MarcaTemporal);

// --- Auditoría y datos personales (CU-13, CU-14; US-30/US-31) ---

public sealed record RegistroAuditoriaDto(Guid AutorUsuarioId, DateTime Momento, string Operacion, string RecursoAfectado);
