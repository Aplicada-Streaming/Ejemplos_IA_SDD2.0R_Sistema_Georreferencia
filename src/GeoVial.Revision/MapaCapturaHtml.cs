using System.Globalization;
using System.Text.Json;

namespace GeoVial.Revision;

/// <summary>
/// HTML autocontenido del mapa de la **pantalla de captura** (evolución de H-01, CU-04/CU-05): Leaflet + teselas
/// de OpenStreetMap (sin clave) con los marcadores del relevamiento como **contexto** (círculos, no arrastrables)
/// y un **pin de captura** que el agente coloca/mueve tocando el mapa. Al tocar (o arrastrar el pin) la página
/// navega al esquema centinela <c>geovial-ubicar://place?lat=..&amp;lon=..</c> que el WebView intercepta —lo
/// parsea <see cref="ParseadorMensajeUbicacion"/>— sin abrirlo, para fijar la coordenada de la captura sobre el
/// propio mapa (sin pasar por una página aparte ni tipear). Combina el contexto de <see cref="MapaRevisionHtml"/>
/// con el "tocar para elegir" de <see cref="MapaUbicacionHtml"/>. Lógica pura y testeable; el WebView sólo carga
/// el HTML. Reusa <see cref="MapaTeselas"/> y <see cref="VistaMapa"/>.
/// </summary>
public static class MapaCapturaHtml
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
        }));

        // Plantilla con tokens (sin interpolación de C#) para no lidiar con el escape de llaves del JS/CSS.
        return Plantilla
            .Replace("__URL_TESELAS__", MapaTeselas.UrlPlantilla)
            .Replace("__ATRIBUCION__", MapaTeselas.Atribucion)
            .Replace("__ESQUEMA__", ParseadorMensajeUbicacion.Esquema)
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
    L.tileLayer('__URL_TESELAS__', { maxZoom: 19, attribution: '__ATRIBUCION__' }).addTo(mapa);
    // Marcadores de contexto del relevamiento (círculos, no arrastrables): para ubicarse respecto de lo ya capturado.
    pines.forEach(function (p) {
      var color = p.conflicto ? '#c62828' : '#1565c0';
      L.circleMarker([p.lat, p.lon], { radius: 7, weight: 2, color: color, fillColor: color, fillOpacity: 0.5 })
        .addTo(mapa).bindPopup('Marcador (' + p.lat.toFixed(5) + ', ' + p.lon.toFixed(5) + ')');
    });
    if (pines.length === 0) {
      mapa.setView([__CENTRO_LAT__, __CENTRO_LON__], 4);
    } else if (pines.length === 1) {
      mapa.setView([pines[0].lat, pines[0].lon], 16);
    } else {
      mapa.fitBounds([[__MIN_LAT__, __MIN_LON__], [__MAX_LAT__, __MAX_LON__]], { padding: [30, 30] });
    }
    // H-01: el agente fija la coordenada de la captura tocando el mapa; un pin arrastrable marca el punto y la app
    // recibe la coordenada por el esquema centinela (lo intercepta MapaWebViewClient; el WebView no lo navega).
    var captura = null;
    function avisar(latlng) { window.location.href = '__ESQUEMA__://place?lat=' + latlng.lat + '&lon=' + latlng.lng; }
    function fijarCaptura(latlng) {
      if (captura) {
        captura.setLatLng(latlng);
      } else {
        captura = L.marker(latlng, { draggable: true }).addTo(mapa).bindPopup('Punto de la captura').openPopup();
        captura.on('dragend', function () { avisar(captura.getLatLng()); });
      }
      avisar(latlng);
    }
    mapa.on('click', function (e) { fijarCaptura(e.latlng); });
  </script>
</body>
</html>
""";
}
