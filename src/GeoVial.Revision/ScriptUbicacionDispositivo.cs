using System.Globalization;

namespace GeoVial.Revision;

/// <summary>
/// Genera el JavaScript que centra el mapa Leaflet en la posición del dispositivo y deja una marca distinta de
/// "tu posición" (hallazgo H-02 de la auditoría UX móvil; experiencia-de-uso §6 / wireframes-captura-movil:
/// "Centrar por GPS" y "mapa con tu posición"). El HTML de los mapas (<see cref="MapaUbicacionHtml"/>,
/// <see cref="MapaRevisionHtml"/>) expone la variable global <c>mapa</c>; este script se inyecta con
/// <c>EvaluateJavaScriptAsync</c> desde la página al obtener la ubicación. Núcleo puro y testeable (gate); la
/// obtención real del GPS (permiso + Geolocation) es glue de plataforma. Usa punto decimal (InvariantCulture).
/// </summary>
public static class ScriptUbicacionDispositivo
{
    /// <summary>Script que recentra <c>mapa</c> en (lat, lon) y coloca/actualiza la marca azul "Tu posición".</summary>
    public static string Centrar(double latitud, double longitud)
    {
        var ci = CultureInfo.InvariantCulture;
        var lat = latitud.ToString(ci);
        var lon = longitud.ToString(ci);
        return
            "(function(){" +
            "if(typeof mapa==='undefined'||!mapa){return;}" +
            $"var p=[{lat},{lon}];" +
            "mapa.setView(p,16);" +
            "if(window.__posDisp){mapa.removeLayer(window.__posDisp);}" +
            "window.__posDisp=L.circleMarker(p,{radius:8,color:'#1565c0',weight:3,fillColor:'#1565c0',fillOpacity:0.6})" +
            ".addTo(mapa).bindPopup('Tu posición');" +
            "})();";
    }
}
