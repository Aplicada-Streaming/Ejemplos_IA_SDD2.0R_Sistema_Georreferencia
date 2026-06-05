using System.Globalization;

namespace GeoVial.Revision;

/// <summary>
/// HTML autocontenido del mapa para ubicar manualmente una observación de la bandeja (S51, CU-05): Leaflet +
/// teselas de OpenStreetMap (sin clave). El agente toca el mapa para colocar/mover un marcador y confirma; al
/// confirmar, la página navega al esquema centinela <c>geovial-ubicar://place?lat=..&amp;lon=..</c>, que el
/// WebView del móvil intercepta (lo parsea <see cref="ParseadorMensajeUbicacion"/>) sin abrirlo. Lógica pura
/// y testeable; el WebView sólo carga el HTML. Reusa <see cref="MapaTeselas"/>.
/// </summary>
public static class MapaUbicacionHtml
{
    public static string Construir(double centroLat, double centroLon)
    {
        var ci = CultureInfo.InvariantCulture;
        return Plantilla
            .Replace("__URL_TESELAS__", MapaTeselas.UrlPlantilla)
            .Replace("__ATRIBUCION__", MapaTeselas.Atribucion)
            .Replace("__ESQUEMA__", ParseadorMensajeUbicacion.Esquema)
            .Replace("__CENTRO_LAT__", centroLat.ToString(ci))
            .Replace("__CENTRO_LON__", centroLon.ToString(ci));
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
  <style>
    html, body { height: 100%; margin: 0; }
    #mapa { position: absolute; top: 0; bottom: 56px; left: 0; right: 0; }
    #barra { position: absolute; bottom: 0; left: 0; right: 0; height: 56px; display: flex; }
    #confirmar { flex: 1; font-size: 16px; border: 0; background: #1565c0; color: #fff; }
    #confirmar:disabled { background: #9e9e9e; }
  </style>
</head>
<body>
  <div id="mapa"></div>
  <div id="barra"><button id="confirmar" disabled>Tocá el mapa para elegir el punto</button></div>
  <script>
    var mapa = L.map('mapa').setView([__CENTRO_LAT__, __CENTRO_LON__], 13);
    L.tileLayer('__URL_TESELAS__', { maxZoom: 19, attribution: '__ATRIBUCION__' }).addTo(mapa);
    var marcador = null;
    var elegido = null;
    var boton = document.getElementById('confirmar');
    mapa.on('click', function (e) {
      elegido = e.latlng;
      if (marcador) { marcador.setLatLng(e.latlng); } else { marcador = L.marker(e.latlng, { draggable: true }).addTo(mapa); marcador.on('dragend', function () { elegido = marcador.getLatLng(); }); }
      boton.disabled = false;
      boton.textContent = 'Confirmar (' + e.latlng.lat.toFixed(5) + ', ' + e.latlng.lng.toFixed(5) + ')';
    });
    boton.addEventListener('click', function () {
      if (!elegido) { return; }
      window.location.href = '__ESQUEMA__://place?lat=' + elegido.lat + '&lon=' + elegido.lng;
    });
  </script>
</body>
</html>
""";
}
