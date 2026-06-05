using FluentAssertions;
using GeoVial.Sync;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Indicador de estado de sincronización (S48, acción de retro S45-S47). El cálculo es una función pura sobre
/// conectividad + pendientes + sync en curso + último error; el monitor lo alimenta desde las colas reales.
/// </summary>
public class ResumenSincronizacionTests
{
    [Fact] // sincronizando tiene prioridad sobre todo lo demás
    public void Sincronizando_gana_sobre_todo()
    {
        var r = ResumenSincronizacion.Calcular(online: false, sincronizando: true, pendientes: 5, huboError: true);
        r.Estado.Should().Be(EstadoSync.Sincronizando);
        r.Texto.Should().Be("Sincronizando…");
    }

    [Fact] // sin conexión y sin pendientes: solo informa la falta de señal
    public void Sin_conexion_sin_pendientes()
    {
        var r = ResumenSincronizacion.Calcular(online: false, sincronizando: false, pendientes: 0);
        r.Estado.Should().Be(EstadoSync.SinConexion);
        r.Texto.Should().Be("Sin conexión");
    }

    [Fact] // sin conexión con pendientes: informa cuántos quedan en cola local
    public void Sin_conexion_con_pendientes_muestra_conteo()
    {
        var r = ResumenSincronizacion.Calcular(online: false, sincronizando: false, pendientes: 3);
        r.Estado.Should().Be(EstadoSync.SinConexion);
        r.Pendientes.Should().Be(3);
        r.Texto.Should().Be("Sin conexión · 3 pendientes sin sincronizar");
    }

    [Fact] // con conexión y pendientes: invita a sincronizar
    public void Online_con_pendientes_es_pendiente()
    {
        var r = ResumenSincronizacion.Calcular(online: true, sincronizando: false, pendientes: 2);
        r.Estado.Should().Be(EstadoSync.Pendiente);
        r.Texto.Should().Be("2 pendientes por sincronizar");
    }

    [Fact] // con conexión y nada pendiente: todo a salvo
    public void Online_sin_pendientes_esta_al_dia()
    {
        var r = ResumenSincronizacion.Calcular(online: true, sincronizando: false, pendientes: 0);
        r.Estado.Should().Be(EstadoSync.AlDia);
        r.Texto.Should().Be("Todo sincronizado");
    }

    [Fact] // con conexión, último intento fallido y pendientes: estado de error con reintento
    public void Online_con_error_y_pendientes_es_error()
    {
        var r = ResumenSincronizacion.Calcular(online: true, sincronizando: false, pendientes: 1, huboError: true);
        r.Estado.Should().Be(EstadoSync.Error);
        r.Texto.Should().Be("No se pudo sincronizar · 1 pendiente (se reintentará)");
    }

    [Fact] // error sin pendientes (nada quedó por subir) no es error: está al día
    public void Error_sin_pendientes_no_es_error()
    {
        var r = ResumenSincronizacion.Calcular(online: true, sincronizando: false, pendientes: 0, huboError: true);
        r.Estado.Should().Be(EstadoSync.AlDia);
    }

    [Theory] // singular vs plural en el texto
    [InlineData(1, "1 pendiente por sincronizar")]
    [InlineData(4, "4 pendientes por sincronizar")]
    public void Texto_respeta_singular_y_plural(int pendientes, string esperado)
    {
        ResumenSincronizacion.Calcular(online: true, sincronizando: false, pendientes: pendientes)
            .Texto.Should().Be(esperado);
    }

    [Fact] // defensivo: un conteo negativo se normaliza a 0
    public void Pendientes_negativos_se_normalizan()
    {
        var r = ResumenSincronizacion.Calcular(online: true, sincronizando: false, pendientes: -3);
        r.Pendientes.Should().Be(0);
        r.Estado.Should().Be(EstadoSync.AlDia);
    }
}

public class MonitorSincronizacionTests
{
    private sealed class ConectividadFake : IConnectivityMonitor
    {
        public bool IsOnline { get; set; } = true;
        public event EventHandler? ConnectivityRestored;
        public void Reconectar() => ConnectivityRestored?.Invoke(this, EventArgs.Empty);
    }

    private sealed class ColaCambiosFake : IChangeQueue
    {
        public int Pendientes { get; set; }
        public Task EnqueueAsync(ChangeRecord change, CancellationToken ct = default) => Task.CompletedTask;
        public Task<IReadOnlyList<ChangeRecord>> ReadPendingAsync(int max, CancellationToken ct = default) =>
            Task.FromResult((IReadOnlyList<ChangeRecord>)Array.Empty<ChangeRecord>());
        public Task MarkConfirmedAsync(IEnumerable<Guid> changeIds, CancellationToken ct = default) => Task.CompletedTask;
        public Task<int> PendingCountAsync(CancellationToken ct = default) => Task.FromResult(Pendientes);
    }

    private sealed class ColaCapturasFake : IColaCapturas
    {
        public int Pendientes { get; set; }
        public Task EncolarAsync(CapturaPendiente c, CancellationToken ct = default) => Task.CompletedTask;
        public Task<IReadOnlyList<CapturaPendiente>> LeerPendientesAsync(int max, CancellationToken ct = default) =>
            Task.FromResult((IReadOnlyList<CapturaPendiente>)Array.Empty<CapturaPendiente>());
        public Task MarcarSubidasAsync(IEnumerable<Guid> ids, CancellationToken ct = default) => Task.CompletedTask;
        public Task<int> PendientesAsync(CancellationToken ct = default) => Task.FromResult(Pendientes);
    }

    [Fact] // F-M (S48): refrescar suma comentarios + capturas y refleja la conectividad; notifica el cambio
    public async Task Refrescar_suma_las_dos_colas_y_notifica()
    {
        var monitor = new MonitorSincronizacion(
            new ConectividadFake { IsOnline = true }, new ColaCambiosFake { Pendientes = 2 }, new ColaCapturasFake { Pendientes = 3 });
        ResumenSincronizacion? notificado = null;
        monitor.Cambiado += (_, r) => notificado = r;

        var resumen = await monitor.RefrescarAsync();

        resumen.Pendientes.Should().Be(5);
        resumen.Estado.Should().Be(EstadoSync.Pendiente);
        monitor.Actual.Should().Be(resumen);
        notificado.Should().Be(resumen);
    }

    [Fact] // sin conexión, el refresco refleja el estado offline con el total en cola
    public async Task Refrescar_sin_conexion_es_sin_conexion()
    {
        var monitor = new MonitorSincronizacion(
            new ConectividadFake { IsOnline = false }, new ColaCambiosFake { Pendientes = 1 }, new ColaCapturasFake { Pendientes = 0 });

        var resumen = await monitor.RefrescarAsync();

        resumen.Estado.Should().Be(EstadoSync.SinConexion);
        resumen.Pendientes.Should().Be(1);
    }

    [Fact] // refrescar con huboError + conexión + pendientes => estado de error
    public async Task Refrescar_con_error_es_error()
    {
        var monitor = new MonitorSincronizacion(
            new ConectividadFake { IsOnline = true }, new ColaCambiosFake { Pendientes = 1 }, new ColaCapturasFake { Pendientes = 0 });

        var resumen = await monitor.RefrescarAsync(huboError: true);

        resumen.Estado.Should().Be(EstadoSync.Error);
    }

    [Fact] // marcar "sincronizando" conserva el conteo conocido y notifica
    public async Task Marcar_sincronizando_conserva_pendientes_y_notifica()
    {
        var monitor = new MonitorSincronizacion(
            new ConectividadFake { IsOnline = true }, new ColaCambiosFake { Pendientes = 4 }, new ColaCapturasFake { Pendientes = 0 });
        await monitor.RefrescarAsync(); // fija Pendientes = 4
        ResumenSincronizacion? notificado = null;
        monitor.Cambiado += (_, r) => notificado = r;

        monitor.MarcarSincronizando();

        monitor.Actual.Estado.Should().Be(EstadoSync.Sincronizando);
        monitor.Actual.Pendientes.Should().Be(4);
        notificado.Should().Be(monitor.Actual);
    }
}
