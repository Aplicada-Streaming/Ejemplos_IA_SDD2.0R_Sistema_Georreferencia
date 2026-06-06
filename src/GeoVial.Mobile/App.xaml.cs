using GeoVial.Sync;
using Microsoft.Extensions.DependencyInjection;

namespace GeoVial.Mobile;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		// La app arranca en el login (US-40): hasta no autenticar no se ven las solapas.
		// LoginPage reemplaza la página de la ventana por el AppShell tras un login exitoso.
		var servicios = IPlatformApplication.Current?.Services
			?? throw new InvalidOperationException("El contenedor de servicios no está disponible.");

		// F-M-14: instanciar el coordinador de auto-sync para que se suscriba al monitor de conectividad
		// y sincronice (relevamiento activo + capturas encoladas) al recuperar señal, sin acción del usuario.
		servicios.GetRequiredService<CoordinadorAutoSync>();

		// S55: la app arranca SIEMPRE en LoginPage; ahí (con una Activity viva, necesaria para el método de
		// seguridad del teléfono) se decide el arranque: entrar (vuelta de cámara), pedir el patrón/PIN para
		// desbloquear la sesión persistida, o pedir usuario y clave. Antes (S54) se restauraba en silencio, lo
		// que dejaba entrar sin autenticar; ahora el reingreso exige el método del dispositivo.
		return new Window(servicios.GetRequiredService<LoginPage>());
	}
}