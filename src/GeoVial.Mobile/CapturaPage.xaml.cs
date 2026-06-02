using System.Net.Http.Headers;
using System.Net.Http.Json;
using GeoVial.CapturaCampo;
using GeoVial.Shared;

namespace GeoVial.Mobile;

public partial class CapturaPage : ContentPage
{
	private readonly HttpClient _http;
	private readonly ArmadorCapturaCampo _armador;

	private CapturarObservacionRequest? _peticion;

	public CapturaPage(HttpClient http, ArmadorCapturaCampo armador)
	{
		InitializeComponent();
		_http = http;
		_armador = armador;
	}

	private async void OnElegirFoto(object? sender, EventArgs e)
	{
		var fotos = await MediaPicker.Default.PickPhotosAsync();
		await ProcesarAsync(fotos?.FirstOrDefault());
	}

	private async void OnTomarFoto(object? sender, EventArgs e) => await ProcesarAsync(await MediaPicker.Default.CapturePhotoAsync());

	private async Task ProcesarAsync(FileResult? foto)
	{
		try
		{
			if (foto is null)
			{
				return;
			}

			using var stream = await foto.OpenReadAsync();
			using var ms = new MemoryStream();
			await stream.CopyToAsync(ms);
			var bytes = ms.ToArray();

			var r = _armador.Armar(bytes, foto.FileName);
			_peticion = r.Peticion;
			EnviarBtn.IsEnabled = true;
			GeorrefLbl.Text = r.Georreferenciada
				? $"Georreferenciada por EXIF: {r.Peticion.LatitudExif:0.#####}, {r.Peticion.LongitudExif:0.#####}"
				: "La foto no trae ubicación: irá a la bandeja sin georreferenciar (ubicación manual pendiente).";
		}
		catch (Exception ex)
		{
			GeorrefLbl.Text = $"No se pudo leer la foto: {ex.Message}";
		}
	}

	private async void OnEnviar(object? sender, EventArgs e)
	{
		if (_peticion is null)
		{
			return;
		}

		try
		{
			EstadoLbl.Text = "Iniciando sesión…";
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
			var resp = await _http.PostAsJsonAsync($"api/v1/relevamientos/{relevamientoId}/observaciones", _peticion);
			if (!resp.IsSuccessStatusCode)
			{
				EstadoLbl.Text = $"El backend rechazó la captura: {(int)resp.StatusCode}.";
				return;
			}

			var capt = await resp.Content.ReadFromJsonAsync<CapturaRespDto>();
			EstadoLbl.Text = capt!.SinGeorreferenciar
				? $"Observación {capt.ObservacionId:N} creada en la bandeja sin georreferenciar."
				: $"Observación {capt.ObservacionId:N} georreferenciada y asociada a su marcador.";
		}
		catch (Exception ex)
		{
			EstadoLbl.Text = $"No se pudo enviar la captura: {ex.Message}";
		}
	}

	private sealed record TokenDto(string AccessToken);

	private sealed record RelevamientoDto(Guid RelevamientoId);

	private sealed record CapturaRespDto(Guid ObservacionId, Guid? MarcadorId, bool SinGeorreferenciar);
}
