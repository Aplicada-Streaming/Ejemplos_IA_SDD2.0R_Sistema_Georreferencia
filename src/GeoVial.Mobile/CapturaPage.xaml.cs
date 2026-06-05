using System.Globalization;
using GeoVial.CapturaCampo;
using GeoVial.Shared;
using GeoVial.Sync;

namespace GeoVial.Mobile;

public partial class CapturaPage : ContentPage
{
	private readonly ServicioSesion _sesion;
	private readonly ArmadorCapturaCampo _armador;
	private readonly ArmadorUbicacionManual _ubicador;
	private readonly IColaCapturas _cola;
	private readonly MotorCapturas _motor;
	private readonly IMarcadorCaptura _marcadorCaptura;

	// Relevamiento destino conocido (se obtiene online y se recuerda para poder capturar sin conexión).
	private static Guid? _relevamientoConocido;

	private CapturarObservacionRequest? _peticion;
	private byte[]? _bytes;
	private string _nombreArchivo = "foto.jpg";
	private decimal? _latManual;
	private decimal? _lonManual;

	public CapturaPage(ServicioSesion sesion, ArmadorCapturaCampo armador, ArmadorUbicacionManual ubicador, IColaCapturas cola, MotorCapturas motor, IMarcadorCaptura marcadorCaptura)
	{
		InitializeComponent();
		_sesion = sesion;
		_armador = armador;
		_ubicador = ubicador;
		_cola = cola;
		_motor = motor;
		_marcadorCaptura = marcadorCaptura;

		// US-16/F-M-12: subir las capturas encoladas cuando haya conexión.
		ToolbarItems.Add(new ToolbarItem("Sincronizar capturas", null, async () => await DrenarAsync("Sincronización de capturas.")));
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		EstadoLbl.Text = $"Capturas pendientes de subir: {await _cola.PendientesAsync()}.";
	}

	// US-40 (bug en dispositivo): elegir foto de la galería sin que un fallo tumbe la app.
	private async void OnElegirFoto(object? sender, EventArgs e)
	{
		try
		{
			var fotos = await MediaPicker.Default.PickPhotosAsync();
			await ProcesarAsync(fotos?.FirstOrDefault());
		}
		catch (Exception ex)
		{
			GeorrefLbl.Text = $"No se pudo abrir la galería: {ex.Message}";
		}
	}

	// US-40 (bug en dispositivo): tomar foto pedía permiso de cámara que no se solicitaba y la
	// llamada al MediaPicker estaba fuera de un try/catch, así que cualquier fallo cerraba la app.
	private async void OnTomarFoto(object? sender, EventArgs e)
	{
		try
		{
			if (!await AsegurarPermisoCamaraAsync())
			{
				GeorrefLbl.Text = "Se necesita permiso de cámara para tomar la foto. Habilitalo en Ajustes.";
				return;
			}

			if (!MediaPicker.Default.IsCaptureSupported)
			{
				GeorrefLbl.Text = "Este dispositivo no permite capturar fotos con la cámara.";
				return;
			}

			// S55: marcar "captura en curso" antes de abrir la cámara. Si el SO mata el proceso mientras la
			// cámara está en primer plano, al volver el arranque sabrá que es una vuelta de cámara y no re-pedirá
			// el método de seguridad (no rebota al login).
			await _marcadorCaptura.MarcarEnCursoAsync();
			try
			{
				var foto = await MediaPicker.Default.CapturePhotoAsync();
				await ProcesarAsync(foto);
			}
			finally
			{
				await _marcadorCaptura.LimpiarAsync();
			}
		}
		catch (Exception ex)
		{
			GeorrefLbl.Text = $"No se pudo tomar la foto: {ex.Message}";
		}
	}

	// Pide el permiso de cámara en runtime (Android 6+ lo exige aunque esté en el manifiesto).
	private static async Task<bool> AsegurarPermisoCamaraAsync()
	{
		var estado = await Permissions.CheckStatusAsync<Permissions.Camera>();
		if (estado != PermissionStatus.Granted)
		{
			estado = await Permissions.RequestAsync<Permissions.Camera>();
		}

		return estado == PermissionStatus.Granted;
	}

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
			_latManual = null;
			_lonManual = null;

			var r = _armador.Armar(_bytes, foto.FileName);
			_peticion = r.Peticion;
			EnviarBtn.IsEnabled = true;
			if (r.Georreferenciada)
			{
				UbicacionPanel.IsVisible = false;
				GeorrefLbl.Text = $"Georreferenciada por EXIF: {r.Peticion.LatitudExif:0.#####}, {r.Peticion.LongitudExif:0.#####}";
			}
			else
			{
				// US-13/CU-05: la foto no trae coordenada. Se puede ubicar a mano AHORA (antes de encolar),
				// así la captura viaja con su coordenada; si no, irá a la bandeja sin georreferenciar.
				UbicacionPanel.IsVisible = true;
				GeorrefLbl.Text = "La foto no trae ubicación: ubicala a mano abajo o encolala para la bandeja.";
			}
		}
		catch (Exception ex)
		{
			GeorrefLbl.Text = $"No se pudo leer la foto: {ex.Message}";
		}
	}

	// "Enviar captura": encola la captura localmente (funciona sin conexión) e intenta subirla en el acto.
	private async void OnEnviar(object? sender, EventArgs e)
	{
		if (_peticion is null || _bytes is null)
		{
			return;
		}

		var relevamientoId = await ResolverRelevamientoAsync();
		if (relevamientoId is null)
		{
			EstadoLbl.Text = "No se conoce el relevamiento destino. Conectate una vez para obtenerlo.";
			return;
		}

		var lat = _peticion.LatitudExif ?? _latManual;
		var lon = _peticion.LongitudExif ?? _lonManual;
		var captura = new CapturaPendiente(Guid.NewGuid(), relevamientoId.Value, _nombreArchivo, lat, lon, _bytes, DateTime.UtcNow);

		try
		{
			await _cola.EncolarAsync(captura);
		}
		catch (AlmacenamientoLocalInsuficienteException)
		{
			EstadoLbl.Text = "Sin espacio de almacenamiento local; se conserva lo ya guardado.";
			return;
		}

		// Limpia el estado de captura para la próxima.
		_peticion = null;
		_bytes = null;
		_latManual = null;
		_lonManual = null;
		EnviarBtn.IsEnabled = false;
		UbicacionPanel.IsVisible = false;
		GeorrefLbl.Text = "Sin foto seleccionada.";

		await DrenarAsync("Captura encolada.");
	}

	// Fija la coordenada manual en la captura (no la postea): se subirá junto con la captura encolada.
	private void OnUbicar(object? sender, EventArgs e)
	{
		if (!decimal.TryParse(LatEntry.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var lat) ||
			!decimal.TryParse(LonEntry.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var lon))
		{
			EstadoLbl.Text = "Latitud/longitud inválidas.";
			return;
		}

		if (_ubicador.Armar(lat, lon) is null)
		{
			EstadoLbl.Text = "Coordenada fuera de rango (lat -90..90, lon -180..180).";
			return;
		}

		_latManual = lat;
		_lonManual = lon;
		UbicacionPanel.IsVisible = false;
		GeorrefLbl.Text = $"Coordenada manual lista: {lat:0.#####}, {lon:0.#####}. Tocá «Enviar captura».";
	}

	// Drena la cola de capturas (subir lo encolado). Sin conexión, conserva lo pendiente sin perderlo.
	private async Task DrenarAsync(string prefijo)
	{
		try
		{
			var r = await _motor.SincronizarAsync();
			var pendientes = await _cola.PendientesAsync();
			EstadoLbl.Text = $"{prefijo} Subidas ahora: {r.Subidas.Count}. Pendientes: {pendientes}.";
		}
		catch (SyncInterruptedException)
		{
			var pendientes = await _cola.PendientesAsync();
			EstadoLbl.Text = $"{prefijo} Sin conexión: quedan {pendientes} captura(s) en cola; se subirán al reconectar.";
		}
		catch (Exception ex)
		{
			EstadoLbl.Text = $"{prefijo} No se pudieron subir las capturas: {ex.Message}";
		}
	}

	// Obtiene el relevamiento destino: online lo busca y lo recuerda; offline usa el último conocido.
	private async Task<Guid?> ResolverRelevamientoAsync()
	{
		try
		{
			if (await _sesion.RelevamientoActivoAsync() is { } id)
			{
				_relevamientoConocido = id;
				return id;
			}
		}
		catch
		{
			// sin conexión: se usa el último relevamiento conocido (si lo hay).
		}

		return _relevamientoConocido;
	}
}
