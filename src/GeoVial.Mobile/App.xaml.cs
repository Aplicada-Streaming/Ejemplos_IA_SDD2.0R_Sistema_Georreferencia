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

		// Si el proceso sobrevivió y la sesión sigue activa (p. ej. al volver de la cámara o tras
		// recrear la actividad por rotación), se va directo a las solapas en vez de rebotar al login.
		var sesion = servicios.GetRequiredService<ServicioSesion>();
		Page inicial = sesion.Autenticado ? new AppShell() : servicios.GetRequiredService<LoginPage>();
		return new Window(inicial);
	}
}