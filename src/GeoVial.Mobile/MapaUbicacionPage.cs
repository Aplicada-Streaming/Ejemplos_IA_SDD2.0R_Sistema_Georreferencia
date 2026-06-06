using GeoVial.Revision;

namespace GeoVial.Mobile;

/// <summary>
/// Página modal para ubicar manualmente una observación de la bandeja sobre el mapa (S51, CU-05). Carga el
/// HTML de <see cref="MapaUbicacionHtml"/> en un WebView; cuando el agente confirma el punto, el HTML navega
/// al esquema centinela, que aquí se intercepta (<see cref="ParseadorMensajeUbicacion"/>), se postea la
/// coordenada (<see cref="ClienteUbicacionManual"/>) y se cierra la modal devolviendo el resultado. Fuera de
/// CI (cáscara MAUI); el puente y el parseo viven en el gate.
/// </summary>
public sealed class MapaUbicacionPage : ContentPage
{
    private readonly ClienteUbicacionManual _cliente;
    private readonly Guid _observacionId;
    private readonly TaskCompletionSource<bool> _resultado = new();
    private bool _enviando;

    /// <summary>Se completa con true si la observación quedó ubicada; false si se canceló o falló.</summary>
    public Task<bool> Resultado => _resultado.Task;

    public MapaUbicacionPage(ClienteUbicacionManual cliente, Guid observacionId, double centroLat, double centroLon)
    {
        _cliente = cliente;
        _observacionId = observacionId;
        Title = "Ubicar observación";

        var web = new WebView
        {
            Source = new HtmlWebViewSource { Html = MapaUbicacionHtml.Construir(centroLat, centroLon) },
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
        };
#if ANDROID
        // Cachea las teselas de OSM en disco (offline parcial) e intercepta el esquema centinela del punto
        // confirmado (S55): como el client reemplaza al de MAUI, la intercepción va en el client, no por Navigating.
        web.HandlerChanged += (_, _) =>
        {
            if (web.Handler?.PlatformView is Android.Webkit.WebView nativo)
            {
                var carpeta = Path.Combine(FileSystem.CacheDirectory, "teselas");
                nativo.SetWebViewClient(new MapaWebViewClient(new CacheTeselasDisco(carpeta),
                    url => MainThread.BeginInvokeOnMainThread(async () => await OnEsquemaUbicacionAsync(url))));
            }
        };
#endif

        ToolbarItems.Add(new ToolbarItem("Cancelar", null, async () =>
        {
            _resultado.TrySetResult(false);
            await Navigation.PopModalAsync();
        }));

        Content = web;
    }

    // S55: el HTML avisa la coordenada por geovial-ubicar://place?lat=…&lon=…; se postea y se cierra.
    private async Task OnEsquemaUbicacionAsync(string url)
    {
        if (ParseadorMensajeUbicacion.Intentar(url) is not { } coordenada)
        {
            return; // otra navegación: se ignora
        }

        if (_enviando)
        {
            return;
        }

        _enviando = true;
        bool ok;
        try
        {
            ok = await _cliente.UbicarAsync(_observacionId, coordenada.Latitud, coordenada.Longitud);
        }
        catch
        {
            ok = false;
        }

        if (!ok)
        {
            _enviando = false;
            await DisplayAlertAsync("Ubicar", "No se pudo ubicar la observación. Reintentá.", "OK");
            return;
        }

        _resultado.TrySetResult(true);
        await Navigation.PopModalAsync();
    }

    protected override bool OnBackButtonPressed()
    {
        _resultado.TrySetResult(false);
        return base.OnBackButtonPressed();
    }
}
