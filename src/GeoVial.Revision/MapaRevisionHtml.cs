using System.Globalization;
using System.Text.Json;

namespace GeoVial.Revision;

/// <summary>
/// Construye el documento HTML autocontenido del mapa de la revisión para mostrarlo en un WebView del móvil
/// (US-21): Leaflet + teselas de OpenStreetMap (sin clave; con atribución) y los marcadores de la
/// <see cref="VistaMapa"/>. Es lógica pura y testeable; el WebView de MAUI sólo carga el HTML resultante.
/// Reusa el mismo enfoque que el front web; las teselas vienen por red (la app necesita red para el mapa).
/// </summary>
public static class MapaRevisionHtml
{
    public static string Construir(VistaMapa vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        var ci = CultureInfo.InvariantCulture;
        var pinesJson = JsonSerializer.Serialize(vista.Pins.Select(p => new
        {
            lat = p.Latitud,
            lon = p.Longitud,
            conflicto = p.EnConflicto,
            fotos = p.Fotos,
            comentarios = p.Comentarios,
        }));

        // Plantilla con tokens (sin interpolación de C#) para no lidiar con el escape de llaves del JS/CSS.
        return Plantilla
            .Replace("__PINES__", pinesJson)
            .Replace("__CENTRO_LAT__", vista.CentroLat.ToString(ci))
            .Replace("__CENTRO_LON__", vista.CentroLon.ToString(ci))
            .Replace("__MIN_LAT__", vista.MinLat.ToString(ci))
            .Replace("__MIN_LON__", vista.MinLon.ToString(ci))
            .Replace("__MAX_LAT__", vista.MaxLat.ToString(ci))
            .Replace("__MAX_LON__", vista.MaxLon.ToString(ci));
    }

    private const string Plantilla =
"""
<!DOCTYPE html>
<html>
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css" />
  <script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script>
  <style>html, body, #mapa { height: 100%; margin: 0; }</style>
</head>
<body>
  <div id="mapa"></div>
  <script>
    var pines = __PINES__;
    var mapa = L.map('mapa');
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(mapa);
    if (pines.length === 0) {
      mapa.setView([__CENTRO_LAT__, __CENTRO_LON__], 4);
    } else {
      pines.forEach(function (p) {
        var txt = 'Marcador (' + p.lat.toFixed(5) + ', ' + p.lon.toFixed(5) + ')' +
          (p.conflicto ? ' &mdash; en conflicto' : '') +
          '<br>Fotos: ' + p.fotos + ' &middot; Comentarios: ' + p.comentarios;
        L.marker([p.lat, p.lon]).bindPopup(txt).addTo(mapa);
      });
      if (pines.length === 1) {
        mapa.setView([pines[0].lat, pines[0].lon], 16);
      } else {
        mapa.fitBounds([[__MIN_LAT__, __MIN_LON__], [__MAX_LAT__, __MAX_LON__]], { padding: [30, 30] });
      }
    }
  </script>
</body>
</html>
""";
}
