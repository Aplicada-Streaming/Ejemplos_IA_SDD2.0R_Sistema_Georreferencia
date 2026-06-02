using GeoVial.Mobile.Servicios;
using GeoVial.Sync;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Networking;

namespace GeoVial.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Cableado de la librería de sincronización (BT-15) sobre la cola SQLite local (US-17, ADR-05).
		// La base de datos vive en el almacenamiento privado de la app.
		var rutaCola = Path.Combine(FileSystem.AppDataDirectory, "cola-sync.db");
		builder.Services.AddSingleton<IChangeQueue>(_ => new ColaCambiosSqlite($"Data Source={rutaCola}"));
		builder.Services.AddSingleton<IConflictReporter, ReporterConflictosLog>();

		// Cliente HTTP hacia el backend GeoVial; la BaseAddress y el token de sesión se configuran al iniciar sesión.
		builder.Services.AddSingleton<ISyncBackendClient>(_ =>
			new ClienteSyncHttp(new HttpClient { BaseAddress = new Uri("https://localhost:5001/") }));
		builder.Services.AddSingleton<ISyncEngine, MotorSincronizacion>();

		// Captura offline (US-16) y sincronización automática por conectividad (US-19).
		builder.Services.AddSingleton(Connectivity.Current);
		builder.Services.AddSingleton<IConnectivityMonitor, MonitorConectividadMaui>();
		builder.Services.AddSingleton<ColectorOffline>();
		builder.Services.AddSingleton<CoordinadorAutoSync>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

/// <summary>Reporta los conflictos a la bitácora; la UI de resolución se construye en sprints posteriores.</summary>
internal sealed class ReporterConflictosLog : IConflictReporter
{
	public Task ReportAsync(IReadOnlyList<ConflictInfo> conflicts, CancellationToken ct = default)
	{
		foreach (var c in conflicts)
		{
			System.Diagnostics.Debug.WriteLine($"Conflicto {c.Kind}: {string.Join(", ", c.InvolvedResources)}");
		}

		return Task.CompletedTask;
	}
}
