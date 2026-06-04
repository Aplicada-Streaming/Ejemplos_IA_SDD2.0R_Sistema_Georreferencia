using GeoVial.Revision;
using GeoVial.Sync;

namespace GeoVial.Mobile;

public partial class RevisionPage : ContentPage
{
	private readonly HttpClient _http;
	private readonly ServicioSesion _sesion;
	private readonly ClienteRevisionHttp _cliente;
	private readonly ClienteEdicionMarcador _editor;
	private readonly CacheFotos _cacheFotos = new();

	private NavegadorRevision? _nav;
	private Guid? _relevamientoId;
	private GeoVial.Shared.RevisionRelevamientoDto? _revision;

	public RevisionPage(HttpClient http, ServicioSesion sesion, ClienteRevisionHttp cliente, ClienteEdicionMarcador editor)
	{
		InitializeComponent();
		_http = http;
		_sesion = sesion;
		_cliente = cliente;
		_editor = editor;
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
		await Navigation.PushModalAsync(new NavigationPage(new MapaRevisionPage(html)));
	}

	private async void OnSiguienteMarcador(object? sender, EventArgs e) { _nav?.SiguienteMarcador(); await RenderAsync(); }

	private async void OnAnteriorMarcador(object? sender, EventArgs e) { _nav?.AnteriorMarcador(); await RenderAsync(); }

	private async void OnSiguienteFoto(object? sender, EventArgs e) { _nav?.SiguienteFoto(); await RenderAsync(); }

	private async void OnAnteriorFoto(object? sender, EventArgs e) { _nav?.AnteriorFoto(); await RenderAsync(); }

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
