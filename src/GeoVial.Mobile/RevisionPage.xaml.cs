using System.Net.Http.Headers;
using System.Net.Http.Json;
using GeoVial.Revision;

namespace GeoVial.Mobile;

public partial class RevisionPage : ContentPage
{
	private readonly HttpClient _http;
	private readonly ClienteRevisionHttp _cliente;
	private readonly ClienteEdicionMarcador _editor;

	private NavegadorRevision? _nav;
	private Guid? _relevamientoId;

	public RevisionPage(HttpClient http, ClienteRevisionHttp cliente, ClienteEdicionMarcador editor)
	{
		InitializeComponent();
		_http = http;
		_cliente = cliente;
		_editor = editor;
	}

	private async void OnCargar(object? sender, EventArgs e)
	{
		try
		{
			MarcadorLbl.Text = "Cargando…";
			if (await AutenticarYElegirRelevamientoAsync() is not { } relevamientoId)
			{
				return;
			}

			_relevamientoId = relevamientoId;
			var revision = await _cliente.ObtenerAsync(relevamientoId);
			if (revision is null)
			{
				MarcadorLbl.Text = "No se pudo obtener la revisión.";
				return;
			}

			_nav = new NavegadorRevision(revision);
			await RenderAsync();
		}
		catch (Exception ex)
		{
			MarcadorLbl.Text = $"Error al cargar: {ex.Message}";
		}
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
			var bytes = await _http.GetByteArrayAsync($"api/v1/fotos/{fotoId}/contenido");
			return ImageSource.FromStream(() => new MemoryStream(bytes));
		}
		catch
		{
			return null; // el binario puede no estar alojado todavía
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
			MarcadorLbl.Text = "No hay un relevamiento en el backend.";
			return null;
		}

		return relevamientos[0].RelevamientoId;
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

	// Recarga la revisión tras una edición para reflejar el cambio (vuelve al primer marcador).
	private async Task RecargarAsync()
	{
		if (_relevamientoId is not { } id)
		{
			return;
		}

		var revision = await _cliente.ObtenerAsync(id);
		if (revision is not null)
		{
			_nav = new NavegadorRevision(revision);
			await RenderAsync();
		}
	}

	private sealed record TokenDto(string AccessToken);

	private sealed record RelevamientoDto(Guid RelevamientoId);
}
