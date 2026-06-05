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

		// S54: si el SO mató el proceso (típico al abrir la cámara en equipos con poca RAM), la sesión en
		// memoria se perdió; se restaura desde el almacén seguro para volver a las solapas en vez de rebotar
		// al login. Se restaura en un hilo del pool para no bloquear la UI ni arriesgar un deadlock al arrancar.
		var sesion = servicios.GetRequiredService<ServicioSesion>();
		bool autenticado;
		try
		{
			autenticado = Task.Run(() => sesion.RestaurarAsync()).GetAwaiter().GetResult();
		}
		catch
		{
			autenticado = sesion.Autenticado;
		}

		Page inicial = autenticado ? new AppShell() : servicios.GetRequiredService<LoginPage>();
		return new Window(inicial);
	}
}