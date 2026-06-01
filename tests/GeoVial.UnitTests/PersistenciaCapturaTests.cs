using FluentAssertions;
using GeoVial.Application.Captura;
using GeoVial.Domain;
using GeoVial.Infrastructure;
using GeoVial.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Integración del flujo de captura contra EF Core (BT-05/BT-06/BT-07): el handler real persiste
/// marcador, observación y foto, y la georreferenciación por metadatos crea el marcador por radio.
/// </summary>
public class PersistenciaCapturaTests
{
    [Fact]
    public async Task Captura_con_metadatos_persiste_marcador_observacion_y_foto()
    {
        var raiz = new InMemoryDatabaseRoot();
        var opciones = new DbContextOptionsBuilder<GeoVialDbContext>()
            .UseInMemoryDatabase("captura-flujo", raiz)
            .Options;

        var area = Area.Crear("Zona Norte");
        var agente = Usuario.Crear("agente", RolJerarquico.AgenteCampo, area.AreaId).Valor!;
        var relevamiento = Relevamiento.Crear("Puente Río 12", 15m, area.AreaId).Valor!;
        relevamiento.AsignarAgente(agente);

        await using (var db = new GeoVialDbContext(opciones))
        {
            db.Add(area);
            db.Add(agente);
            db.Add(relevamiento);
            await db.SaveChangesAsync();
        }

        await using (var db = new GeoVialDbContext(opciones))
        {
            var reloj = new RelojUtc();
            var handler = new CapturarObservacionHandler(
                new RelevamientoRepository(db), new UsuarioRepository(db), new MarcadorRepository(db),
                new ObservacionRepository(db), new FotoRepository(db), new ServicioAuditoria(db, reloj), reloj);

            var r = await handler.ManejarAsync(
                new CapturarObservacionCommand(agente.UsuarioId, relevamiento.RelevamientoId, "foto.jpg", -34.6000m, -58.4000m));

            r.EsExito.Should().BeTrue();
            r.Valor!.SinGeorreferenciar.Should().BeFalse();
        }

        await using (var db = new GeoVialDbContext(opciones))
        {
            (await db.Marcadores.CountAsync()).Should().Be(1);
            (await db.Observaciones.CountAsync()).Should().Be(1);
            (await db.Fotos.CountAsync()).Should().Be(1);
            var foto = await db.Fotos.SingleAsync();
            foto.Fuente.Should().Be(FuenteCoordenada.Metadatos);
            foto.TieneMetadatosUbicacion.Should().BeTrue();
        }
    }
}
