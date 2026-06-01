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
