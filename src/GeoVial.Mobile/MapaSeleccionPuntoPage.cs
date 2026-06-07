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
    private readonly WebView _web;

    /// <summary>Se completa con el punto elegido, o <c>null</c> si se cerró el mapa sin confirmar.</summary>
    public Task<CoordenadaElegida?> PuntoElegido => _punto.Task;

    public MapaSeleccionPuntoPage(double centroLat, double centroLon)
    {
        Title = "Elegí el punto en el mapa";

        _web = new WebView
        {
            Source = new HtmlWebViewSource { Html = MapaUbicacionHtml.Construir(centroLat, centroLon) },
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
        };

#if ANDROID
        // Cachea las teselas de OSM (offline parcial) e intercepta el esquema centinela del punto confirmado:
        // como el client reemplaza al de MAUI, la intercepción va en el client, no por el evento Navigating.
        _web.HandlerChanged += (_, _) =>
        {
            if (_web.Handler?.PlatformView is Android.Webkit.WebView nativo)
            {
                var carpeta = Path.Combine(FileSystem.CacheDirectory, "teselas");
                nativo.SetWebViewClient(new MapaWebViewClient(new CacheTeselasDisco(carpeta),
                    url => MainThread.BeginInvokeOnMainThread(async () => await OnEsquemaPuntoAsync(url))));
            }
        };
#endif

        // H-02 (auditoría UX): centrar el mapa en la posición del agente para elegir el punto cerca suyo.
        ToolbarItems.Add(new ToolbarItem("📍 Mi ubicación", null, async () => await CentrarEnMiUbicacionAsync()));
        ToolbarItems.Add(new ToolbarItem("Cancelar", null, async () =>
        {
            _punto.TrySetResult(null);
            await Navigation.PopModalAsync();
        }));

        Content = _web;
    }

    private async Task CentrarEnMiUbicacionAsync()
    {
        var coordenada = await UbicacionDispositivo.ObtenerAsync();
        if (coordenada is null)
        {
            await DisplayAlertAsync("Mi ubicación", "No se pudo obtener tu ubicación. Activá el GPS y el permiso de ubicación.", "OK");
            return;
        }

        await _web.EvaluateJavaScriptAsync(
            ScriptUbicacionDispositivo.Centrar((double)coordenada.Latitud, (double)coordenada.Longitud));
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
