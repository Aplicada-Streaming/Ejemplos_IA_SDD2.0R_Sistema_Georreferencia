using GeoVial.Sync;
using Microsoft.Maui.Networking;

namespace GeoVial.Mobile.Servicios;

/// <summary>
/// Implementación de <see cref="IConnectivityMonitor"/> sobre la API de red de MAUI (US-19). Notifica la
/// recuperación de conexión cuando el acceso a red pasa a Internet, para disparar la sincronización automática.
/// </summary>
public sealed class MonitorConectividadMaui : IConnectivityMonitor, IDisposable
{
	private readonly IConnectivity _conectividad;

	public MonitorConectividadMaui(IConnectivity conectividad)
	{
		_conectividad = conectividad;
		_conectividad.ConnectivityChanged += AlCambiar;
	}

	public bool IsOnline => _conectividad.NetworkAccess == NetworkAccess.Internet;

	public event EventHandler ConnectivityRestored = delegate { };

	private void AlCambiar(object? sender, ConnectivityChangedEventArgs e)
	{
		if (e.NetworkAccess == NetworkAccess.Internet)
		{
			ConnectivityRestored.Invoke(this, EventArgs.Empty);
		}
	}

	public void Dispose() => _conectividad.ConnectivityChanged -= AlCambiar;
}
