using GeoVial.Revision;
using GeoVial.Sync;

namespace GeoVial.Mobile;

public partial class RevisionPage : ContentPage
{
	private readonly HttpClient _http;
	private readonly ServicioSesion _sesion;
	private readonly ClienteRevisionHttp _cliente;
	private readonly ClienteEdicionMarcador _editor;
	private readonly ArmadorFotoMarcador _armadorFoto;
	private readonly IColaCapturas _cola;
	private readonly MotorCapturas _motor;
	private readonly IMarcadorCaptura _marcadorCaptura;
	private readonly CacheFotos _cacheFotos = new();

	private NavegadorRevision? _nav;
	private Guid? _relevamientoId;
	private GeoVial.Shared.RevisionRelevamientoDto? _revision;

	public RevisionPage(
		HttpClient http,
		ServicioSesion sesion,
		ClienteRevisionHttp cliente,
		ClienteEdicionMarcador editor,
		ArmadorFotoMarcador armadorFoto,
		IColaCapturas cola,
		MotorCapturas motor,
		IMarcadorCaptura marcadorCaptura,
		MonitorSincronizacion monitor)
	{
		InitializeComponent();
		_http = http;
		_sesion = sesion;
		_cliente = cliente;
		_editor = editor;
		_armadorFoto = armadorFoto;
		_cola = cola;
		_motor = motor;
		_marcadorCaptura = marcadorCaptura;
		Cinta.Vincular(monitor); // H-05: cinta de estado de conexión persistente
	}

	private async void OnCargar(object? sender, EventArgs e)
	{
		try
		{
			MarcadorLbl.Text = "Cargando…";
			// La sesión ya está iniciada (token asentado en el HttpClient compartido al loguearse).
			if (await _sesion.RelevamientoActivoAsync() is not { } relevamientoId)
			{
				MarcadorLbl.Text = "No hay un relevamiento en el backend.";
				return;
			}

			_relevamientoId = relevamientoId;
			var revision = await _cliente.ObtenerAsync(relevamientoId);
			if (revision is null)
			{
				MarcadorLbl.Text = "No se pudo obtener la revisión.";
				return;
			}

			_revision = revision;
			_nav = new NavegadorRevision(revision);
			await RenderAsync();
		}
		catch (Exception ex)
		{
			MarcadorLbl.Text = $"Error al cargar: {ex.Message}";
		}
	}

	// Abre el mapa interactivo (Leaflet + OSM, sin clave) con los marcadores del relevamiento cargado.
	private async void OnVerMapa(object? sender, EventArgs e)
	{
		if (_revision is null)
		{
			MarcadorLbl.Text = "Cargá una revisión antes de ver el mapa.";
			return;
		}

		var html = MapaRevisionHtml.Construir(new VistaMapa(_revision.Marcadores));
		// En NavigationPage para que la barra (con "Cerrar") se muestre sobre el modal.
		var mapa = new MapaRevisionPage(html);
		await Navigation.PushModalAsync(new NavigationPage(mapa));
		// S55: si el agente tocó un pin, al cerrarse el mapa se abre ese marcador en el carrusel.
		if (await mapa.MarcadorElegido is { } marcadorId)
		{
			_nav?.IrAlMarcador(marcadorId);
			await RenderAsync();
		}
	}

	private async void OnSiguienteMarcador(object? sender, EventArgs e) { _nav?.SiguienteMarcador(); await RenderAsync(); }

	private async void OnAnteriorMarcador(object? sender, EventArgs e) { _nav?.AnteriorMarcador(); await RenderAsync(); }

	private async void OnSiguienteFoto(object? sender, EventArgs e) { _nav?.SiguienteFoto(); await RenderAsync(); }

	private async void OnAnteriorFoto(object? sender, EventArgs e) { _nav?.AnteriorFoto(); await RenderAsync(); }

	// H-04: carrusel deslizable. Swipe a la izquierda avanza (y cruza al marcador siguiente al terminar las fotos);
	// a la derecha retrocede (y cruza al marcador anterior, a su última foto). Complementa los botones.
	private async void OnSwipeFoto(object? sender, SwipedEventArgs e)
	{
		if (_nav is null)
		{
			return;
		}

		if (e.Direction == SwipeDirection.Left)
		{
			_nav.Avanzar();
		}
		else if (e.Direction == SwipeDirection.Right)
		{
			_nav.Retroceder();
		}

		await RenderAsync();
	}

	private async Task RenderAsync()
	{
		if (_nav is null || !_nav.HayMarcadores)
		{
			MarcadorLbl.Text = "El relevamiento no tiene marcadores.";
			FotoImg.Source = null;
			FotoLbl.Text = "";
			ComentariosLbl.Text = "";
			EdicionPanel.IsVisible = false;
			return;
		}

		EdicionPanel.IsVisible = true;

		var m = _nav.MarcadorActual!;
		var conflicto = m.EnConflicto ? " · ⚠ en conflicto" : "";
		MarcadorLbl.Text = $"Marcador {_nav.IndiceMarcador + 1}/{_nav.CantidadMarcadores}: {m.Latitud:0.#####}, {m.Longitud:0.#####}{conflicto}";

		var foto = _nav.FotoActual;
		if (foto is not null)
		{
			FotoLbl.Text = $"Foto {_nav.IndiceFoto + 1}/{m.Fotos.Count}: {foto.ReferenciaArchivo}";
			FotoImg.Source = await DescargarFotoAsync(foto.FotoId);
		}
		else
		{
			FotoLbl.Text = "Marcador sin fotos.";
			FotoImg.Source = null;
		}

		ComentariosLbl.Text = m.Comentarios.Count == 0
			? "Sin comentarios."
			: string.Join("\n", m.Comentarios.Select(c => $"• {c.Texto}"));
	}

	private async Task<ImageSource?> DescargarFotoAsync(Guid fotoId)
	{
		try
		{
			// Sirve la foto en foco desde la caché en memoria para no re-descargarla al navegar el carrusel.
			var bytes = _cacheFotos.Obtener(fotoId);
			if (bytes is null)
			{
				bytes = await _http.GetByteArrayAsync($"api/v1/fotos/{fotoId}/contenido");
				_cacheFotos.Guardar(fotoId, bytes);
			}

			return ImageSource.FromStream(() => new MemoryStream(bytes));
		}
		catch
		{
			return null; // el binario puede no estar alojado todavía
		}
	}

	// US-15/CU-09: agrega una foto (cámara o galería) al marcador en foco, en SU posición. La coordenada es la
	// del marcador (no el EXIF de la foto): así una foto del catálogo igual cae en este marcador por radio (RN-02).
	// Reusa la cola de capturas (offline) y su subida; el backend rechaza si el relevamiento está cerrado (RN-05).
	private async void OnAgregarFoto(object? sender, EventArgs e)
	{
		if (_nav?.MarcadorActual is not { } marcador)
		{
			return;
		}

		if (_relevamientoId is not { } relevamientoId)
		{
			EdicionLbl.Text = "Cargá una revisión antes de agregar una foto.";
			return;
		}

		var origen = await DisplayActionSheetAsync("Agregar foto al marcador", "Cancelar", null, "📷 Cámara", "🖼 Galería");
		if (origen is null or "Cancelar")
		{
			return;
		}

		try
		{
			var foto = await TomarOElegirFotoAsync(origen);
			if (foto is null)
			{
				return;
			}

			using var stream = await foto.OpenReadAsync();
			using var ms = new MemoryStream();
			await stream.CopyToAsync(ms);

			var captura = _armadorFoto.Armar(
				Guid.NewGuid(), relevamientoId, marcador.Latitud, marcador.Longitud, ms.ToArray(), foto.FileName, DateTime.UtcNow);
			if (captura is null)
			{
				EdicionLbl.Text = "No se pudo preparar la foto (vacía o coordenada del marcador inválida).";
				return;
			}

			try
			{
				await _cola.EncolarAsync(captura);
			}
			catch (AlmacenamientoLocalInsuficienteException)
			{
				EdicionLbl.Text = "Sin espacio local; no se pudo encolar la foto.";
				return;
			}

			await SubirYRecargarAsync();
		}
		catch (Exception ex)
		{
			EdicionLbl.Text = $"No se pudo agregar la foto: {ex.Message}";
		}
	}

	// Toma con cámara (pidiendo el permiso en runtime) o elige de la galería. Marca "captura en curso" alrededor
	// de la cámara (S55) para que, si el SO mata el proceso, al volver el arranque no rebote al login pidiendo patrón.
	private async Task<FileResult?> TomarOElegirFotoAsync(string origen)
	{
		if (origen.Contains("Galería"))
		{
			var fotos = await MediaPicker.Default.PickPhotosAsync();
			return fotos?.FirstOrDefault();
		}

		if (!await AsegurarPermisoCamaraAsync())
		{
			EdicionLbl.Text = "Se necesita permiso de cámara. Habilitalo en Ajustes.";
			return null;
		}

		if (!MediaPicker.Default.IsCaptureSupported)
		{
			EdicionLbl.Text = "Este dispositivo no permite capturar fotos con la cámara.";
			return null;
		}

		await _marcadorCaptura.MarcarEnCursoAsync();
		try
		{
			return await MediaPicker.Default.CapturePhotoAsync();
		}
		finally
		{
			await _marcadorCaptura.LimpiarAsync();
		}
	}

	private static async Task<bool> AsegurarPermisoCamaraAsync()
	{
		var estado = await Permissions.CheckStatusAsync<Permissions.Camera>();
		if (estado != PermissionStatus.Granted)
		{
			estado = await Permissions.RequestAsync<Permissions.Camera>();
		}

		return estado == PermissionStatus.Granted;
	}

	// Drena la cola de capturas (sube la foto recién encolada) y recarga la revisión para que aparezca en el
	// marcador. Sin conexión queda en cola y se subirá al reconectar (no se pierde).
	private async Task SubirYRecargarAsync()
	{
		try
		{
			var r = await _motor.SincronizarAsync();
			EdicionLbl.Text = r.Subidas.Count > 0
				? "Foto agregada al marcador."
				: $"Foto encolada; pendientes de subir: {await _cola.PendientesAsync()}.";
		}
		catch (SyncInterruptedException)
		{
			EdicionLbl.Text = $"Sin conexión: la foto quedó en cola ({await _cola.PendientesAsync()} pendiente(s)); se subirá al reconectar.";
		}

		await RecargarAsync();
	}

	private async void OnAgregarComentario(object? sender, EventArgs e)
	{
		if (_nav?.MarcadorActual is not { } marcador)
		{
			return;
		}

		var r = await _editor.AgregarComentarioAsync(marcador.MarcadorId, _nav.FotoActual?.FotoId, ComentarioEntry.Text ?? "");
		EdicionLbl.Text = r.Mensaje;
		if (r.Exito)
		{
			ComentarioEntry.Text = "";
			await RecargarAsync();
		}
	}

	private async void OnEtiquetarFoto(object? sender, EventArgs e)
	{
		if (_nav?.FotoActual is not { } foto)
		{
			EdicionLbl.Text = "El marcador en foco no tiene una foto para etiquetar.";
			return;
		}

		var r = await _editor.EtiquetarFotoAsync(foto.FotoId, EtiquetaEntry.Text ?? "");
		EdicionLbl.Text = r.Mensaje;
		if (r.Exito)
		{
			EtiquetaEntry.Text = "";
			await RecargarAsync();
		}
	}

	// US-15/CU-09 §5.A: quita la foto en foco del marcador (con confirmación). El backend borra foto + observación
	// + binario y desvincula los comentarios que la referenciaban; rechaza si el relevamiento está cerrado (RN-05).
	private async void OnQuitarFoto(object? sender, EventArgs e)
	{
		if (_nav?.FotoActual is not { } foto)
		{
			EdicionLbl.Text = "El marcador en foco no tiene una foto para quitar.";
			return;
		}

		var confirma = await DisplayAlertAsync("Quitar foto", "¿Quitar esta foto del marcador? No se puede deshacer.", "Quitar", "Cancelar");
		if (!confirma)
		{
			return;
		}

		var r = await _editor.QuitarFotoAsync(foto.FotoId);
		EdicionLbl.Text = r.Mensaje;
		if (r.Exito)
		{
			await RecargarAsync();
		}
	}

	// Recarga la revisión tras una edición para reflejar el cambio, conservando el marcador en foco.
	private async Task RecargarAsync()
	{
		if (_relevamientoId is not { } id)
		{
			return;
		}

		var marcadorEnFoco = _nav?.MarcadorActual?.MarcadorId;

		var revision = await _cliente.ObtenerAsync(id);
		if (revision is not null)
		{
			_revision = revision;
			_nav = new NavegadorRevision(revision);
			if (marcadorEnFoco is { } mid)
			{
				_nav.IrAlMarcador(mid); // restaura la posición; si el marcador ya no existe, queda en el primero
			}

			await RenderAsync();
		}
	}
}
