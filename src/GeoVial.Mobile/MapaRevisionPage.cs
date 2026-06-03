namespace GeoVial.Mobile;

/// <summary>
/// Muestra el mapa interactivo de la revisión (US-21) en un WebView: Leaflet + teselas de OpenStreetMap
/// (sin clave), con los marcadores del relevamiento. El HTML lo arma <see cref="GeoVial.Revision.MapaRevisionHtml"/>
/// (núcleo testeable en el gate); esta página sólo lo carga. Fuera de CI (cáscara MAUI).
/// </summary>
public sealed class MapaRevisionPage : ContentPage
{
    public MapaRevisionPage(string html)
    {
        Title = "Mapa de la revisión";
        ToolbarItems.Add(new ToolbarItem("Cerrar", null, async () => await Navigation.PopModalAsync()));
        Content = new WebView
        {
            Source = new HtmlWebViewSource { Html = html },
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
        };
    }
}
