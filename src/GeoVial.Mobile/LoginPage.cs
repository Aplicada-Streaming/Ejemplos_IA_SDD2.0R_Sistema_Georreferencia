using GeoVial.Sync;

namespace GeoVial.Mobile;

/// <summary>
/// Pantalla de acceso (US-40, CU-02, RN-06). Primera pantalla. Decide el arranque (S55): si hay sesión
/// persistida y el teléfono tiene método de seguridad, pide el patrón/huella/PIN para volver a la sesión
/// (sin clave); si se vuelve de la cámara, entra directo; si no, pide usuario y clave. "Cerrar sesión" es
/// logout total (vuelve a usuario y clave). El login con conexión configura el método y habilita el offline
/// (RN-06). Página por código (sin XAML). Fuera de CI.
/// </summary>
public sealed class LoginPage : ContentPage
{
    private readonly ServicioSesion _sesion;
    private readonly SeguridadDispositivo _seguridad;
    private readonly CoordinadorArranque _arranque;
    private readonly IAutenticadorBiometrico _biometrico;
    private readonly Entry _usuario;
    private readonly Entry _clave;
    private readonly Button _ingresar;
    private readonly Button _desbloquear;
    private readonly Label _estado;
    private readonly ActivityIndicator _spinner;
    private bool _decisionTomada;

    public LoginPage(ServicioSesion sesion, SeguridadDispositivo seguridad, CoordinadorArranque arranque, IAutenticadorBiometrico biometrico)
    {
        _sesion = sesion;
        _seguridad = seguridad;
        _arranque = arranque;
        _biometrico = biometrico;
        Title = "GeoVial";

        _usuario = new Entry { Placeholder = "Usuario", ReturnType = ReturnType.Next };
        _clave = new Entry { Placeholder = "Clave", IsPassword = true, ReturnType = ReturnType.Go };
        // Ojito: mostrar/ocultar la clave para evitar errores de tipeo en el teléfono (S54).
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
        // Desbloqueo con el método del teléfono: visible sólo cuando la sesión quedó bloqueada (S55).
        _desbloquear = new Button { Text = "Desbloquear con el patrón / huella", IsVisible = false, BackgroundColor = Color.FromArgb("#2e7d32") };
        _desbloquear.Clicked += OnDesbloquear;
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
                    _desbloquear,
                    _spinner,
                    _estado,
                },
            },
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_decisionTomada)
        {
            return; // no re-disparar el método al volver a aparecer la página
        }

        _decisionTomada = true;
        // Prefijar el usuario recordado (si lo hay) para el formulario de usuario/clave.
        var recordado = await _seguridad.UsuarioRecordadoAsync();
        if (!string.IsNullOrEmpty(recordado))
        {
            _usuario.Text = recordado;
        }

        try
        {
            await AplicarDecisionAsync(await _arranque.DecidirInicialAsync());
        }
        catch
        {
            MostrarFormulario(); // ante cualquier fallo, queda el acceso por usuario y clave
        }
    }

    // Ejecuta la acción que indica la política de arranque (S55).
    private async Task AplicarDecisionAsync(DecisionArranque decision)
    {
        switch (decision)
        {
            case DecisionArranque.Entrar:
                IrAlShell();
                break;
            case DecisionArranque.PedirBiometrico:
                await DesbloquearAsync();
                break;
            case DecisionArranque.Bloqueado:
                _estado.Text = "Sesión bloqueada. Desbloqueá con el patrón/huella o entrá con usuario y clave.";
                _estado.TextColor = Color.FromArgb("#b00020");
                _estado.IsVisible = true;
                _desbloquear.IsVisible = true;
                break;
            default: // PedirClave
                MostrarFormulario();
                break;
        }
    }

    private void MostrarFormulario()
    {
        _desbloquear.IsVisible = false;
        _estado.IsVisible = false;
    }

    private void IrAlShell() => Application.Current!.Windows[0].Page = new AppShell();

    private async Task DesbloquearAsync() =>
        await EjecutarAsync(async () =>
        {
            var decision = await _arranque.DesbloquearConBiometricoAsync();
            if (decision == DecisionArranque.Entrar)
            {
                IrAlShell();
                return null;
            }

            await AplicarDecisionAsync(decision); // Bloqueado → muestra el botón de reintento; PedirClave → formulario
            return null;
        });

    private async void OnDesbloquear(object? sender, EventArgs e) => await DesbloquearAsync();

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
            // teléfono tiene un método nativo (huella/rostro/patrón/PIN), porque el reingreso lo exige.
            // Best-effort: no bloquea el ingreso si falla.
            try
            {
                if (await _biometrico.HayMetodoDisponibleAsync()
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

            IrAlShell();
            return null;
        });
    }

    // Ejecuta una acción de autenticación mostrando spinner y, si devuelve un mensaje, el error.
    private async Task EjecutarAsync(Func<Task<string?>> accion)
    {
        _estado.IsVisible = false;
        _ingresar.IsEnabled = _desbloquear.IsEnabled = false;
        _spinner.IsVisible = _spinner.IsRunning = true;
        try
        {
            var error = await accion();
            if (error is not null)
            {
                _estado.Text = error;
                _estado.TextColor = Color.FromArgb("#b00020");
                _estado.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            _estado.Text = $"No se pudo completar el acceso: {ex.Message}";
            _estado.IsVisible = true;
        }
        finally
        {
            _ingresar.IsEnabled = _desbloquear.IsEnabled = true;
            _spinner.IsVisible = _spinner.IsRunning = false;
        }
    }
}
