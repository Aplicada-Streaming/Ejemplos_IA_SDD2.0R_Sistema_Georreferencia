using GeoVial.Sync;
using Microsoft.Extensions.DependencyInjection;

namespace GeoVial.Mobile;

public partial class MainPage : ContentPage
{
	private readonly ServicioSesion _sesion;
	private readonly ColectorOffline _colector;
	private readonly IChangeQueue _cola;
	private readonly ISyncEngine _motor;

	public MainPage(ServicioSesion sesion, ColectorOffline colector, IChangeQueue cola, ISyncEngine motor)
	{
		InitializeComponent();
		_sesion = sesion;
		_colector = colector;
		_cola = cola;
		_motor = motor;

		// Cerrar sesión (US-40): limpia el token del HttpClient compartido y vuelve al login.
		ToolbarItems.Add(new ToolbarItem("Cerrar sesión", null, CerrarSesion));

		_ = ActualizarPendientesAsync();
	}

	private async Task ActualizarPendientesAsync()
	{
		var n = await _cola.PendingCountAsync();
		PendientesLbl.Text = $"Pendientes en cola: {n}";
	}

	private async void OnCapturar(object? sender, EventArgs e)
	{
		var obs = new ObservacionCapturada(
			Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), $"Observación {DateTime.Now:HH:mm:ss}", DateTime.UtcNow);
		try
		{
			await _colector.RecolectarAsync(obs);
			EstadoLbl.Text = "Observación capturada y encolada localmente (offline).";
		}
		catch (AlmacenamientoLocalInsuficienteException)
		{
			EstadoLbl.Text = "Sin espacio de almacenamiento local; se conserva lo ya guardado.";
		}

		await ActualizarPendientesAsync();
	}

	private async void OnSincronizar(object? sender, EventArgs e)
	{
		try
		{
			// La sesión ya está iniciada (el token se asentó en el HttpClient compartido al loguearse).
			EstadoLbl.Text = "Buscando relevamiento destino…";
			if (await _sesion.PrimerRelevamientoAsync() is not { } relevamientoId)
			{
				EstadoLbl.Text = "Conexión OK, pero no hay un relevamiento destino en el backend.";
				return;
			}

			var r = await _motor.SynchronizeAsync(relevamientoId);
			EstadoLbl.Text = $"Sincronización OK contra el backend.\nConfirmados: {r.Confirmed.Count} · Conflictos: {r.Conflicts.Count} · Actualizaciones: {r.Updates.Count}";
		}
		catch (Exception ex)
		{
			EstadoLbl.Text = $"No se pudo sincronizar: {ex.Message}";
		}

		await ActualizarPendientesAsync();
	}

	// Cierra la sesión y reemplaza las solapas por la pantalla de login.
	private void CerrarSesion()
	{
		_sesion.Salir();
		Application.Current!.Windows[0].Page =
			IPlatformApplication.Current!.Services.GetRequiredService<LoginPage>();
	}
}
