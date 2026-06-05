using System.Globalization;

namespace GeoVial.Revision;

/// <summary>Coordenada que el agente eligió tocando el mapa para ubicar una observación (S51, CU-05).</summary>
public sealed record CoordenadaElegida(decimal Latitud, decimal Longitud);

/// <summary>
/// Puente WebView → app: el mapa de ubicación (S51) avisa la coordenada elegida navegando a una URL con un
/// esquema centinela (<c>geovial-ubicar://place?lat=..&amp;lon=..</c>) que la página intercepta y la app no
/// abre. Este parser es la pieza testeable: reconoce ese mensaje y extrae la coordenada (punto decimal
/// invariante), validando el rango; devuelve <c>null</c> para cualquier otra navegación. Mantenerlo puro
/// permite cubrir en el gate el contrato del puente sin depender del WebView.
/// </summary>
public static class ParseadorMensajeUbicacion
{
    /// <summary>Esquema centinela que usa el HTML del mapa para devolver la coordenada (no es navegable real).</summary>
    public const string Esquema = "geovial-ubicar";

    /// <summary>Devuelve la coordenada si <paramref name="url"/> es un mensaje de ubicación válido y en rango; si no, null.</summary>
    public static CoordenadaElegida? Intentar(string? url)
    {
        if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return null;
        }

        if (!uri.Scheme.Equals(Esquema, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var query = LeerQuery(uri.Query);
        if (!TryDecimal(query.GetValueOrDefault("lat"), out var lat) || !TryDecimal(query.GetValueOrDefault("lon"), out var lon))
        {
            return null;
        }

        // Rangos geográficos válidos (RN-03 ubica un punto real); fuera de rango se descarta como ruido.
        if (lat is < -90m or > 90m || lon is < -180m or > 180m)
        {
            return null;
        }

        return new CoordenadaElegida(lat, lon);
    }

    private static bool TryDecimal(string? valor, out decimal resultado) =>
        decimal.TryParse(valor, NumberStyles.Float, CultureInfo.InvariantCulture, out resultado);

    // Parseo simple de la query (sin dependencias): "?lat=-34.6&lon=-58.4" → { lat, lon }.
    private static Dictionary<string, string> LeerQuery(string query)
    {
        var resultado = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var par in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var i = par.IndexOf('=');
            if (i > 0)
            {
                resultado[Uri.UnescapeDataString(par[..i])] = Uri.UnescapeDataString(par[(i + 1)..]);
            }
        }

        return resultado;
    }
}
