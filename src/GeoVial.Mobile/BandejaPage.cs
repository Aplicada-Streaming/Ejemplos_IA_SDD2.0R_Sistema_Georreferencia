using GeoVial.Revision;
using GeoVial.Sync;

namespace GeoVial.Mobile;

/// <summary>
/// Solapa "Bandeja" (S50, RN-03): lista las observaciones del relevamiento activo cuya foto no traía GPS y
/// esperan ubicación manual (CU-05). Trae la revisión (<see cref="ClienteRevisionHttp"/>), arma las filas con
/// <see cref="PresentadorBandeja"/> (núcleo del gate) y las muestra. La colocación en el mapa desde la app
/// queda para un sprint siguiente; por ahora el agente ve qué quedó sin ubicar (se resuelve en la web).
/// Fuera de CI (cáscara MAUI).
/// </summary>
public sealed class BandejaPage : ContentPage
{
    private readonly ServicioSesion _sesion;
    private readonly ClienteRevisionHttp _cliente;
    private readonly CollectionView _lista;
    private readonly Label _estado;

    public BandejaPage(ServicioSesion sesion, ClienteRevisionHttp cliente)
    {
        _sesion = sesion;
        _cliente = cliente;
        Title = "Bandeja";

        _estado = new Label { Text = "Cargando la bandeja…", Margin = 16 };
        _lista = new CollectionView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                var etiqueta = new Label { Padding = new Thickness(16, 12), LineBreakMode = LineBreakMode.WordWrap };
                etiqueta.SetBinding(Label.TextProperty, nameof(FilaBandeja.Etiqueta));
                return etiqueta;
            }),
        };

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
            var filas = PresentadorBandeja.Presentar(revision?.Bandeja);
            _lista.ItemsSource = filas;

            if (filas.Count == 0)
            {
                _estado.Text = "Sin observaciones por ubicar. Todo lo capturado tiene su lugar en el mapa.";
                return;
            }

            _estado.Text = $"{filas.Count} observación(es) sin ubicar — se ubican manualmente en la web/revisión.";
            _lista.IsVisible = true;
        }
        catch (Exception ex)
        {
            _estado.Text = $"No se pudo cargar la bandeja: {ex.Message}";
        }
    }
}
