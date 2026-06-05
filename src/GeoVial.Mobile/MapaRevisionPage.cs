using GeoVial.Revision;

namespace GeoVial.Mobile;

/// <summary>
/// Muestra el mapa interactivo de la revisión (US-21) en un WebView: Leaflet + teselas de OpenStreetMap
/// (sin clave), con los marcadores del relevamiento. El HTML lo arma <see cref="GeoVial.Revision.MapaRevisionHtml"/>
/// (núcleo testeable en el gate); esta página sólo lo carga. En Android, intercepta las teselas para cachearlas
/// en disco (<see cref="CacheTeselasDisco"/>) y darle offline parcial al mapa. Fuera de CI (cáscara MAUI).
/// </summary>
public sealed class MapaRevisionPage : ContentPage
{
    private readonly TaskCompletionSource<Guid?> _marcadorElegido = new();

    /// <summary>Se completa con el id del marcador tocado (S55), o null si se cerró el mapa sin elegir.</summary>
    public Task<Guid?> MarcadorElegido => _marcadorElegido.Task;

    public MapaRevisionPage(string html)
    {
        Title = "Mapa de la revisión";
        ToolbarItems.Add(new ToolbarItem("Cerrar", null, async () =>
        {
            _marcadorElegido.TrySetResult(null);
            await Navigation.PopModalAsync();
        }));

        var web = new WebView
        {
            Source = new HtmlWebViewSource { Html = html },
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
        };
        // S55: al tocar un pin, el HTML navega al esquema centinela; se intercepta, se devuelve el id y se cierra.
        web.Navigating += async (_, e) =>
        {
            if (ParseadorMensajeMarcador.Intentar(e.Url) is { } marcadorId)
            {
                e.Cancel = true;
                _marcadorElegido.TrySetResult(marcadorId);
                await Navigation.PopModalAsync();
            }
        };

#if ANDROID
        // Al estar listo el WebView nativo, se le asigna el cliente que cachea las teselas en disco (offline).
        web.HandlerChanged += (_, _) =>
        {
            if (web.Handler?.PlatformView is Android.Webkit.WebView nativo)
            {
                var carpeta = Path.Combine(FileSystem.CacheDirectory, "teselas");
                nativo.SetWebViewClient(new MapaWebViewClient(new CacheTeselasDisco(carpeta)));
            }
        };
#endif

        Content = web;
    }

    protected override bool OnBackButtonPressed()
    {
        _marcadorElegido.TrySetResult(null);
        return base.OnBackButtonPressed();
    }
}
