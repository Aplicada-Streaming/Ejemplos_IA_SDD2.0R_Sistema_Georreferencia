using FluentAssertions;
using GeoVial.Application;
using GeoVial.Application.Cqrs;
using GeoVial.Application.Relevamientos;
using GeoVial.Domain;
using GeoVial.Infrastructure;
using GeoVial.Infrastructure.Persistencia;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeoVial.IntegrationTests;

/// <summary>
/// Reproduce y cubre como regresión el bug crítico de la revisión funcional (§2): asignar un agente a un
/// relevamiento fallaba contra un proveedor relacional porque EF persistía la <see cref="AsignacionAgente"/>
/// nueva como UPDATE (0 filas → DbUpdateConcurrencyException → ACCION_NO_AUDITADA) en vez de INSERT.
/// Se usa SQLite (relacional) porque EF InMemory NO aplica el chequeo de "se esperaba 1 fila afectada"
/// y por eso el gate no lo detectaba. Cierra ese agujero del gate.
/// </summary>
public sealed class AsignacionAgenteRelacionalTests : IDisposable
{
    private readonly SqliteConnection _conexion;
    private readonly ServiceProvider _sp;

    public AsignacionAgenteRelacionalTests()
    {
        // Una sola conexión :memory: abierta mantiene viva la base mientras dure la prueba; todos los
        // scopes/DbContext comparten esa conexión (misma base relacional).
        _conexion = new SqliteConnection("DataSource=:memory:");
        _conexion.Open();

        var servicios = new ServiceCollection();
        servicios.AddLogging();
        var configuracion = new ConfigurationBuilder().AddInMemoryCollection().Build();
        servicios.AddApplication();
        servicios.AddInfrastructure(configuracion); // sin cadena de conexión → registra el DbContext InMemory…

        // …que reemplazamos por SQLite relacional (el que sí reproduce el bug). Hay que quitar TODO lo que
        // ate el DbContext al proveedor InMemory: las opciones (genéricas y no) y la IDbContextOptionsConfiguration
        // de EF Core 10; si no, quedan dos proveedores registrados a la vez.
        var aQuitar = servicios
            .Where(d => d.ServiceType == typeof(GeoVialDbContext)
                        || (d.ServiceType.FullName?.Contains("DbContextOptions") ?? false))
            .ToList();
        foreach (var descriptor in aQuitar)
        {
            servicios.Remove(descriptor);
        }

        servicios.AddDbContext<GeoVialDbContext>(o => o.UseSqlite(_conexion));

        _sp = servicios.BuildServiceProvider();

        using var scope = _sp.CreateScope();
        scope.ServiceProvider.GetRequiredService<GeoVialDbContext>().Database.EnsureCreated();
    }

    [Fact] // revision-funcional §2 / CU-01: asignar un agente persiste la asignación (INSERT) y habilita su captura
    public async Task Asignar_agente_persiste_la_asignacion_contra_proveedor_relacional()
    {
        var (jefeId, agenteId, relevamientoId) = SembrarAreaJefeAgenteYRelevamiento();

        // Act: en un scope nuevo (como un request HTTP fresco) el Jefe de Área asigna al agente.
        var resultado = await EnviarAsync(new AsignarAgentesCommand(jefeId, relevamientoId, new[] { agenteId }));

        // Assert: la asignación es exitosa (antes del fix fallaba con ACCION_NO_AUDITADA por el UPDATE de 0 filas).
        resultado.EsExito.Should().BeTrue($"la asignación no debe fallar; código = {resultado.Codigo}");

        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GeoVialDbContext>();
        var relevamiento = await db.Relevamientos.Include(r => r.Asignaciones)
            .FirstAsync(r => r.RelevamientoId == relevamientoId);
        relevamiento.AgentesVigentes().Should().ContainSingle().Which.Should().Be(agenteId,
            "la asignación debe quedar persistida y vigente para que el agente pueda capturar");
    }

    [Fact] // CU-01 §5.A: quitar y volver a asignar reactiva la asignación (UPDATE), sin duplicar ni romper
    public async Task Reasignar_un_agente_dado_de_baja_lo_reactiva()
    {
        var (jefeId, agenteId, relevamientoId) = SembrarAreaJefeAgenteYRelevamiento();

        (await EnviarAsync(new AsignarAgentesCommand(jefeId, relevamientoId, new[] { agenteId }))).EsExito.Should().BeTrue();
        // Reasignar a un conjunto vacío da de baja lógica al agente; reasignarlo de nuevo lo reactiva.
        (await EnviarAsync(new ReasignarAgentesCommand(jefeId, relevamientoId, Array.Empty<Guid>()))).EsExito.Should().BeTrue();
        (await EnviarAsync(new AsignarAgentesCommand(jefeId, relevamientoId, new[] { agenteId }))).EsExito.Should().BeTrue();

        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GeoVialDbContext>();
        var relevamiento = await db.Relevamientos.Include(r => r.Asignaciones)
            .FirstAsync(r => r.RelevamientoId == relevamientoId);
        relevamiento.AgentesVigentes().Should().ContainSingle().Which.Should().Be(agenteId);
    }

    private (Guid jefeId, Guid agenteId, Guid relevamientoId) SembrarAreaJefeAgenteYRelevamiento()
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GeoVialDbContext>();

        var area = Area.Crear("Zona Test");
        db.Areas.Add(area);
        var jefe = Usuario.Crear("Jefa de Área", RolJerarquico.JefeArea, area.AreaId).Valor!;
        var agente = Usuario.Crear("Agente de Campo", RolJerarquico.AgenteCampo, area.AreaId).Valor!;
        db.Usuarios.AddRange(jefe, agente);
        var relevamiento = Relevamiento.Crear("Obra de prueba", 15m, area.AreaId).Valor!;
        db.Relevamientos.Add(relevamiento);
        db.SaveChanges();

        return (jefe.UsuarioId, agente.UsuarioId, relevamiento.RelevamientoId);
    }

    private async Task<Resultado> EnviarAsync(IPeticion<Resultado> peticion)
    {
        using var scope = _sp.CreateScope();
        var mediador = scope.ServiceProvider.GetRequiredService<IMediador>();
        return await mediador.EnviarAsync(peticion);
    }

    public void Dispose()
    {
        _sp.Dispose();
        _conexion.Dispose();
    }
}
