using GeoVial.Sync;
using GeoVial.SyncDemo;
using Microsoft.Extensions.Logging;

namespace SyncDemo.Maui;

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

		// Demo autónoma (US-32): toda la plomería de sincronización es la superficie pública de GeoVial.Sync,
		// más el backend SIMULADO en memoria. No hay backend real ni red: la demo es íntegramente local.
		var recursoEnConflicto = Guid.NewGuid();
		var relevamiento = Guid.NewGuid();
		var rutaCola = Path.Combine(FileSystem.AppDataDirectory, "syncdemo-cola.db");

		builder.Services.AddSingleton<IChangeQueue>(_ => new ColaCambiosSqlite($"Data Source={rutaCola}"));
		builder.Services.AddSingleton<BackendSimulado>(_ => new BackendSimulado(
			recursosEnConflicto: new[] { recursoEnConflicto },
			actualizaciones: new[] { "comentario remoto del jefe de área (bajado del backend)" }));
		builder.Services.AddSingleton<ISyncBackendClient>(sp => sp.GetRequiredService<BackendSimulado>());
		builder.Services.AddSingleton<ReporterEnMemoria>();
		builder.Services.AddSingleton<IConflictReporter>(sp => sp.GetRequiredService<ReporterEnMemoria>());
		builder.Services.AddSingleton<ResolutorConflictos>(sp =>
			new ResolutorConflictos(sp.GetRequiredService<BackendSimulado>(), sp.GetRequiredService<IChangeQueue>()));
		builder.Services.AddSingleton<ISyncEngine>(sp => new MotorSincronizacion(
			sp.GetRequiredService<IChangeQueue>(),
			sp.GetRequiredService<ISyncBackendClient>(),
			sp.GetRequiredService<IConflictReporter>()));
		builder.Services.AddSingleton<CoordinadorDemo>(sp => new CoordinadorDemo(
			sp.GetRequiredService<IChangeQueue>(),
			sp.GetRequiredService<ISyncEngine>(),
			sp.GetRequiredService<ResolutorConflictos>(),
			relevamiento));
		builder.Services.AddTransient<MainPage>(sp => new MainPage(
			sp.GetRequiredService<CoordinadorDemo>(),
			sp.GetRequiredService<ReporterEnMemoria>(),
			recursoEnConflicto));

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

/// <summary>Reporter de conflictos de la demo: los acumula para mostrarlos en pantalla (CU-12, alcance básico).</summary>
public sealed class ReporterEnMemoria : IConflictReporter
{
	public List<ConflictInfo> Ultimos { get; } = new();

	public Task ReportAsync(IReadOnlyList<ConflictInfo> conflicts, CancellationToken ct = default)
	{
		Ultimos.Clear();
		Ultimos.AddRange(conflicts);
		return Task.CompletedTask;
	}
}
