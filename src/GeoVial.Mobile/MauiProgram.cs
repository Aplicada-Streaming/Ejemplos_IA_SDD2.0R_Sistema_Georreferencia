using GeoVial.CapturaCampo;
using GeoVial.Mobile.Servicios;
using GeoVial.Revision;
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

		// Cliente HTTP compartido hacia el backend GeoVial. En el dispositivo se alcanza por adb reverse
		// (localhost:5080 → host). El token de sesión se asienta al iniciar sesión y lo reusa el cliente de sync.
		builder.Services.AddSingleton(_ => new HttpClient { BaseAddress = new Uri("http://localhost:5080/") });
		// Sesión única (US-40): autentica una vez y asienta el token en el HttpClient compartido,
		// que reusan todas las páginas (incluido el cliente de sync). Reemplaza el login hardcodeado.
		// S54: persiste el token en SecureStorage para sobrevivir a que el SO mate el proceso (al usar la cámara).
		builder.Services.AddSingleton<IAlmacenTokenSesion, AlmacenTokenSecureStorage>();
		builder.Services.AddSingleton(sp => new ServicioSesion(
			sp.GetRequiredService<HttpClient>(), sp.GetRequiredService<IAlmacenTokenSesion>()));
		// Método de seguridad del teléfono para el reingreso en terreno (RN-06): recuerda usuario + marcador.
		builder.Services.AddSingleton<SeguridadDispositivo>();
		// Biométrico nativo (RN-06, S53): verificación con huella/rostro/PIN antes del reingreso sin clave.
		builder.Services.AddSingleton<IAutenticadorBiometrico, AutenticadorBiometricoAndroid>();
		builder.Services.AddSingleton(sp => new CoordinadorReingreso(
			sp.GetRequiredService<ServicioSesion>(), sp.GetRequiredService<IAutenticadorBiometrico>()));
		builder.Services.AddSingleton<ISyncBackendClient>(sp => new ClienteSyncHttp(sp.GetRequiredService<HttpClient>()));
		builder.Services.AddSingleton<ISyncEngine, MotorSincronizacion>();

		// Captura offline (US-16) y sincronización automática por conectividad (US-19, F-M-14).
		builder.Services.AddSingleton(Connectivity.Current);
		builder.Services.AddSingleton<IConnectivityMonitor, MonitorConectividadMaui>();
		builder.Services.AddSingleton<ColectorOffline>();
		// Al recuperar señal, sincroniza el relevamiento activo (de la sesión) y drena la cola de capturas (S42).
		builder.Services.AddSingleton(sp => new CoordinadorAutoSync(
			sp.GetRequiredService<IConnectivityMonitor>(),
			sp.GetRequiredService<ISyncEngine>(),
			sp.GetRequiredService<IChangeQueue>(),
			sp.GetRequiredService<MotorCapturas>(),
			() => sp.GetRequiredService<ServicioSesion>().RelevamientoActivoId));

		// Captura de campo (US-11): extracción de la coordenada desde EXIF + armado de la petición de captura.
		builder.Services.AddSingleton<IExtractorGpsExif, LectorGpsExif>();
		builder.Services.AddSingleton<ArmadorCapturaCampo>();
		builder.Services.AddSingleton<ArmadorUbicacionManual>();

		// Captura offline real (US-16, CU-06): cola local de capturas (foto + coordenada) sobre SQLite +
		// cliente REST que sube observación + binario + motor que drena la cola al sincronizar. Así la
		// captura se encola sin conexión y se sube al reconectar, en vez de postear directo (F-M-12/13).
		var rutaCapturas = Path.Combine(FileSystem.AppDataDirectory, "cola-capturas.db");
		builder.Services.AddSingleton<IColaCapturas>(_ => new ColaCapturasSqlite($"Data Source={rutaCapturas}"));
		builder.Services.AddSingleton<ICapturaBackendClient>(sp => new ClienteCapturaHttp(sp.GetRequiredService<HttpClient>()));
		builder.Services.AddSingleton(sp => new MotorCapturas(sp.GetRequiredService<IColaCapturas>(), sp.GetRequiredService<ICapturaBackendClient>()));

		// Indicador de estado de sincronización (acción de retro S45-S47): el monitor calcula el estado
		// (al día / pendiente / sincronizando / sin conexión / error) desde las dos colas + la conectividad.
		builder.Services.AddSingleton(sp => new MonitorSincronizacion(
			sp.GetRequiredService<IConnectivityMonitor>(),
			sp.GetRequiredService<IChangeQueue>(),
			sp.GetRequiredService<IColaCapturas>()));

		// Revisión sobre mapa (US-21/US-22): cliente de la API de revisión.
		builder.Services.AddSingleton(sp => new ClienteRevisionHttp(sp.GetRequiredService<HttpClient>()));
		// Ubicación manual desde la app (S51, CU-05): postea la coordenada elegida en el mapa de la bandeja.
		builder.Services.AddSingleton(sp => new ClienteUbicacionManual(sp.GetRequiredService<HttpClient>()));
		// Edición sobre el marcador (US-15): cliente de comentarios y etiquetas.
		builder.Services.AddSingleton(sp => new ClienteEdicionMarcador(sp.GetRequiredService<HttpClient>()));

		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<CapturaPage>();
		builder.Services.AddTransient<RevisionPage>();
		builder.Services.AddTransient<MapaPage>();
		builder.Services.AddTransient<BandejaPage>();

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
