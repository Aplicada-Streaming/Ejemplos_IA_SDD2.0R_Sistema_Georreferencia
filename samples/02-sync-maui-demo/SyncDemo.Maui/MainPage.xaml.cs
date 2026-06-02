using GeoVial.Sync;
using GeoVial.SyncDemo;

namespace SyncDemo.Maui;

public partial class MainPage : ContentPage
{
	private readonly CoordinadorDemo _coord;
	private readonly ReporterEnMemoria _reporter;
	private readonly Guid _recursoEnConflicto;

	private Guid? _changeIdEnConflicto;
	private int _contador;

	public MainPage(CoordinadorDemo coord, ReporterEnMemoria reporter, Guid recursoEnConflicto)
	{
		InitializeComponent();
		_coord = coord;
		_reporter = reporter;
		_recursoEnConflicto = recursoEnConflicto;
		_ = ActualizarPendientesAsync();
	}

	private async Task ActualizarPendientesAsync()
	{
		PendientesLbl.Text = $"Pendientes en cola: {await _coord.PendientesAsync()}";
	}

	private ObservacionCapturada NuevaObservacion(Guid comentarioId) =>
		new(comentarioId, Guid.NewGuid(), null, Guid.NewGuid(), $"Observación #{++_contador}", DateTime.UtcNow);

	private async void OnCapturar(object? sender, EventArgs e)
	{
		await _coord.CapturarAsync(NuevaObservacion(Guid.NewGuid()));
		EstadoLbl.Text = "Registro local encolado. Sincronizará limpio.";
		await ActualizarPendientesAsync();
	}

	private async void OnCapturarConflicto(object? sender, EventArgs e)
	{
		// Captura sobre el recurso que el backend simulado marca en conflicto: al sincronizar se reportará.
		var cambio = await _coord.CapturarAsync(NuevaObservacion(_recursoEnConflicto));
		_changeIdEnConflicto = cambio.ChangeId;
		EstadoLbl.Text = "Registro en conflicto encolado. Al sincronizar, el backend lo reportará.";
		await ActualizarPendientesAsync();
	}

	private async void OnSincronizar(object? sender, EventArgs e)
	{
		var r = await _coord.SincronizarAsync();
		EstadoLbl.Text =
			$"Sincronización: confirmados {r.Confirmed.Count} · conflictos {r.Conflicts.Count} · actualizaciones {r.Updates.Count}.";
		MostrarResolucionSiHayConflicto(r);
		await ActualizarPendientesAsync();
	}

	private void MostrarResolucionSiHayConflicto(SyncResult r)
	{
		if (r.Conflicts.Count > 0)
		{
			var recursos = string.Join(", ", _reporter.Ultimos.SelectMany(c => c.InvolvedResources));
			ConflictoLbl.Text = $"Conflicto reportado en: {recursos}. Resolvelo para continuar.";
			ResolucionPanel.IsVisible = true;
		}
		else
		{
			ResolucionPanel.IsVisible = false;
		}
	}

	private async void OnMantenerLocal(object? sender, EventArgs e)
	{
		_coord.ResolverManteniendoLocal(_recursoEnConflicto);
		ResolucionPanel.IsVisible = false;
		EstadoLbl.Text = "Resuelto: prevalece lo local. Sincronizá de nuevo para confirmarlo.";
		await ActualizarPendientesAsync();
	}

	private async void OnAceptarRemoto(object? sender, EventArgs e)
	{
		if (_changeIdEnConflicto is { } changeId)
		{
			await _coord.ResolverAceptandoRemotoAsync(changeId);
			_changeIdEnConflicto = null;
		}

		ResolucionPanel.IsVisible = false;
		EstadoLbl.Text = "Resuelto: se aceptó lo remoto y se descartó el cambio local.";
		await ActualizarPendientesAsync();
	}
}
