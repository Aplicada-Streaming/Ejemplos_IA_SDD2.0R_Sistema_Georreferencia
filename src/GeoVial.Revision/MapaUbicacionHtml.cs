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
        var script = Script
            .Replace("__ESQUEMA__", ParseadorMensajeUbicacion.Esquema)
            .Replace("__CENTRO_LAT__", centroLat.ToString(ci))
            .Replace("__CENTRO_LON__", centroLon.ToString(ci));

        return MapaLeaflet.Documento(Estilos, Cuerpo, script);
    }

    // El mapa ocupa todo menos una barra inferior con el botón de confirmar (lo propio de este mapa).
    private const string Estilos =
"""
html, body { height: 100%; margin: 0; }
    #mapa { position: absolute; top: 0; bottom: 56px; left: 0; right: 0; }
    #barra { position: absolute; bottom: 0; left: 0; right: 0; height: 56px; display: flex; }
    #confirmar { flex: 1; font-size: 16px; border: 0; background: #1565c0; color: #fff; }
    #confirmar:disabled { background: #9e9e9e; }
""";

    private const string Cuerpo =
"""
  <div id="mapa"></div>
  <div id="barra"><button id="confirmar" disabled>Tocá el mapa para elegir el punto</button></div>
""";

    private const string Script =
"""
mapa.setView([__CENTRO_LAT__, __CENTRO_LON__], 13);
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
""";
}
