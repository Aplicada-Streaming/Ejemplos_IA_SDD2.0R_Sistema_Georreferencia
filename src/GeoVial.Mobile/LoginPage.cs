using GeoVial.Sync;

namespace GeoVial.Mobile;

/// <summary>
/// Pantalla de inicio de sesión (US-40). Es la primera pantalla de la app: hasta no
/// autenticar no se muestran las solapas. Apoya el login en <see cref="ServicioSesion"/>,
/// que asienta el token en el HttpClient compartido; tras un login exitoso reemplaza la
/// página de la ventana por el <see cref="AppShell"/>. Página por código (sin XAML) para
/// no acoplar la cáscara MAUI a más markup. Fuera de CI.
/// </summary>
public sealed class LoginPage : ContentPage
{
    private readonly ServicioSesion _sesion;
    private readonly Entry _usuario;
    private readonly Entry _clave;
    private readonly Button _ingresar;
    private readonly Label _estado;
    private readonly ActivityIndicator _spinner;

    public LoginPage(ServicioSesion sesion)
    {
        _sesion = sesion;
        Title = "GeoVial";

        // Usuario por defecto de demo precargado; la clave se ingresa siempre.
        _usuario = new Entry { Placeholder = "Usuario", Text = "raiz", ReturnType = ReturnType.Next };
        _clave = new Entry { Placeholder = "Clave", IsPassword = true, ReturnType = ReturnType.Go };
        _ingresar = new Button { Text = "Ingresar" };
        _ingresar.Clicked += OnIngresar;
        _clave.Completed += OnIngresar;
        _estado = new Label { TextColor = Color.FromArgb("#b00020"), IsVisible = false };
        _spinner = new ActivityIndicator { IsRunning = false, IsVisible = false };

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 24,
                Spacing = 14,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label { Text = "GeoVial", FontSize = 28, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center },
                    new Label { Text = "Iniciá sesión para sincronizar, capturar y revisar.", FontSize = 13, TextColor = Color.FromArgb("#666666"), HorizontalOptions = LayoutOptions.Center },
                    _usuario,
                    _clave,
                    _ingresar,
                    _spinner,
                    _estado,
                },
            },
        };
    }

    private async void OnIngresar(object? sender, EventArgs e)
    {
        _estado.IsVisible = false;
        _ingresar.IsEnabled = false;
        _spinner.IsVisible = _spinner.IsRunning = true;
        try
        {
            var r = await _sesion.IngresarAsync(_usuario.Text, _clave.Text);
            if (r.Exito)
            {
                // El token ya quedó asentado en el HttpClient compartido: las solapas no re-loguean.
                Application.Current!.Windows[0].Page = new AppShell();
                return;
            }

            _estado.Text = r.Mensaje;
            _estado.IsVisible = true;
        }
        finally
        {
            _ingresar.IsEnabled = true;
            _spinner.IsVisible = _spinner.IsRunning = false;
        }
    }
}
