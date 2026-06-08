using System.Globalization;
using GeoVial.CapturaCampo;
using GeoVial.Revision;
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
	private readonly ClienteRevisionHttp _revision;
	private bool _mapaCargado;

	// Relevamiento destino conocido (se obtiene online y se recuerda para poder capturar sin conexión).
	private static Guid? _relevamientoConocido;

	private CapturarObservacionRequest? _peticion;
	private byte[]? _bytes;
	private string _nombreArchivo = "foto.jpg";
	private decimal? _latManual;
	private decimal? _lonManual;

	public CapturaPage(ServicioSesion sesion, ArmadorCapturaCampo armador, ArmadorUbicacionManual ubicador, IColaCapturas cola, MotorCapturas motor, IMarcadorCaptura marcadorCaptura, MonitorSincronizacion monitor, ClienteRevisionHttp revision)
	{
		InitializeComponent();
		_sesion = sesion;
		_armador = armador;
		_ubicador = ubicador;
		_cola = cola;
		_motor = motor;
		_marcadorCaptura = marcadorCaptura;
		_revision = revision;
		Cinta.Vincular(monitor); // H-05: cinta de estado de conexión persistente

#if ANDROID
		// H-01/H-03: el mapa de la captura cachea las teselas de OSM en disco (offline parcial, S38) e intercepta
		// el esquema centinela del punto tocado (evolución de H-01): el client reemplaza al de MAUI, así que la
		// intercepción va acá, no por el evento Navigating.
		MapaWeb.HandlerChanged += (_, _) =>
		{
			if (MapaWeb.Handler?.PlatformView is Android.Webkit.WebView nativo)
			{
				var carpeta = Path.Combine(FileSystem.CacheDirectory, "teselas");
				nativo.SetWebViewClient(new MapaWebViewClient(new CacheTeselasDisco(carpeta),
					url => MainThread.BeginInvokeOnMainThread(() => OnPuntoMapa(url))));
			}
		};
#endif

		// US-16/F-M-12: subir las capturas encoladas cuando haya conexión.
		ToolbarItems.Add(new ToolbarItem("Sincronizar capturas", null, async () => await DrenarAsync("Sincronización de capturas.")));
		// H-02/H-03: centrar el mapa de la captura en la posición del agente.
		ToolbarItems.Add(new ToolbarItem("📍 Mi ubicación", null, async () => await CentrarEnMiUbicacionAsync()));
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		EstadoLbl.Text = $"Capturas pendientes de subir: {await _cola.PendientesAsync()}.";
		await CargarMapaAsync();
	}

	// H-01/H-03: arma el mapa de la captura con los marcadores del relevamiento activo (pines), una sola vez.
	private async Task CargarMapaAsync()
	{
		if (_mapaCargado)
		{
			return;
		}

		try
		{
			if (await _sesion.RelevamientoActivoAsync() is not { } relevamientoId)
			{
				return; // sin relevamiento activo todavía; se reintenta en el próximo OnAppearing
			}

			var revision = await _revision.ObtenerAsync(relevamientoId);
			if (revision is null)
			{
				return;
			}

			MapaWeb.Source = new HtmlWebViewSource { Html = MapaCapturaHtml.Construir(new VistaMapa(revision.Marcadores)) };
			_mapaCargado = true;
		}
		catch
		{
			// El mapa es contexto: si no carga (sin conexión la 1ª vez), la captura sigue funcionando igual.
		}
	}

	// H-02/H-03: obtiene el GPS (pidiendo permiso) y recentra el mapa de la captura en la posición del agente.
	private async Task CentrarEnMiUbicacionAsync()
	{
		var coordenada = await UbicacionDispositivo.ObtenerAsync();
		if (coordenada is null)
		{
			GeorrefLbl.Text = "No se pudo obtener tu ubicación. Activá el GPS y el permiso de ubicación.";
			return;
		}

		await MapaWeb.EvaluateJavaScriptAsync(
			ScriptUbicacionDispositivo.Centrar((double)coordenada.Latitud, (double)coordenada.Longitud));
	}

	// Evolución de H-01: el agente fijó la coordenada de la captura tocando (o arrastrando el pin) el mapa embebido;
	// el HTML avisa por geovial-ubicar://place?lat=..&lon=.. y acá se fija como coordenada de la captura.
	private void OnPuntoMapa(string url)
	{
		if (ParseadorMensajeUbicacion.Intentar(url) is { } punto)
		{
			FijarCoordenada(punto.Latitud, punto.Longitud);
		}
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
				// US-13/CU-05 (evolución H-01): la foto no trae coordenada. Lo más cómodo es tocar el mapa de
				// arriba para fijar el punto; si no, ubicarla a mano abajo. Sin coordenada, irá a la bandeja.
				UbicacionPanel.IsVisible = true;
				GeorrefLbl.Text = "La foto no trae ubicación: tocá el mapa de arriba para fijar el punto (o ubicala a mano abajo), o encolala para la bandeja.";
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

		FijarCoordenada(lat, lon);
	}

	// S56: elegir el punto sobre el mapa (mover el marcador) en vez de tipear las coordenadas (US-13, CU-05).
	private async void OnElegirEnMapa(object? sender, EventArgs e)
	{
		try
		{
			var mapa = new MapaSeleccionPuntoPage(VistaMapa.CentroPorDefectoLat, VistaMapa.CentroPorDefectoLon);
			await Navigation.PushModalAsync(new NavigationPage(mapa));
			if (await mapa.PuntoElegido is { } punto)
			{
				FijarCoordenada(punto.Latitud, punto.Longitud);
			}
		}
		catch (Exception ex)
		{
			EstadoLbl.Text = $"No se pudo abrir el mapa: {ex.Message}";
		}
	}

	// Valida y deja lista la coordenada (del mapa o manual) para que «Enviar captura» la use.
	private void FijarCoordenada(decimal lat, decimal lon)
	{
		if (_ubicador.Armar(lat, lon) is null)
		{
			EstadoLbl.Text = "Coordenada fuera de rango (lat -90..90, lon -180..180).";
			return;
		}

		_latManual = lat;
		_lonManual = lon;
		LatEntry.Text = lat.ToString("0.#####", CultureInfo.InvariantCulture);
		LonEntry.Text = lon.ToString("0.#####", CultureInfo.InvariantCulture);
		UbicacionPanel.IsVisible = false;
		GeorrefLbl.Text = $"Punto listo: {lat:0.#####}, {lon:0.#####}. Tocá «Enviar captura».";
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
