namespace GeoVial.Revision;

/// <summary>
/// Puente WebView → app para abrir un marcador desde el mapa (S55): al tocar un pin, el HTML del mapa navega a
/// una URL con esquema centinela (<c>geovial-marcador://abrir?id=&lt;guid&gt;</c>) que la página intercepta y
/// la app no abre. Este parser es la pieza testeable: reconoce ese mensaje y extrae el id del marcador;
/// devuelve <c>null</c> para cualquier otra navegación. Espejo de <see cref="ParseadorMensajeUbicacion"/>.
/// </summary>
public static class ParseadorMensajeMarcador
{
    /// <summary>Esquema centinela que usa el HTML del mapa para avisar qué marcador se tocó.</summary>
    public const string Esquema = "geovial-marcador";

    /// <summary>Devuelve el id del marcador si <paramref name="url"/> es un mensaje válido; si no, null.</summary>
    public static Guid? Intentar(string? url)
    {
        if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return null;
        }

        if (!uri.Scheme.Equals(Esquema, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        foreach (var par in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var i = par.IndexOf('=');
            if (i > 0 && Uri.UnescapeDataString(par[..i]).Equals("id", StringComparison.OrdinalIgnoreCase)
                && Guid.TryParse(Uri.UnescapeDataString(par[(i + 1)..]), out var id))
            {
                return id;
            }
        }

        return null;
    }
}
