namespace GeoVial.Revision;

/// <summary>
/// Configuración de las teselas del mapa de la revisión (US-21): única fuente de la URL de OpenStreetMap, su
/// atribución obligatoria y el host. <see cref="EsUrlDeTesela"/> es el contrato de qué peticiones son teselas
/// cacheables —el Service Worker de la web (`sw-teselas.js`) cachea exactamente ese host para el modo offline—.
/// Lógica pura y testeable; reusada por <see cref="MapaRevisionHtml"/>.
/// </summary>
public static class MapaTeselas
{
    /// <summary>Plantilla de URL de teselas de OpenStreetMap (libre, sin clave; requiere atribución).</summary>
    public const string UrlPlantilla = "https://tile.openstreetmap.org/{z}/{x}/{y}.png";

    /// <summary>Atribución obligatoria de OpenStreetMap (la agrega el tile layer de Leaflet).</summary>
    public const string Atribucion =
        "&copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors";

    /// <summary>Host de las teselas; lo que el Service Worker debe cachear para el modo offline.</summary>
    public const string Host = "tile.openstreetmap.org";

    /// <summary>
    /// Indica si una URL corresponde a una tesela cacheable (host de OSM + extensión de imagen de tesela).
    /// Es el contrato que implementa también el Service Worker en JavaScript.
    /// </summary>
    public static bool EsUrlDeTesela(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return false;
        }

        return uri.Host.Equals(Host, StringComparison.OrdinalIgnoreCase)
            && uri.AbsolutePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
    }
}
