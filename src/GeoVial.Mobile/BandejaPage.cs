using GeoVial.Revision;
using GeoVial.Shared;
using GeoVial.Sync;

namespace GeoVial.Mobile;

/// <summary>
/// Solapa "Bandeja" (S50/S51, RN-03): lista las observaciones del relevamiento activo cuya foto no traía GPS
/// y esperan ubicación manual (CU-05). Trae la revisión (<see cref="ClienteRevisionHttp"/>), arma las filas
/// con <see cref="PresentadorBandeja"/> (núcleo del gate) y, al tocar una, abre el mapa para ubicarla
/// (<see cref="MapaUbicacionPage"/>, S51). Fuera de CI (cáscara MAUI).
/// </summary>
public sealed class BandejaPage : ContentPage
{
    private readonly ServicioSesion _sesion;
    private readonly ClienteRevisionHttp _cliente;
    private readonly ClienteUbicacionManual _ubicacion;
    private readonly CollectionView _lista;
    private readonly Label _estado;
    private double _centroLat = VistaMapa.CentroPorDefectoLat;
    private double _centroLon = VistaMapa.CentroPorDefectoLon;

    public BandejaPage(ServicioSesion sesion, ClienteRevisionHttp cliente, ClienteUbicacionManual ubicacion)
    {
        _sesion = sesion;
        _cliente = cliente;
        _ubicacion = ubicacion;
        Title = "Bandeja";

        _estado = new Label { Text = "Cargando la bandeja…", Margin = 16 };
        _lista = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            ItemTemplate = new DataTemplate(() =>
            {
                var etiqueta = new Label { Padding = new Thickness(16, 12), LineBreakMode = LineBreakMode.WordWrap };
                etiqueta.SetBinding(Label.TextProperty, nameof(FilaBandeja.Etiqueta));
                return etiqueta;
            }),
        };
        _lista.SelectionChanged += OnSeleccion;

        ToolbarItems.Add(new ToolbarItem("Recargar", null, async () => await CargarAsync()));

        // El estado/conteo va arriba (fila Auto) y la lista debajo (fila *), sin superponerse.
        var grid = new Grid { RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star) } };
        grid.Add(_estado, 0, 0);
        grid.Add(_lista, 0, 1);
        Content = grid;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarAsync();
    }

    private async Task CargarAsync()
    {
        try
        {
            _estado.IsVisible = true;
            _estado.Text = "Cargando la bandeja…";
            _lista.IsVisible = false;

            if (await _sesion.RelevamientoActivoAsync() is not { } relevamientoId)
            {
                _estado.Text = "No hay un relevamiento activo. Elegí uno en la solapa Sync.";
                return;
            }

            var revision = await _cliente.ObtenerAsync(relevamientoId);
            // Centra el mapa de ubicación en los marcadores ya existentes (o el centro por defecto si no hay).
            var vista = new VistaMapa(revision?.Marcadores ?? Array.Empty<RevisionMarcadorDto>());
            _centroLat = vista.CentroLat;
            _centroLon = vista.CentroLon;

            var filas = PresentadorBandeja.Presentar(revision?.Bandeja);
            _lista.ItemsSource = filas;

            if (filas.Count == 0)
            {
                _estado.Text = "Sin observaciones por ubicar. Todo lo capturado tiene su lugar en el mapa.";
                return;
            }

            _estado.Text = $"{filas.Count} observación(es) sin ubicar — tocá una para ubicarla en el mapa.";
            _lista.IsVisible = true;
        }
        catch (Exception ex)
        {
            _estado.Text = $"No se pudo cargar la bandeja: {ex.Message}";
        }
    }

    // S51: al tocar una observación, abre el mapa para ubicarla; si se ubica, recarga la bandeja.
    private async void OnSeleccion(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not FilaBandeja fila)
        {
            return;
        }

        _lista.SelectedItem = null; // permite volver a tocar la misma fila luego

        var pagina = new MapaUbicacionPage(_ubicacion, fila.ObservacionId, _centroLat, _centroLon);
        await Navigation.PushModalAsync(new NavigationPage(pagina));
        if (await pagina.Resultado)
        {
            await DisplayAlertAsync("Bandeja", "Observación ubicada.", "OK");
            await CargarAsync();
        }
    }
}
