using GeoVial.Revision;
using GeoVial.Sync;

namespace GeoVial.Mobile;

/// <summary>
/// Solapa "Mapa" (US-40): muestra el mapa interactivo del relevamiento de forma directa,
/// sin tener que entrar a la revisión. Carga el primer relevamiento (<see cref="ServicioSesion"/>),
/// arma el HTML con <see cref="MapaRevisionHtml"/> (núcleo testeable del gate) y lo muestra en un
/// WebView. En Android cachea las teselas de OpenStreetMap en disco para offline parcial
/// (<see cref="CacheTeselasDisco"/>). Fuera de CI (cáscara MAUI).
/// </summary>
public sealed class MapaPage : ContentPage
{
    private readonly ServicioSesion _sesion;
    private readonly ClienteRevisionHttp _cliente;
    private readonly WebView _web;
    private readonly Label _estado;
    private bool _cargado;

    public MapaPage(ServicioSesion sesion, ClienteRevisionHttp cliente, MonitorSincronizacion monitor)
    {
        _sesion = sesion;
        _cliente = cliente;
        Title = "Mapa";

        // H-05: cinta de estado de conexión persistente arriba del mapa.
        var cinta = new CintaConexionView();
        cinta.Vincular(monitor);

        _estado = new Label { Text = "Cargando el mapa…", Margin = 16 };
        _web = new WebView
        {
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
            IsVisible = false,
        };

#if ANDROID
        // Al estar listo el WebView nativo, se le asigna el cliente que cachea las teselas en disco.
        _web.HandlerChanged += (_, _) =>
        {
            if (_web.Handler?.PlatformView is Android.Webkit.WebView nativo)
            {
                var carpeta = Path.Combine(FileSystem.CacheDirectory, "teselas");
                nativo.SetWebViewClient(new MapaWebViewClient(new CacheTeselasDisco(carpeta)));
            }
        };
#endif

        ToolbarItems.Add(new ToolbarItem("Recargar", null, async () => await CargarAsync()));
        // H-02 (auditoría UX): centrar el mapa en la posición del agente (GPS).
        ToolbarItems.Add(new ToolbarItem("📍 Mi ubicación", null, async () => await CentrarEnMiUbicacionAsync()));

        var mapa = new Grid { Children = { _web, _estado } };
        var raiz = new Grid { RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star) } };
        raiz.Add(cinta, 0, 0);
        raiz.Add(mapa, 0, 1);
        Content = raiz;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_cargado)
        {
            await CargarAsync();
        }
    }

    // H-02: obtiene el GPS (pidiendo permiso) y recentra el mapa en la posición del agente con una marca distinta.
    private async Task CentrarEnMiUbicacionAsync()
    {
        if (!_cargado)
        {
            await CargarAsync();
        }

        var coordenada = await UbicacionDispositivo.ObtenerAsync();
        if (coordenada is null)
        {
            _estado.Text = "No se pudo obtener tu ubicación. Activá el GPS y el permiso de ubicación.";
            _estado.IsVisible = true;
            return;
        }

        await _web.EvaluateJavaScriptAsync(
            ScriptUbicacionDispositivo.Centrar((double)coordenada.Latitud, (double)coordenada.Longitud));
    }

    private async Task CargarAsync()
    {
        try
        {
            _estado.Text = "Cargando el mapa…";
            _estado.IsVisible = true;
            _web.IsVisible = false;

            if (await _sesion.RelevamientoActivoAsync() is not { } relevamientoId)
            {
                _estado.Text = "No hay un relevamiento en el backend para mostrar en el mapa.";
                return;
            }

            var revision = await _cliente.ObtenerAsync(relevamientoId);
            if (revision is null)
            {
                _estado.Text = "No se pudo obtener el relevamiento.";
                return;
            }

            var html = MapaRevisionHtml.Construir(new VistaMapa(revision.Marcadores));
            _web.Source = new HtmlWebViewSource { Html = html };
            _web.IsVisible = true;
            _estado.IsVisible = false;
            _cargado = true;
        }
        catch (Exception ex)
        {
            _estado.Text = $"No se pudo cargar el mapa: {ex.Message}";
            _estado.IsVisible = true;
        }
    }
}
