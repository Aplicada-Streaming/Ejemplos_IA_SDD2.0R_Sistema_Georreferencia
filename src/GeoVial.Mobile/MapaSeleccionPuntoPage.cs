using GeoVial.Revision;

namespace GeoVial.Mobile;

/// <summary>
/// Página modal para elegir un punto en el mapa (S56): el agente toca/arrastra un marcador sobre Leaflet+OSM y
/// confirma; devuelve la <see cref="CoordenadaElegida"/> sin tocar el backend. Reusa <see cref="MapaUbicacionHtml"/>
/// (núcleo del gate) y la intercepción del esquema centinela en <see cref="MapaWebViewClient"/>. La usan la
/// captura (para ubicar una foto sin GPS) y la bandeja (para ubicar una observación). Fuera de CI (cáscara MAUI).
/// </summary>
public sealed class MapaSeleccionPuntoPage : ContentPage
{
    private readonly TaskCompletionSource<CoordenadaElegida?> _punto = new();

    /// <summary>Se completa con el punto elegido, o <c>null</c> si se cerró el mapa sin confirmar.</summary>
    public Task<CoordenadaElegida?> PuntoElegido => _punto.Task;

    public MapaSeleccionPuntoPage(double centroLat, double centroLon)
    {
        Title = "Elegí el punto en el mapa";

        var web = new WebView
        {
            Source = new HtmlWebViewSource { Html = MapaUbicacionHtml.Construir(centroLat, centroLon) },
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
        };

#if ANDROID
        // Cachea las teselas de OSM (offline parcial) e intercepta el esquema centinela del punto confirmado:
        // como el client reemplaza al de MAUI, la intercepción va en el client, no por el evento Navigating.
        web.HandlerChanged += (_, _) =>
        {
            if (web.Handler?.PlatformView is Android.Webkit.WebView nativo)
            {
                var carpeta = Path.Combine(FileSystem.CacheDirectory, "teselas");
                nativo.SetWebViewClient(new MapaWebViewClient(new CacheTeselasDisco(carpeta),
                    url => MainThread.BeginInvokeOnMainThread(async () => await OnEsquemaPuntoAsync(url))));
            }
        };
#endif

        ToolbarItems.Add(new ToolbarItem("Cancelar", null, async () =>
        {
            _punto.TrySetResult(null);
            await Navigation.PopModalAsync();
        }));

        Content = web;
    }

    // El HTML avisa la coordenada por geovial-ubicar://place?lat=…&lon=…; se devuelve y se cierra.
    private async Task OnEsquemaPuntoAsync(string url)
    {
        if (ParseadorMensajeUbicacion.Intentar(url) is { } coordenada)
        {
            _punto.TrySetResult(coordenada);
            await Navigation.PopModalAsync();
        }
    }

    protected override bool OnBackButtonPressed()
    {
        _punto.TrySetResult(null);
        return base.OnBackButtonPressed();
    }
}
