namespace GeoVial.Revision;

/// <summary>
/// Andamiaje HTML compartido de los mapas Leaflet + OpenStreetMap del móvil (US-21; "control de mapa
/// compartido"): un **único lugar** para la versión de Leaflet, las teselas de OSM (reusa <see cref="MapaTeselas"/>)
/// y la estructura del documento (cabecera, <c>div#mapa</c>, creación del mapa y capa de teselas). Cada mapa
/// concreto —revisión (<see cref="MapaRevisionHtml"/>), ubicación (<see cref="MapaUbicacionHtml"/>) y captura
/// (<see cref="MapaCapturaHtml"/>)— aporta sólo sus **estilos**, su **cuerpo** extra y su **script** (marcadores
/// e interacciones, que ya operan sobre la variable global <c>mapa</c>). Antes el grueso del boilerplate estaba
/// triplicado; centralizarlo evita que la versión de Leaflet o el origen de teselas queden desincronizados.
/// Lógica pura y testeable; el WebView sólo carga el HTML resultante.
/// </summary>
public static class MapaLeaflet
{
    /// <summary>
    /// Arma el documento HTML del mapa: cabecera Leaflet común + <paramref name="estilos"/> y
    /// <paramref name="cuerpo"/> del mapa concreto + creación del mapa y teselas + <paramref name="script"/>.
    /// El <paramref name="script"/> debe venir con sus propios tokens ya resueltos (centro, pines, esquema…).
    /// </summary>
    public static string Documento(string estilos, string cuerpo, string script) =>
        Plantilla
            .Replace("__ESTILOS__", estilos)
            .Replace("__CUERPO__", cuerpo)
            .Replace("__URL_TESELAS__", MapaTeselas.UrlPlantilla)
            .Replace("__ATRIBUCION__", MapaTeselas.Atribucion)
            .Replace("__SCRIPT__", script);

    // El mapa y la capa de teselas se crean acá (comunes a los tres mapas); el script del mapa concreto opera
    // sobre la variable global `mapa`. Tokens (sin interpolación de C#) para no escapar las llaves del JS/CSS.
    private const string Plantilla =
"""
<!DOCTYPE html>
<html>
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css" />
  <script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script>
  <style>__ESTILOS__</style>
</head>
<body>
__CUERPO__
  <script>
    var mapa = L.map('mapa');
    L.tileLayer('__URL_TESELAS__', { maxZoom: 19, attribution: '__ATRIBUCION__' }).addTo(mapa);
    __SCRIPT__
  </script>
</body>
</html>
""";
}
