namespace GeoVial.Application.Configuracion;

/// <summary>
/// Guardrails de arranque para **producción** (hardening): valida que el backend no arranque con una
/// configuración insegura — la base **en memoria** (que pierde datos y nunca debe usarse en producción) o la
/// **clave JWT de desarrollo** (pública, en el repo). Lógica pura y testeable; <c>Program</c> la invoca al
/// arrancar y, si hay errores, **falla rápido** (no levanta el servidor) en vez de correr inseguro. En entornos
/// no productivos (Development/tests) es permisiva: ahí la base en memoria y la clave de desarrollo son válidas.
/// </summary>
public static class GuardrailsProduccion
{
    /// <summary>Clave JWT de desarrollo (la de <c>appsettings.Development.json</c>); prohibida en producción.</summary>
    public const string ClaveJwtDesarrollo = "clave-de-desarrollo-no-productiva-geovial-32bytes!!";

    /// <summary>Largo mínimo de la clave JWT (HS256 recomienda ≥ 256 bits = 32 bytes).</summary>
    public const int LargoMinimoClaveJwt = 32;

    private const string EntornoProduccion = "Production";

    /// <summary>
    /// Devuelve la lista de problemas **fatales** de configuración para el entorno dado (vacía = OK). Sólo es
    /// estricta en <c>Production</c>: exige cadena de conexión real (no InMemory) y una clave JWT propia, larga
    /// y distinta de la de desarrollo.
    /// </summary>
    public static IReadOnlyList<string> Validar(string entorno, string? cadenaConexion, string? claveJwt)
    {
        var errores = new List<string>();

        if (!string.Equals(entorno, EntornoProduccion, StringComparison.OrdinalIgnoreCase))
        {
            return errores; // Development / Staging / tests: permisivo
        }

        if (string.IsNullOrWhiteSpace(cadenaConexion))
        {
            errores.Add("ConnectionStrings:GeoVial es obligatoria en producción (no se permite la base en memoria); provéela por variable de entorno o secreto.");
        }

        if (string.IsNullOrWhiteSpace(claveJwt))
        {
            errores.Add("Jwt:ClaveSecreta es obligatoria en producción; provéela por variable de entorno o secreto.");
        }
        else
        {
            if (string.Equals(claveJwt, ClaveJwtDesarrollo, StringComparison.Ordinal))
            {
                errores.Add("Jwt:ClaveSecreta no puede ser la clave de desarrollo en producción (es pública).");
            }

            if (claveJwt.Length < LargoMinimoClaveJwt)
            {
                errores.Add($"Jwt:ClaveSecreta debe tener al menos {LargoMinimoClaveJwt} caracteres en producción.");
            }
        }

        return errores;
    }
}
