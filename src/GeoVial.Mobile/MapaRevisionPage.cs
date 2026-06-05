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

#if ANDROID
        // Al estar listo el WebView nativo, se le asigna el cliente que cachea las teselas en disco (offline)
        // y que intercepta el esquema centinela del pin tocado (S55) — como reemplaza al client de MAUI, la
        // intercepción del esquema debe hacerse en el client, no por el evento Navigating (que deja de dispararse).
        web.HandlerChanged += (_, _) =>
        {
            if (web.Handler?.PlatformView is Android.Webkit.WebView nativo)
            {
                var carpeta = Path.Combine(FileSystem.CacheDirectory, "teselas");
                nativo.SetWebViewClient(new MapaWebViewClient(new CacheTeselasDisco(carpeta), OnEsquemaMarcador));
            }
        };
#endif

        Content = web;
    }

    // S55: el HTML avisa por geovial-marcador://abrir?id=…; se extrae el id, se devuelve y se cierra el mapa.
    private void OnEsquemaMarcador(string url) => MainThread.BeginInvokeOnMainThread(async () =>
    {
        if (ParseadorMensajeMarcador.Intentar(url) is { } marcadorId)
        {
            _marcadorElegido.TrySetResult(marcadorId);
            await Navigation.PopModalAsync();
        }
    });

    protected override bool OnBackButtonPressed()
    {
        _marcadorElegido.TrySetResult(null);
        return base.OnBackButtonPressed();
    }
}
