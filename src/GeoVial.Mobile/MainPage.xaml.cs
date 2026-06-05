using GeoVial.Sync;
using Microsoft.Extensions.DependencyInjection;

namespace GeoVial.Mobile;

public partial class MainPage : ContentPage
{
	private readonly ServicioSesion _sesion;
	private readonly SeguridadDispositivo _seguridad;
	private readonly ColectorOffline _colector;
	private readonly IChangeQueue _cola;
	private readonly ISyncEngine _motor;
	private readonly MonitorSincronizacion _estadoSync;
	private readonly CoordinadorAutoSync _autoSync;

	private List<RelevamientoResumen> _relevamientos = new();

	public MainPage(ServicioSesion sesion, SeguridadDispositivo seguridad, ColectorOffline colector, IChangeQueue cola, ISyncEngine motor, MonitorSincronizacion estadoSync, CoordinadorAutoSync autoSync)
	{
		InitializeComponent();
		_sesion = sesion;
		_seguridad = seguridad;
		_colector = colector;
		_cola = cola;
		_motor = motor;
		_estadoSync = estadoSync;
		_autoSync = autoSync;

		// Cerrar sesión (US-40): limpia el token del HttpClient compartido y vuelve al login.
		ToolbarItems.Add(new ToolbarItem("Cerrar sesión", null, CerrarSesion));
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		// Suscripciones acotadas a la vida visible de la página (evita fugas/duplicados al navegar entre solapas):
		// el indicador (S48) refleja cambios de estado; el evento de fin de auto-sync (S49) lo refresca en vivo.
		_estadoSync.Cambiado += OnEstadoSyncCambiado;
		_autoSync.SincronizacionCompletada += OnAutoSyncCompletada;

		await CargarRelevamientosAsync();
		await ActualizarPendientesAsync();
		await _estadoSync.RefrescarAsync();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		_estadoSync.Cambiado -= OnEstadoSyncCambiado;
		_autoSync.SincronizacionCompletada -= OnAutoSyncCompletada;
	}

	private void OnEstadoSyncCambiado(object? sender, ResumenSincronizacion resumen) =>
		MainThread.BeginInvokeOnMainThread(() => SyncEstadoLbl.Text = resumen.Texto);

	// S49: al terminar un auto-sync (p. ej. al recuperar señal), refresca el indicador en vivo y avisa de
	// conflictos sin que el agente toque la pantalla. Cierra la salvedad de S48.
	private void OnAutoSyncCompletada(object? sender, ResultadoAutoSync r) =>
		MainThread.BeginInvokeOnMainThread(async () =>
		{
			MostrarAvisoConflictos(r.Conflictos);
			await _estadoSync.RefrescarAsync(r.HuboError);
		});

	private void MostrarAvisoConflictos(int conflictos)
	{
		AvisoConflictosLbl.IsVisible = conflictos > 0;
		if (conflictos > 0)
		{
			AvisoConflictosLbl.Text = conflictos == 1
				? "⚠ 1 conflicto detectado — revisalo en la web/revisión."
				: $"⚠ {conflictos} conflictos detectados — revisalos en la web/revisión.";
		}
	}

	// F-M-04/05: trae los relevamientos accesibles, marca los asignados y deja elegido el activo.
	private async Task CargarRelevamientosAsync()
	{
		try
		{
			_relevamientos = (await _sesion.ListarRelevamientosAsync()).ToList();
			RelevamientoPicker.ItemsSource = _relevamientos
				.Select(r => $"{r.IdentificacionObra} · {Estado(r.Estado)}{(r.Asignado ? " · ✓ asignado" : "")}")
				.ToList();

			var activo = SelectorRelevamientos.Activo(_relevamientos, _sesion.RelevamientoActivoId);
			var idx = _relevamientos.FindIndex(r => r.RelevamientoId == activo);
			if (idx >= 0)
			{
				RelevamientoPicker.SelectedIndex = idx; // dispara OnRelevamientoSeleccionado → fija el activo
			}
		}
		catch (Exception ex)
		{
			EstadoLbl.Text = $"No se pudieron cargar los relevamientos: {ex.Message}";
		}
	}

	private void OnRelevamientoSeleccionado(object? sender, EventArgs e)
	{
		if (RelevamientoPicker.SelectedIndex is var i && i >= 0 && i < _relevamientos.Count)
		{
			_sesion.SeleccionarRelevamiento(_relevamientos[i].RelevamientoId);
		}
	}

	private static string Estado(int estado) => estado switch
	{
		1 => "recolección",
		2 => "revisión",
		3 => "cerrado",
		_ => "?",
	};

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
		await _estadoSync.RefrescarAsync(); // capturar agrega un pendiente: refleja el nuevo estado
	}

	private async void OnSincronizar(object? sender, EventArgs e)
	{
		var huboError = false;
		try
		{
			// La sesión ya está iniciada (el token se asentó en el HttpClient compartido al loguearse).
			EstadoLbl.Text = "Sincronizando el relevamiento activo…";
			_estadoSync.MarcarSincronizando();
			if (await _sesion.RelevamientoActivoAsync() is not { } relevamientoId)
			{
				EstadoLbl.Text = "Conexión OK, pero no hay un relevamiento destino. Elegí uno arriba.";
				return;
			}

			var r = await _motor.SynchronizeAsync(relevamientoId);
			EstadoLbl.Text = $"Sincronización OK contra el backend.\nConfirmados: {r.Confirmed.Count} · Conflictos: {r.Conflicts.Count} · Actualizaciones: {r.Updates.Count}";
			MostrarAvisoConflictos(r.Conflicts.Count); // S49: también avisa en la sync manual
		}
		catch (Exception ex)
		{
			huboError = true;
			EstadoLbl.Text = $"No se pudo sincronizar: {ex.Message}";
		}

		await ActualizarPendientesAsync();
		await _estadoSync.RefrescarAsync(huboError); // refleja si quedó al día o con pendientes/error
	}

	// Cierra la sesión (explícita): limpia el token y olvida el método de seguridad recordado (RN-06),
	// así el próximo ingreso es con usuario y clave (el reingreso en terreno es para reabrir, no para logout).
	private void CerrarSesion()
	{
		_sesion.Salir();
		_seguridad.Olvidar();
		Application.Current!.Windows[0].Page =
			IPlatformApplication.Current!.Services.GetRequiredService<LoginPage>();
	}
}
