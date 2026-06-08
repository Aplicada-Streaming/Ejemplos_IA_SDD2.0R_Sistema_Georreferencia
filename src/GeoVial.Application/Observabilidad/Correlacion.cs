namespace GeoVial.Application.Observabilidad;

/// <summary>
/// Identificador de correlación de request (hardening/observabilidad): un id que acompaña a una petición de punta
/// a punta para poder **rastrear** todos sus logs (y, a futuro, su traza entre servicios). Si el cliente o un
/// proxy ya envió un id por la cabecera <see cref="Cabecera"/> y es válido, se **reusa** (para correlacionar a
/// través de saltos); si no, se **genera** uno nuevo. Lógica pura y testeable; el middleware de la API sólo la
/// invoca, lo pone en el scope de logging y lo devuelve en la respuesta.
/// </summary>
public static class Correlacion
{
    /// <summary>Cabecera HTTP estándar de facto para el id de correlación (entrada y salida).</summary>
    public const string Cabecera = "X-Correlation-ID";

    /// <summary>Largo máximo aceptado de un id entrante (evita cabeceras abusivas).</summary>
    public const int LargoMaximo = 128;

    /// <summary>
    /// Devuelve el id de correlación a usar: el <paramref name="entrante"/> si es válido (recortado), o uno nuevo
    /// generado. El id nuevo es un GUID sin guiones (compacto y seguro para logs/cabeceras).
    /// </summary>
    public static string Resolver(string? entrante) =>
        EsValido(entrante) ? entrante!.Trim() : Guid.NewGuid().ToString("N");

    /// <summary>Un id entrante es válido si no está vacío, no excede el largo máximo y no trae caracteres de control.</summary>
    public static bool EsValido(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return false;
        }

        var recortado = id.Trim();
        return recortado.Length <= LargoMaximo && !recortado.Any(char.IsControl);
    }
}
