namespace GeoVial.Application.Configuracion;

/// <summary>
/// Cabeceras de seguridad HTTP que el backend agrega a todas las respuestas (hardening). Son defensas de bajo
/// costo contra MIME sniffing, clickjacking y fuga de la URL de origen. Definidas como dato puro y testeable; el
/// middleware de la API sólo las aplica.
/// </summary>
public static class CabecerasSeguridad
{
    /// <summary>Cabeceras (nombre → valor) que se aplican a toda respuesta.</summary>
    public static IReadOnlyDictionary<string, string> Predeterminadas { get; } = new Dictionary<string, string>
    {
        // El navegador no debe "adivinar" el content-type (evita interpretar datos como script).
        ["X-Content-Type-Options"] = "nosniff",
        // No permitir embeber la respuesta en un iframe (clickjacking).
        ["X-Frame-Options"] = "DENY",
        // No filtrar la URL de origen al navegar a otros sitios.
        ["Referrer-Policy"] = "no-referrer",
        // Sólo el propio origen puede usar la respuesta como recurso (Spectre / robo de recursos).
        ["Cross-Origin-Resource-Policy"] = "same-origin",
    };
}
