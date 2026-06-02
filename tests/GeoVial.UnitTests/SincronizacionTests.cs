using FluentAssertions;
using GeoVial.Application.Sincronizacion;
using GeoVial.Domain;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Consolidación backend de la sincronización: idempotencia, last-write-wins y conflictos (CU-07, US-18).</summary>
public class SincronizacionTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();
    private static readonly Guid AreaSur = Guid.NewGuid();
    private static readonly DateTime T0 = new(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);

    private sealed record Escenario(
        Usuario Agente,
        Relevamiento Rel,
        Guid MarcadorId,
        FakeUsuarioRepository Usuarios,
        FakeRelevamientoRepository Relevamientos,
        FakeComentarioRepository Comentarios,
        FakeConflictoRepository Conflictos,
        FakeCambioAplicadoRepository Cambios,
        FakeAuditoria Auditoria);

    private static Escenario Armar(EstadoRelevamiento estado = EstadoRelevamiento.Recoleccion, params Comentario[] comentarios)
    {
        var agente = Usuario.Crear("agente", RolJerarquico.AgenteCampo, AreaNorte).Valor!;
        var rel = Relevamiento.Importar("Puente Río 12", 15m, AreaNorte, estado).Valor!;
        var marcador = Marcador.Crear(rel.RelevamientoId, new Coordenada(-34.6m, -58.4m));
        return new Escenario(agente, rel, marcador.MarcadorId, new FakeUsuarioRepository(agente),
            new FakeRelevamientoRepository(rel), new FakeComentarioRepository(comentarios),
            new FakeConflictoRepository(), new FakeCambioAplicadoRepository(), new FakeAuditoria());
    }

    private static SincronizarHandler Handler(Escenario e) =>
        new(e.Usuarios, e.Relevamientos, e.Comentarios, e.Conflictos, e.Cambios, e.Auditoria, new FakeReloj(), new FakeMediador());

    private static CambioComentario Crear(Guid cambioId, Guid comentarioId, Guid marcadorId, Guid autor, string texto, DateTime marca) =>
        new(cambioId, OperacionSync.Crear, comentarioId, marcadorId, null, autor, texto, marca);

    private static CambioComentario Actualizar(Guid cambioId, Guid comentarioId, Guid marcadorId, Guid autor, string texto, DateTime marca) =>
        new(cambioId, OperacionSync.Actualizar, comentarioId, marcadorId, null, autor, texto, marca);

    [Fact] // CU-07 CA-01: un cambio de creación se sube, se aplica y se confirma; baja el comentario
    public async Task Sincronizar_crea_comentario_y_confirma()
    {
        var e = Armar();
        var comentarioId = Guid.NewGuid();
        var cambio = Crear(Guid.NewGuid(), comentarioId, e.MarcadorId, e.Agente.UsuarioId, "fisura en viga", T0);

        var r = await Handler(e).ManejarAsync(new SincronizarCommand(e.Agente.UsuarioId, e.Rel.RelevamientoId, new[] { cambio }, null));

        r.EsExito.Should().BeTrue();
        r.Valor!.Confirmados.Should().ContainSingle().Which.Should().Be(cambio.CambioId);
        (await e.Comentarios.ObtenerPorIdAsync(comentarioId))!.Texto.Should().Be("fisura en viga");
        r.Valor.Actualizaciones.Should().ContainSingle().Which.ComentarioId.Should().Be(comentarioId);
        e.Auditoria.Registros.Should().Contain(x => x.StartsWith("SINCRONIZAR:"));
    }

    [Fact] // CU-07 CA-03: reenviar el mismo CambioId no aplica dos veces (idempotencia, RC-03)
    public async Task Sincronizar_es_idempotente_por_cambio_id()
    {
        var e = Armar();
        var cambio = Crear(Guid.NewGuid(), Guid.NewGuid(), e.MarcadorId, e.Agente.UsuarioId, "fisura", T0);
        var cmd = new SincronizarCommand(e.Agente.UsuarioId, e.Rel.RelevamientoId, new[] { cambio }, null);

        await Handler(e).ManejarAsync(cmd);
        var segunda = await Handler(e).ManejarAsync(cmd); // mismo CambioId

        segunda.EsExito.Should().BeTrue();
        segunda.Valor!.Confirmados.Should().ContainSingle(); // confirmado sin reaplicar
        (await e.Comentarios.ListarPorMarcadorAsync(e.MarcadorId)).Should().ContainSingle(); // no se duplicó
    }

    [Fact] // CU-07 CA-02 / RN-04: dos ediciones del mismo comentario — prevalece la más reciente y se marca conflicto
    public async Task Sincronizar_last_write_wins_marca_conflicto()
    {
        // Comentario central ya editado (marca T1 > su creación) → compite con la edición entrante.
        var comentario = Comentario.Sincronizar(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "versión central", T0).Valor!;
        comentario.AplicarEdicion("versión central editada", T0.AddMinutes(5)); // FueEditado = true
        var e = Armar(EstadoRelevamiento.Recoleccion, comentario);

        var entrante = Actualizar(Guid.NewGuid(), comentario.ComentarioId, comentario.MarcadorId, e.Agente.UsuarioId, "versión de campo", T0.AddMinutes(10));
        var r = await Handler(e).ManejarAsync(new SincronizarCommand(e.Agente.UsuarioId, e.Rel.RelevamientoId, new[] { entrante }, null));

        r.EsExito.Should().BeTrue();
        comentario.Texto.Should().Be("versión de campo"); // la más reciente prevalece (T0+10 > T0+5)
        comentario.MarcaUltimaEdicion.Should().Be(T0.AddMinutes(10));
        e.Conflictos.Todos.Should().ContainSingle().Which.Tipo.Should().Be(TipoConflicto.EdicionEnConflicto);
        r.Valor!.Conflictos.Should().ContainSingle();
    }

    [Fact] // RN-04: si la entrante es más vieja que la central, la central se conserva pero igual se marca conflicto
    public async Task Sincronizar_edicion_mas_vieja_conserva_central_pero_marca_conflicto()
    {
        var comentario = Comentario.Sincronizar(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "central", T0).Valor!;
        comentario.AplicarEdicion("central nueva", T0.AddMinutes(20));
        var e = Armar(EstadoRelevamiento.Recoleccion, comentario);

        var entrante = Actualizar(Guid.NewGuid(), comentario.ComentarioId, comentario.MarcadorId, e.Agente.UsuarioId, "campo viejo", T0.AddMinutes(10));
        await Handler(e).ManejarAsync(new SincronizarCommand(e.Agente.UsuarioId, e.Rel.RelevamientoId, new[] { entrante }, null));

        comentario.Texto.Should().Be("central nueva"); // la central (más reciente) prevalece
        e.Conflictos.Todos.Should().ContainSingle();
    }

    [Fact] // una primera edición (no editado aún) se aplica sin marcar conflicto
    public async Task Sincronizar_primera_edicion_no_marca_conflicto()
    {
        var comentario = Comentario.Sincronizar(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "original", T0).Valor!;
        var e = Armar(EstadoRelevamiento.Recoleccion, comentario);

        var entrante = Actualizar(Guid.NewGuid(), comentario.ComentarioId, comentario.MarcadorId, e.Agente.UsuarioId, "editado", T0.AddMinutes(5));
        await Handler(e).ManejarAsync(new SincronizarCommand(e.Agente.UsuarioId, e.Rel.RelevamientoId, new[] { entrante }, null));

        comentario.Texto.Should().Be("editado");
        e.Conflictos.Todos.Should().BeEmpty();
    }

    [Fact] // RN-05: no se sincroniza un relevamiento cerrado
    public async Task Sincronizar_sobre_cerrado_rechaza()
    {
        var e = Armar(EstadoRelevamiento.Cerrado);
        var cambio = Crear(Guid.NewGuid(), Guid.NewGuid(), e.MarcadorId, e.Agente.UsuarioId, "x", T0);
        var r = await Handler(e).ManejarAsync(new SincronizarCommand(e.Agente.UsuarioId, e.Rel.RelevamientoId, new[] { cambio }, null));
        r.Codigo.Should().Be(CodigosError.RelevamientoSoloLectura);
    }

    [Fact] // RN-01: un agente de otra área no sincroniza
    public async Task Sincronizar_otra_area_rechaza()
    {
        var e = Armar();
        var agenteSur = Usuario.Crear("sur", RolJerarquico.AgenteCampo, AreaSur).Valor!;
        var handler = new SincronizarHandler(
            new FakeUsuarioRepository(agenteSur), e.Relevamientos, e.Comentarios, e.Conflictos, e.Cambios, e.Auditoria, new FakeReloj(), new FakeMediador());

        var cambio = Crear(Guid.NewGuid(), Guid.NewGuid(), e.MarcadorId, agenteSur.UsuarioId, "x", T0);
        var r = await handler.ManejarAsync(new SincronizarCommand(agenteSur.UsuarioId, e.Rel.RelevamientoId, new[] { cambio }, null));

        r.Codigo.Should().Be(CodigosError.AccesoNoAutorizado);
    }

    [Fact] // RN-07: si no se puede auditar, no se sincroniza
    public async Task Sincronizar_sin_auditoria_falla()
    {
        var e = Armar() with { };
        var handler = new SincronizarHandler(
            e.Usuarios, e.Relevamientos, e.Comentarios, e.Conflictos, e.Cambios, new FakeAuditoria(exito: false), new FakeReloj(), new FakeMediador());

        var cambio = Crear(Guid.NewGuid(), Guid.NewGuid(), e.MarcadorId, e.Agente.UsuarioId, "x", T0);
        var r = await handler.ManejarAsync(new SincronizarCommand(e.Agente.UsuarioId, e.Rel.RelevamientoId, new[] { cambio }, null));

        r.Codigo.Should().Be(CodigosError.AccionNoAuditada);
    }

    [Fact] // CU-07 §4.2: los cambios se aplican en orden de marca temporal
    public async Task Sincronizar_aplica_en_orden_de_marca_temporal()
    {
        var e = Armar();
        var comentarioId = Guid.NewGuid();
        var crear = Crear(Guid.NewGuid(), comentarioId, e.MarcadorId, e.Agente.UsuarioId, "v1", T0);
        var actualizar = Actualizar(Guid.NewGuid(), comentarioId, e.MarcadorId, e.Agente.UsuarioId, "v2", T0.AddMinutes(5));

        // Se envían desordenados; el handler ordena por marca temporal (crear antes que actualizar).
        var r = await Handler(e).ManejarAsync(new SincronizarCommand(e.Agente.UsuarioId, e.Rel.RelevamientoId, new[] { actualizar, crear }, null));

        r.EsExito.Should().BeTrue();
        r.Valor!.Confirmados.Should().HaveCount(2);
        (await e.Comentarios.ObtenerPorIdAsync(comentarioId))!.Texto.Should().Be("v2");
    }
}
