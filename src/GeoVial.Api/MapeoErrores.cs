using GeoVial.Domain;

namespace GeoVial.Api;

/// <summary>
/// Traduce los códigos de error del dominio a respuestas Problem Details (RFC 7807, ADR-11).
/// </summary>
public static class MapeoErrores
{
    public static IResult AProblema(string? codigo) => codigo switch
    {
        CodigosError.CredencialesInvalidas => Problema(codigo, StatusCodes.Status401Unauthorized, "Credenciales inválidas"),
        CodigosError.AccesoNoAutorizado => Problema(codigo, StatusCodes.Status403Forbidden, "Acceso no autorizado"),
        CodigosError.AccesoDatoPersonalNoAutorizado => Problema(codigo, StatusCodes.Status403Forbidden, "Acceso a dato personal no autorizado"),
        CodigosError.OfflineNoHabilitado => Problema(codigo, StatusCodes.Status409Conflict, "Modo sin conexión no habilitado"),
        CodigosError.ReingresoSinMetodoSeguridad => Problema(codigo, StatusCodes.Status409Conflict, "Reingreso sin método de seguridad"),
        CodigosError.AreaInexistente => Problema(codigo, StatusCodes.Status404NotFound, "Área inexistente"),
        CodigosError.UsuarioInexistente => Problema(codigo, StatusCodes.Status404NotFound, "Usuario inexistente"),
        CodigosError.RelevamientoInexistente => Problema(codigo, StatusCodes.Status404NotFound, "Relevamiento inexistente"),
        CodigosError.ObservacionInexistente => Problema(codigo, StatusCodes.Status404NotFound, "Observación inexistente"),
        CodigosError.MarcadorInexistente => Problema(codigo, StatusCodes.Status404NotFound, "Marcador inexistente"),
        CodigosError.FuenteUbicacionIncorrecta => Problema(codigo, StatusCodes.Status409Conflict, "Fuente de ubicación incorrecta"),
        CodigosError.ObservacionSinGeorreferencia => Problema(codigo, StatusCodes.Status409Conflict, "Observación sin georreferencia"),
        CodigosError.ComentarioInexistente => Problema(codigo, StatusCodes.Status404NotFound, "Comentario inexistente"),
        CodigosError.FotoInexistente => Problema(codigo, StatusCodes.Status404NotFound, "Foto inexistente"),
        CodigosError.TextoRequerido => Problema(codigo, StatusCodes.Status400BadRequest, "Texto requerido"),
        CodigosError.EtiquetaRequerida => Problema(codigo, StatusCodes.Status400BadRequest, "Etiqueta requerida"),
        CodigosError.NombreUsuarioEnUso => Problema(codigo, StatusCodes.Status409Conflict, "Nombre de usuario en uso"),
        CodigosError.ClaveRequerida => Problema(codigo, StatusCodes.Status400BadRequest, "Clave requerida"),
        CodigosError.NombreUsuarioRequerido => Problema(codigo, StatusCodes.Status400BadRequest, "Nombre de usuario requerido"),
        CodigosError.NombreRequerido => Problema(codigo, StatusCodes.Status400BadRequest, "Nombre requerido"),
        CodigosError.AreaRequerida => Problema(codigo, StatusCodes.Status400BadRequest, "Área requerida"),
        CodigosError.IdentificacionRequerida => Problema(codigo, StatusCodes.Status400BadRequest, "Identificación de obra requerida"),
        CodigosError.RadioInvalido => Problema(codigo, StatusCodes.Status400BadRequest, "Radio de agrupación inválido"),
        CodigosError.AgenteFueraDeArea => Problema(codigo, StatusCodes.Status409Conflict, "Agente fuera del área"),
        CodigosError.RelevamientoSoloLectura => Problema(codigo, StatusCodes.Status409Conflict, "Relevamiento de solo lectura"),
        CodigosError.TransicionInvalida => Problema(codigo, StatusCodes.Status409Conflict, "Transición de estado inválida"),
        CodigosError.ReaperturaNoAutorizada => Problema(codigo, StatusCodes.Status409Conflict, "Reapertura no autorizada"),
        CodigosError.AccionNoAuditada => Problema(codigo, StatusCodes.Status503ServiceUnavailable, "La acción no pudo auditarse"),
        _ => Problema(codigo ?? "ERROR_DESCONOCIDO", StatusCodes.Status500InternalServerError, "Error"),
    };

    private static IResult Problema(string codigo, int status, string titulo) =>
        Results.Problem(
            title: titulo,
            statusCode: status,
            type: $"https://geovial/errores/{codigo}",
            extensions: new Dictionary<string, object?> { ["codigo"] = codigo });
}
