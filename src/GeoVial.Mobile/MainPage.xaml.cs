using System.Net.Http.Headers;
using System.Net.Http.Json;
using GeoVial.Sync;

namespace GeoVial.Mobile;

public partial class MainPage : ContentPage
{
	private readonly HttpClient _http;
	private readonly ColectorOffline _colector;
	private readonly IChangeQueue _cola;
	private readonly ISyncEngine _motor;

	public MainPage(HttpClient http, ColectorOffline colector, IChangeQueue cola, ISyncEngine motor)
	{
		InitializeComponent();
		_http = http;
		_colector = colector;
		_cola = cola;
		_motor = motor;
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
			EstadoLbl.Text = "Iniciando sesión en el backend…";
			var login = await _http.PostAsJsonAsync("api/v1/auth/login", new { nombreUsuario = "raiz", clave = "GeoVial.Raiz.2026" });
			login.EnsureSuccessStatusCode();
			var token = await login.Content.ReadFromJsonAsync<TokenDto>();
			_http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.AccessToken);

			var relevamientos = await _http.GetFromJsonAsync<List<RelevamientoDto>>("api/v1/relevamientos");
			if (relevamientos is null || relevamientos.Count == 0)
			{
				EstadoLbl.Text = "Conexión OK, pero no hay un relevamiento destino en el backend.";
				return;
			}

			var relevamientoId = relevamientos[0].RelevamientoId;
			var r = await _motor.SynchronizeAsync(relevamientoId);
			EstadoLbl.Text = $"Sincronización OK contra el backend.\nConfirmados: {r.Confirmed.Count} · Conflictos: {r.Conflicts.Count} · Actualizaciones: {r.Updates.Count}";
		}
		catch (Exception ex)
		{
			EstadoLbl.Text = $"No se pudo sincronizar: {ex.Message}";
		}

		await ActualizarPendientesAsync();
	}

	private sealed record TokenDto(string AccessToken);

	private sealed record RelevamientoDto(Guid RelevamientoId);
}
