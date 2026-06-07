namespace GeoVial.Sync;

/// <summary>
/// Apariencia de la cinta de estado de conexión/sincronización (UX experiencia-de-uso §4.1, hallazgo H-05 de la
/// auditoría UX móvil): ícono, texto, color de fondo y de texto. El ícono y el texto **no dependen solo del color**
/// (WCAG 2.2 AA, 1.4.1): cada estado lleva un glifo distinto. Núcleo puro y testeable (gate); la cinta de plataforma
/// (MAUI <c>CintaConexionView</c>) sólo pinta lo que esta función decide a partir del <see cref="ResumenSincronizacion"/>.
/// </summary>
public sealed record CintaVisual(string Icono, string Texto, string ColorFondo, string ColorTexto);

public static class PresentacionCintaConexion
{
    public static CintaVisual Para(ResumenSincronizacion resumen) => resumen.Estado switch
    {
        // Sin conexión / error: rojo de alerta, glifo de advertencia.
        EstadoSync.SinConexion => new CintaVisual("⚠", resumen.Texto, "#b00020", "#ffffff"),
        EstadoSync.Error => new CintaVisual("⚠", resumen.Texto, "#b00020", "#ffffff"),
        // Sincronizando: azul, glifo de actualización.
        EstadoSync.Sincronizando => new CintaVisual("↻", resumen.Texto, "#1565c0", "#ffffff"),
        // Pendiente: ámbar, glifo de punto; texto oscuro para contraste sobre ámbar.
        EstadoSync.Pendiente => new CintaVisual("●", resumen.Texto, "#e8a000", "#000000"),
        // Al día: verde, tilde.
        _ => new CintaVisual("✓", resumen.Texto, "#2e7d32", "#ffffff"),
    };
}
