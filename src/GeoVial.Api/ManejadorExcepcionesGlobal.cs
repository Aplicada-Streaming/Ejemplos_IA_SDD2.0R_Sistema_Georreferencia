using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeoVial.Api;

/// <summary>
/// Manejador global de excepciones no controladas (hardening): registra el error (con el id de correlación del
/// scope, S69) y responde un <see cref="ProblemDetails"/> 500 **genérico** —sin filtrar el mensaje ni el stack
/// trace de la excepción— en cualquier entorno, para no exponer detalles internos en producción. Reemplaza el
/// 500 por defecto y la fuga de detalles.
/// </summary>
internal sealed class ManejadorExcepcionesGlobal(IProblemDetailsService problemas, ILogger<ManejadorExcepcionesGlobal> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext contexto, Exception excepcion, CancellationToken cancellationToken)
    {
        logger.LogError(excepcion, "Excepción no controlada procesando {Metodo} {Ruta}", contexto.Request.Method, contexto.Request.Path);

        contexto.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return await problemas.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = contexto,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Error interno del servidor.",
                Detail = "Ocurrió un error inesperado. Si persiste, contactá al soporte con el identificador de correlación de la respuesta.",
            },
        });
    }
}
