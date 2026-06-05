using FluentAssertions;
using GeoVial.Application.Relevamientos;
using GeoVial.Domain;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Manejadores CQRS del módulo de relevamientos (CU-01, CU-10; RN-01/RN-02/RN-05/RN-07).</summary>
public class RelevamientoAplicacionTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();
    private static readonly Guid AreaSur = Guid.NewGuid();

    private static Usuario JefeArea(Guid area) => Usuario.Crear("jefe", RolJerarquico.JefeArea, area).Valor!;

    private static Usuario Agente(Guid area) => Usuario.Crear("agente", RolJerarquico.AgenteCampo, area).Valor!;

    private static Relevamiento RelevamientoDe(Guid area)
    {
        var r = Relevamiento.Crear("Puente Río 12", 15m, area).Valor!;
        return r;
    }

    [Fact] // CU-01 CA-01 + RN-07: alta por jefe de área, auditada
    public async Task Crear_por_jefe_de_area_exito_y_audita()
    {
        var jefe = JefeArea(AreaNorte);
        var auditoria = new FakeAuditoria();
        var handler = new CrearRelevamientoHandler(new FakeRelevamientoRepository(), new FakeUsuarioRepository(jefe), auditoria);

        var r = await handler.ManejarAsync(new CrearRelevamientoCommand(jefe.UsuarioId, "Puente Río 12", 15m));

        r.EsExito.Should().BeTrue();
        r.Valor!.Estado.Should().Be(EstadoRelevamiento.Recoleccion);
        r.Valor!.AreaId.Should().Be(AreaNorte);
        auditoria.Registros.Should().Contain(x => x.StartsWith("ALTA_RELEVAMIENTO:"));
    }

    [Fact] // RN-01: un agente no puede crear relevamientos
    public async Task Crear_por_agente_rechaza()
    {
        var agente = Agente(AreaNorte);
        var handler = new CrearRelevamientoHandler(new FakeRelevamientoRepository(), new FakeUsuarioRepository(agente), new FakeAuditoria());

        var r = await handler.ManejarAsync(new CrearRelevamientoCommand(agente.UsuarioId, "Obra", 10m));

        r.Codigo.Should().Be(CodigosError.AccesoNoAutorizado);
    }

    [Fact] // RN-07: sin auditoría se rechaza
    public async Task Crear_sin_auditoria_rechaza()
    {
        var jefe = JefeArea(AreaNorte);
        var handler = new CrearRelevamientoHandler(new FakeRelevamientoRepository(), new FakeUsuarioRepository(jefe), new FakeAuditoria(exito: false));

        var r = await handler.ManejarAsync(new CrearRelevamientoCommand(jefe.UsuarioId, "Obra", 10m));

        r.Codigo.Should().Be(CodigosError.AccionNoAuditada);
    }

    [Fact] // CU-01 CA-01: asignar agente del área
    public async Task Asignar_agente_del_area_exito()
    {
        var jefe = JefeArea(AreaNorte);
        var agente = Agente(AreaNorte);
        var rel = RelevamientoDe(AreaNorte);
        var handler = new AsignarAgentesHandler(
            new FakeRelevamientoRepository(rel), new FakeUsuarioRepository(jefe, agente), new FakeAuditoria());

        var r = await handler.ManejarAsync(new AsignarAgentesCommand(jefe.UsuarioId, rel.RelevamientoId, new[] { agente.UsuarioId }));

        r.EsExito.Should().BeTrue();
        rel.AgentesVigentes().Should().Contain(agente.UsuarioId);
    }

    [Fact] // CU-01 CA-02: agente de otra área se rechaza
    public async Task Asignar_agente_de_otra_area_rechaza()
    {
        var jefe = JefeArea(AreaNorte);
        var agenteSur = Agente(AreaSur);
        var rel = RelevamientoDe(AreaNorte);
        var handler = new AsignarAgentesHandler(
            new FakeRelevamientoRepository(rel), new FakeUsuarioRepository(jefe, agenteSur), new FakeAuditoria());

        var r = await handler.ManejarAsync(new AsignarAgentesCommand(jefe.UsuarioId, rel.RelevamientoId, new[] { agenteSur.UsuarioId }));

        r.Codigo.Should().Be(CodigosError.AgenteFueraDeArea);
    }

    [Fact] // relevamiento inexistente
    public async Task Asignar_a_relevamiento_inexistente_rechaza()
    {
        var jefe = JefeArea(AreaNorte);
        var handler = new AsignarAgentesHandler(
            new FakeRelevamientoRepository(), new FakeUsuarioRepository(jefe), new FakeAuditoria());

        var r = await handler.ManejarAsync(new AsignarAgentesCommand(jefe.UsuarioId, Guid.NewGuid(), Array.Empty<Guid>()));

        r.Codigo.Should().Be(CodigosError.RelevamientoInexistente);
    }

    [Fact] // RN-01: jefe de otra área no accede al relevamiento
    public async Task Transicion_por_jefe_de_otra_area_rechaza()
    {
        var jefeSur = JefeArea(AreaSur);
        var rel = RelevamientoDe(AreaNorte);
        var handler = new TransicionarEstadoHandler(
            new FakeRelevamientoRepository(rel), new FakeUsuarioRepository(jefeSur), new FakeAuditoria());

        var r = await handler.ManejarAsync(new TransicionarEstadoCommand(jefeSur.UsuarioId, rel.RelevamientoId, EstadoRelevamiento.Revision));

        r.Codigo.Should().Be(CodigosError.AccesoNoAutorizado);
    }

    [Fact] // CU-10 CA-01: transición válida auditada
    public async Task Transicion_valida_exito()
    {
        var jefe = JefeArea(AreaNorte);
        var rel = RelevamientoDe(AreaNorte);
        var auditoria = new FakeAuditoria();
        var handler = new TransicionarEstadoHandler(
            new FakeRelevamientoRepository(rel), new FakeUsuarioRepository(jefe), auditoria);

        var r = await handler.ManejarAsync(new TransicionarEstadoCommand(jefe.UsuarioId, rel.RelevamientoId, EstadoRelevamiento.Revision));

        r.EsExito.Should().BeTrue();
        rel.Estado.Should().Be(EstadoRelevamiento.Revision);
        auditoria.Registros.Should().Contain(x => x.StartsWith("TRANSICION_RELEVAMIENTO:"));
    }

    [Fact] // CU-10 CA-02: transición inválida
    public async Task Transicion_invalida_rechaza()
    {
        var jefe = JefeArea(AreaNorte);
        var rel = RelevamientoDe(AreaNorte);
        var handler = new TransicionarEstadoHandler(
            new FakeRelevamientoRepository(rel), new FakeUsuarioRepository(jefe), new FakeAuditoria());

        var r = await handler.ManejarAsync(new TransicionarEstadoCommand(jefe.UsuarioId, rel.RelevamientoId, EstadoRelevamiento.Cerrado));

        r.Codigo.Should().Be(CodigosError.TransicionInvalida);
    }

    [Fact] // CU-10 CA-03: reapertura de un cerrado
    public async Task Reabrir_un_cerrado_exito()
    {
        var jefe = JefeArea(AreaNorte);
        var rel = RelevamientoDe(AreaNorte);
        rel.TransicionarA(EstadoRelevamiento.Revision);
        rel.TransicionarA(EstadoRelevamiento.Cerrado);
        var handler = new ReabrirRelevamientoHandler(
            new FakeRelevamientoRepository(rel), new FakeUsuarioRepository(jefe), new FakeAuditoria());

        var r = await handler.ManejarAsync(new ReabrirRelevamientoCommand(jefe.UsuarioId, rel.RelevamientoId));

        r.EsExito.Should().BeTrue();
        rel.Estado.Should().Be(EstadoRelevamiento.Recoleccion);
    }

    [Fact] // US-08: reasignar fija el conjunto vigente
    public async Task Reasignar_fija_el_conjunto()
    {
        var jefe = JefeArea(AreaNorte);
        var agenteA = Agente(AreaNorte);
        var agenteB = Agente(AreaNorte);
        var rel = RelevamientoDe(AreaNorte);
        rel.AsignarAgente(agenteA);
        var handler = new ReasignarAgentesHandler(
            new FakeRelevamientoRepository(rel), new FakeUsuarioRepository(jefe, agenteA, agenteB), new FakeAuditoria());

        var r = await handler.ManejarAsync(new ReasignarAgentesCommand(jefe.UsuarioId, rel.RelevamientoId, new[] { agenteB.UsuarioId }));

        r.EsExito.Should().BeTrue();
        rel.AgentesVigentes().Should().ContainSingle().Which.Should().Be(agenteB.UsuarioId);
    }

    [Fact] // US-31/RN-01: el listado filtra por área del solicitante
    public async Task Listar_filtra_por_area()
    {
        var jefeNorte = JefeArea(AreaNorte);
        var relNorte = RelevamientoDe(AreaNorte);
        var relSur = RelevamientoDe(AreaSur);
        var handler = new ListarRelevamientosHandler(
            new FakeRelevamientoRepository(relNorte, relSur), new FakeUsuarioRepository(jefeNorte));

        var lista = await handler.ManejarAsync(new ListarRelevamientosQuery(jefeNorte.UsuarioId));

        lista.Select(x => x.RelevamientoId).Should().Contain(relNorte.RelevamientoId).And.NotContain(relSur.RelevamientoId);
    }

    [Fact] // S47/F-M-04: "asignados a mí" devuelve sólo los relevamientos con asignación vigente del agente
    public async Task Listar_asignados_devuelve_solo_los_del_agente()
    {
        var agente = Agente(AreaNorte);
        var asignado = RelevamientoDe(AreaNorte);
        asignado.AsignarAgente(agente);
        var noAsignado = RelevamientoDe(AreaNorte); // mismo área, pero sin asignar al agente
        var handler = new ListarRelevamientosAsignadosHandler(
            new FakeRelevamientoRepository(asignado, noAsignado), new FakeUsuarioRepository(agente));

        var lista = await handler.ManejarAsync(new ListarRelevamientosAsignadosQuery(agente.UsuarioId));

        lista.Select(x => x.RelevamientoId).Should().ContainSingle()
            .Which.Should().Be(asignado.RelevamientoId);
    }

    [Fact] // S47: un agente dado de baja no recibe relevamientos aunque tenga asignaciones
    public async Task Listar_asignados_excluye_agente_dado_de_baja()
    {
        var agente = Agente(AreaNorte);
        var asignado = RelevamientoDe(AreaNorte);
        asignado.AsignarAgente(agente);
        agente.DarDeBaja();
        var handler = new ListarRelevamientosAsignadosHandler(
            new FakeRelevamientoRepository(asignado), new FakeUsuarioRepository(agente));

        var lista = await handler.ManejarAsync(new ListarRelevamientosAsignadosQuery(agente.UsuarioId));

        lista.Should().BeEmpty();
    }
}
