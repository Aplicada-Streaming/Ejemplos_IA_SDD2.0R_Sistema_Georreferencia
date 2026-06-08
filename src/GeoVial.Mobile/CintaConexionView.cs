using GeoVial.Sync;
using Microsoft.Maui.Accessibility;

namespace GeoVial.Mobile;

/// <summary>
/// Cinta de estado de conexión/sincronización **persistente** (UX experiencia-de-uso §4.1; hallazgo H-05 de la
/// auditoría UX móvil): se muestra fija en la parte superior de las solapas de trabajo (Captura, Revisión, Mapa,
/// Bandeja) para que el estado "sin conexión / pendientes" esté siempre a la vista —especialmente durante la
/// captura en terreno—. Reusa el <see cref="MonitorSincronizacion"/> (S48) y pinta lo que decide la función pura
/// <see cref="PresentacionCintaConexion"/> (gate). Ícono + texto, no sólo color (WCAG 2.2 AA, 1.4.1). Se suscribe
/// al monitor sólo mientras está visible (Loaded/Unloaded) para no fugar ni duplicar al cambiar de solapa.
/// </summary>
public sealed class CintaConexionView : ContentView
{
    private readonly Label _icono = new() { FontSize = 14, VerticalOptions = LayoutOptions.Center };
    private readonly Label _texto = new() { FontSize = 13, VerticalOptions = LayoutOptions.Center, LineBreakMode = LineBreakMode.TailTruncation };
    private readonly Border _borde;
    private MonitorSincronizacion? _monitor;

    public CintaConexionView()
    {
        _borde = new Border
        {
            Padding = new Thickness(12, 6),
            StrokeThickness = 0,
            Content = new HorizontalStackLayout { Spacing = 8, Children = { _icono, _texto } },
        };
        Content = _borde;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>La página la vincula en su constructor con el monitor (singleton) inyectado por DI.</summary>
    public void Vincular(MonitorSincronizacion monitor) => _monitor = monitor;

    private async void OnLoaded(object? sender, EventArgs e)
    {
        if (_monitor is null)
        {
            return;
        }

        _monitor.Cambiado += OnCambiado;
        Render(_monitor.Actual);
        try { await _monitor.RefrescarAsync(); } catch { /* el último estado conocido ya quedó pintado */ }
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        if (_monitor is not null)
        {
            _monitor.Cambiado -= OnCambiado;
        }
    }

    private void OnCambiado(object? sender, ResumenSincronizacion resumen) =>
        MainThread.BeginInvokeOnMainThread(() => Render(resumen, anunciar: true));

    private void Render(ResumenSincronizacion resumen, bool anunciar = false)
    {
        var v = PresentacionCintaConexion.Para(resumen);
        var textoColor = Color.FromArgb(v.ColorTexto);
        _icono.Text = v.Icono;
        _icono.TextColor = textoColor;
        _texto.Text = v.Texto;
        _texto.TextColor = textoColor;
        _borde.BackgroundColor = Color.FromArgb(v.ColorFondo);
        // Anuncio accesible: el lector de pantalla lee el estado completo, no sólo el color.
        SemanticProperties.SetDescription(this, $"Estado de sincronización: {v.Texto}");
        // H-14 (auditoría UX, región en vivo): al CAMBIAR el estado (no en el render inicial) se anuncia por el
        // lector de pantalla, para que el agente —que en terreno suele no estar mirando la pantalla— se entere de
        // "sin conexión", "sincronizando", "pendiente", etc. sin tener que enfocar la cinta.
        if (anunciar)
        {
            try { SemanticScreenReader.Default.Announce(v.Texto); } catch { /* sin lector activo: no pasa nada */ }
        }
    }
}
