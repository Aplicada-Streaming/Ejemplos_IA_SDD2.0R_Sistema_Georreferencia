using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GeoVial.CapturaCampo;
using GeoVial.Shared;

namespace GeoVial.Mobile;

public partial class CapturaPage : ContentPage
{
	private readonly HttpClient _http;
	private readonly ArmadorCapturaCampo _armador;
	private readonly ArmadorUbicacionManual _ubicador;

	private CapturarObservacionRequest? _peticion;
	private byte[]? _bytes;
	private string _nombreArchivo = "foto.jpg";
	private Guid? _observacionId;

	public CapturaPage(HttpClient http, ArmadorCapturaCampo armador, ArmadorUbicacionManual ubicador)
	{
		InitializeComponent();
		_http = http;
		_armador = armador;
		_ubicador = ubicador;
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
			_bytes = ms.ToArray();
			_nombreArchivo = foto.FileName;

			var r = _armador.Armar(_bytes, foto.FileName);
			_peticion = r.Peticion;
			EnviarBtn.IsEnabled = true;
			UbicacionPanel.IsVisible = false;
			GeorrefLbl.Text = r.Georreferenciada
				? $"Georreferenciada por EXIF: {r.Peticion.LatitudExif:0.#####}, {r.Peticion.LongitudExif:0.#####}"
				: "La foto no trae ubicación: irá a la bandeja sin georreferenciar (podés ubicarla a mano abajo).";
		}
		catch (Exception ex)
		{
			GeorrefLbl.Text = $"No se pudo leer la foto: {ex.Message}";
		}
	}

	private async void OnEnviar(object? sender, EventArgs e)
	{
		if (_peticion is null || _bytes is null)
		{
			return;
		}

		try
		{
			EstadoLbl.Text = "Iniciando sesión…";
			if (await AutenticarYElegirRelevamientoAsync() is not { } relevamientoId)
			{
				return;
			}

			var resp = await _http.PostAsJsonAsync($"api/v1/relevamientos/{relevamientoId}/observaciones", _peticion);
			if (!resp.IsSuccessStatusCode)
			{
				EstadoLbl.Text = $"El backend rechazó la captura: {(int)resp.StatusCode}.";
				return;
			}

			var capt = await resp.Content.ReadFromJsonAsync<CapturaResponse>();
			_observacionId = capt!.ObservacionId;

			// BT-20 / ADR-08: encadenar la subida del binario de la foto al alojamiento.
			using var contenido = ConstructorContenidoMultipart.Construir(_nombreArchivo, _bytes);
			var subida = await _http.PostAsync($"api/v1/fotos/{capt.FotoId}/contenido", contenido);
			var aviso = subida.IsSuccessStatusCode ? "imagen subida" : $"imagen NO subida ({(int)subida.StatusCode})";

			if (capt.SinGeorreferenciar)
			{
				EstadoLbl.Text = $"Observación en la bandeja sin georreferenciar · {aviso}. Ubicala a mano abajo.";
				UbicacionPanel.IsVisible = true;
			}
			else
			{
				EstadoLbl.Text = $"Observación georreferenciada y asociada a su marcador · {aviso}.";
				UbicacionPanel.IsVisible = false;
			}
		}
		catch (Exception ex)
		{
			EstadoLbl.Text = $"No se pudo enviar la captura: {ex.Message}";
		}
	}

	private async void OnUbicar(object? sender, EventArgs e)
	{
		if (_observacionId is not { } observacionId)
		{
			return;
		}

		if (!decimal.TryParse(LatEntry.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var lat) ||
			!decimal.TryParse(LonEntry.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var lon))
		{
			EstadoLbl.Text = "Latitud/longitud inválidas.";
			return;
		}

		var peticion = _ubicador.Armar(lat, lon);
		if (peticion is null)
		{
			EstadoLbl.Text = "Coordenada fuera de rango (lat -90..90, lon -180..180).";
			return;
		}

		try
		{
			var resp = await _http.PostAsJsonAsync($"api/v1/observaciones/{observacionId}/ubicacion", peticion);
			EstadoLbl.Text = resp.IsSuccessStatusCode
				? "Punto ubicado: la observación salió de la bandeja y se reagrupó por radio."
				: $"No se pudo ubicar el punto: {(int)resp.StatusCode}.";
			if (resp.IsSuccessStatusCode)
			{
				UbicacionPanel.IsVisible = false;
			}
		}
		catch (Exception ex)
		{
			EstadoLbl.Text = $"No se pudo ubicar el punto: {ex.Message}";
		}
	}

	private async Task<Guid?> AutenticarYElegirRelevamientoAsync()
	{
		var login = await _http.PostAsJsonAsync("api/v1/auth/login", new { nombreUsuario = "raiz", clave = "GeoVial.Raiz.2026" });
		login.EnsureSuccessStatusCode();
		var token = await login.Content.ReadFromJsonAsync<TokenDto>();
		_http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.AccessToken);

		var relevamientos = await _http.GetFromJsonAsync<List<RelevamientoDto>>("api/v1/relevamientos");
		if (relevamientos is null || relevamientos.Count == 0)
		{
			EstadoLbl.Text = "Conexión OK, pero no hay un relevamiento destino en el backend.";
			return null;
		}

		return relevamientos[0].RelevamientoId;
	}

	private sealed record TokenDto(string AccessToken);

	private sealed record RelevamientoDto(Guid RelevamientoId);
}
