using FluentAssertions;
using GeoVial.Domain;
using GeoVial.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Verifica el mapeo EF del agregado Relevamiento con su colección de asignaciones (BT-07).
/// Usa el proveedor en memoria con una raíz compartida para simular el round-trip entre contextos.
/// </summary>
public class PersistenciaRelevamientoTests
{
    [Fact]
    public async Task Relevamiento_con_asignaciones_round_trip()
    {
        var raiz = new InMemoryDatabaseRoot();
        var opciones = new DbContextOptionsBuilder<GeoVialDbContext>()
            .UseInMemoryDatabase("relevamientos-round-trip", raiz)
            .Options;

        var areaNorte = Guid.NewGuid();
        var agente = Usuario.Crear("agente", RolJerarquico.AgenteCampo, areaNorte).Valor!;
        var relevamiento = Relevamiento.Crear("Puente Río 12", 15m, areaNorte).Valor!;
        relevamiento.AsignarAgente(agente);
        var id = relevamiento.RelevamientoId;

        await using (var db = new GeoVialDbContext(opciones))
        {
            var repo = new RelevamientoRepository(db);
            await repo.AgregarAsync(relevamiento);
            await repo.GuardarCambiosAsync();
        }

        await using (var db = new GeoVialDbContext(opciones))
        {
            var repo = new RelevamientoRepository(db);
            var leido = await repo.ObtenerPorIdAsync(id);

            leido.Should().NotBeNull();
            leido!.Estado.Should().Be(EstadoRelevamiento.Recoleccion);
            leido.RadioAgrupacionMetros.Should().Be(15m);
            leido.AgentesVigentes().Should().ContainSingle().Which.Should().Be(agente.UsuarioId);
        }
    }
}
