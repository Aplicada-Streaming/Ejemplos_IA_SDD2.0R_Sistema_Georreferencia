using GeoVial.Sync;

namespace GeoVial.Mobile;

/// <summary>
/// Pantalla de inicio de sesión (US-40) con método de seguridad y reingreso en terreno (RN-06, CU-02).
/// Es la primera pantalla: hasta no autenticar no se muestran las solapas. El login con conexión asienta el
/// token, configura el método de seguridad del teléfono y habilita el modo sin conexión; además recuerda el
/// usuario para el reingreso. Si hay un usuario recordado (p. ej. al reabrir la app en terreno), ofrece
/// "Reingreso en terreno" sin reescribir la clave (US-05). Página por código (sin XAML). Fuera de CI.
/// </summary>
public sealed class LoginPage : ContentPage
{
    private readonly ServicioSesion _sesion;
    private readonly SeguridadDispositivo _seguridad;
    private readonly CoordinadorReingreso _reingresoBiometrico;
    private readonly Entry _usuario;
    private readonly Entry _clave;
    private readonly Button _ingresar;
    private readonly Button _reingresar;
    private readonly Label _estado;
    private readonly ActivityIndicator _spinner;

    public LoginPage(ServicioSesion sesion, SeguridadDispositivo seguridad, CoordinadorReingreso reingresoBiometrico)
    {
        _sesion = sesion;
        _seguridad = seguridad;
        _reingresoBiometrico = reingresoBiometrico;
        Title = "GeoVial";

        _usuario = new Entry { Placeholder = "Usuario", Text = "raiz", ReturnType = ReturnType.Next };
        _clave = new Entry { Placeholder = "Clave", IsPassword = true, ReturnType = ReturnType.Go };
        // Ojito: mostrar/ocultar la clave para evitar errores de tipeo en el teléfono.
        var verClave = new Button { Text = "👁", WidthRequest = 52, BackgroundColor = Colors.Transparent, FontSize = 18 };
        verClave.Clicked += (_, _) =>
        {
            _clave.IsPassword = !_clave.IsPassword;
            verClave.Text = _clave.IsPassword ? "👁" : "🙈";
        };
        var filaClave = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };
        filaClave.Add(_clave, 0, 0);
        filaClave.Add(verClave, 1, 0);
        _ingresar = new Button { Text = "Ingresar" };
        _ingresar.Clicked += OnIngresar;
        _clave.Completed += OnIngresar;
        _reingresar = new Button { Text = "Reingreso en terreno (sin clave)", IsVisible = false, BackgroundColor = Color.FromArgb("#2e7d32") };
        _reingresar.Clicked += OnReingresar;
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
                    filaClave,
                    _ingresar,
                    _reingresar,
                    _spinner,
                    _estado,
                },
            },
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // RN-06: si hay un método de seguridad configurado (usuario recordado), ofrecer el reingreso en terreno.
        var recordado = await _seguridad.UsuarioRecordadoAsync();
        if (!string.IsNullOrEmpty(recordado))
        {
            _usuario.Text = recordado;
            _reingresar.Text = $"Reingreso en terreno como «{recordado}» (sin clave)";
            _reingresar.IsVisible = true;
        }
    }

    private async void OnIngresar(object? sender, EventArgs e)
    {
        await EjecutarAsync(async () =>
        {
            var r = await _sesion.IngresarAsync(_usuario.Text, _clave.Text);
            if (!r.Exito)
            {
                return r.Mensaje;
            }

            // RN-06: configurar el método de seguridad y habilitar el offline + recordar el usuario sólo si el
            // teléfono tiene un método nativo (huella/rostro/PIN), porque el reingreso ahora lo exige (S53).
            // Best-effort: no bloquea el ingreso si falla.
            try
            {
                if (await _reingresoBiometrico.HayMetodoDisponibleAsync()
                    && await _sesion.ConfigurarMetodoSeguridadAsync())
                {
                    await _sesion.HabilitarOfflineAsync();
                    await _seguridad.ConfigurarAsync(_usuario.Text?.Trim() ?? "");
                }
            }
            catch
            {
                // el método de seguridad es opcional para entrar; el reingreso quedará deshabilitado.
            }

            Application.Current!.Windows[0].Page = new AppShell();
            return null;
        });
    }

    private async void OnReingresar(object? sender, EventArgs e)
    {
        await EjecutarAsync(async () =>
        {
            // RN-06 (S53): el reingreso exige verificación biométrica nativa (huella/rostro/PIN) antes de
            // re-autenticar sin clave; el coordinador la pide y, sólo si tiene éxito, reingresa contra el backend.
            var usuario = await _seguridad.UsuarioRecordadoAsync();
            var r = await _reingresoBiometrico.ReingresarAsync(usuario);
            if (!r.Exito)
            {
                return r.Mensaje;
            }

            Application.Current!.Windows[0].Page = new AppShell();
            return null;
        });
    }

    // Ejecuta una acción de autenticación mostrando spinner y, si devuelve un mensaje, el error.
    private async Task EjecutarAsync(Func<Task<string?>> accion)
    {
        _estado.IsVisible = false;
        _ingresar.IsEnabled = _reingresar.IsEnabled = false;
        _spinner.IsVisible = _spinner.IsRunning = true;
        try
        {
            var error = await accion();
            if (error is not null)
            {
                _estado.Text = error;
                _estado.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            // Cualquier fallo inesperado (p. ej. SecureStorage en un dispositivo sin bloqueo) se muestra, no tumba la app.
            _estado.Text = $"No se pudo completar el acceso: {ex.Message}";
            _estado.IsVisible = true;
        }
        finally
        {
            _ingresar.IsEnabled = _reingresar.IsEnabled = true;
            _spinner.IsVisible = _spinner.IsRunning = false;
        }
    }
}
