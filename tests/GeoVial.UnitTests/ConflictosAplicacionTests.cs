using FluentAssertions;
using GeoVial.Application.Conflictos;
using GeoVial.Domain;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Detección y resolución de conflictos por radio (CU-11, CU-12; RN-02, RN-05, RN-07).</summary>
public class ConflictosAplicacionTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();
    private static readonly Guid AreaSur = Guid.NewGuid();

    // Dos puntos a ~1,1 m (cercanos) y uno a ~1,1 km (lejano) del primero.
    private static readonly Coordenada PuntoA = new(-34.600000m, -58.400000m);
    private static readonly Coordenada PuntoCerca = new(-34.600010m, -58.400000m);
    private static readonly Coordenada PuntoLejos = new(-34.610000m, -58.400000m);

    private static Usuario Jefe() => Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;

    private static Relevamiento Relevamiento(EstadoRelevamiento? estado = null)
    {
        var r = Domain.Relevamiento.Crear("Puente Río 12", 15m, AreaNorte).Valor!;
        if (estado == EstadoRelevamiento.Cerrado)
        {
            r.TransicionarA(EstadoRelevamiento.Revision);
            r.TransicionarA(EstadoRelevamiento.Cerrado);
        }

        return r;
    }

    private static DetectarConflictosHandler DetectarHandler(
        Usuario jefe, Relevamiento rel, FakeMarcadorRepository marcadores, FakeConflictoRepository conflictos, FakeAuditoria? auditoria = null) =>
        new(new FakeUsuarioRepository(jefe), new FakeRelevamientoRepository(rel), marcadores, conflictos, auditoria ?? new FakeAuditoria());

    [Fact] // CU-11 CA-01: dos marcadores dentro del radio generan un conflicto pendiente y se marcan
    public async Task Detectar_marcadores_cercanos_registra_conflicto()
    {
        var jefe = Jefe();
        var rel = Relevamiento();
        var mA = Marcador.Crear(rel.RelevamientoId, PuntoA);
        var mB = Marcador.Crear(rel.RelevamientoId, PuntoCerca);
        var marcadores = new FakeMarcadorRepository(mA, mB);
        var conflictos = new FakeConflictoRepository();
        var handler = DetectarHandler(jefe, rel, marcadores, conflictos);

        var r = await handler.ManejarAsync(new DetectarConflictosCommand(jefe.UsuarioId, rel.RelevamientoId));

        r.EsExito.Should().BeTrue();
        r.Valor.Should().ContainSingle();
        conflictos.Todos.Should().ContainSingle().Which.EstaPendiente.Should().BeTrue();
        mA.EnConflicto.Should().BeTrue();
        mB.EnConflicto.Should().BeTrue();
    }

    [Fact] // CU-11: marcadores lejos del radio no generan conflicto
    public async Task Detectar_marcadores_lejanos_no_registra()
    {
        var jefe = Jefe();
        var rel = Relevamiento();
        var marcadores = new FakeMarcadorRepository(Marcador.Crear(rel.RelevamientoId, PuntoA), Marcador.Crear(rel.RelevamientoId, PuntoLejos));
        var conflictos = new FakeConflictoRepository();
        var handler = DetectarHandler(jefe, rel, marcadores, conflictos);

        var r = await handler.ManejarAsync(new DetectarConflictosCommand(jefe.UsuarioId, rel.RelevamientoId));

        r.EsExito.Should().BeTrue();
        r.Valor.Should().BeEmpty();
        conflictos.Todos.Should().BeEmpty();
    }

    [Fact] // RN-02 idempotencia: detectar dos veces no duplica el conflicto del par
    public async Task Detectar_dos_veces_es_idempotente()
    {
        var jefe = Jefe();
        var rel = Relevamiento();
        var marcadores = new FakeMarcadorRepository(Marcador.Crear(rel.RelevamientoId, PuntoA), Marcador.Crear(rel.RelevamientoId, PuntoCerca));
        var conflictos = new FakeConflictoRepository();
        var handler = DetectarHandler(jefe, rel, marcadores, conflictos);

        await handler.ManejarAsync(new DetectarConflictosCommand(jefe.UsuarioId, rel.RelevamientoId));
        var segunda = await handler.ManejarAsync(new DetectarConflictosCommand(jefe.UsuarioId, rel.RelevamientoId));

        segunda.Valor.Should().BeEmpty();
        conflictos.Todos.Should().ContainSingle();
    }

    [Fact] // RN-05: la detección sobre un relevamiento cerrado se rechaza
    public async Task Detectar_sobre_cerrado_rechaza()
    {
        var jefe = Jefe();
        var rel = Relevamiento(EstadoRelevamiento.Cerrado);
        var handler = DetectarHandler(jefe, rel, new FakeMarcadorRepository(), new FakeConflictoRepository());

        var r = await handler.ManejarAsync(new DetectarConflictosCommand(jefe.UsuarioId, rel.RelevamientoId));

        r.Codigo.Should().Be(CodigosError.RelevamientoSoloLectura);
    }

    [Fact] // RN-01: un jefe de otra área no detecta conflictos del relevamiento
    public async Task Detectar_otra_area_rechaza()
    {
        var rel = Relevamiento();
        var jefeSur = Usuario.Crear("js", RolJerarquico.JefeArea, AreaSur).Valor!;
        var handler = DetectarHandler(jefeSur, rel, new FakeMarcadorRepository(), new FakeConflictoRepository());

        var r = await handler.ManejarAsync(new DetectarConflictosCommand(jefeSur.UsuarioId, rel.RelevamientoId));

        r.Codigo.Should().Be(CodigosError.AccesoNoAutorizado);
    }

    [Fact] // RN-07: si no se puede auditar, la detección falla
    public async Task Detectar_sin_auditoria_falla()
    {
        var jefe = Jefe();
        var rel = Relevamiento();
        var marcadores = new FakeMarcadorRepository(Marcador.Crear(rel.RelevamientoId, PuntoA), Marcador.Crear(rel.RelevamientoId, PuntoCerca));
        var handler = DetectarHandler(jefe, rel, marcadores, new FakeConflictoRepository(), new FakeAuditoria(exito: false));

        var r = await handler.ManejarAsync(new DetectarConflictosCommand(jefe.UsuarioId, rel.RelevamientoId));

        r.Codigo.Should().Be(CodigosError.AccionNoAuditada);
    }

    // --- Ajuste de radio (CU-11 §5.A) ---

    [Fact] // CU-11 §5.A: el jefe ajusta el radio del relevamiento
    public async Task Ajustar_radio_exito_y_audita()
    {
        var jefe = Jefe();
        var rel = Relevamiento();
        var auditoria = new FakeAuditoria();
        var handler = new AjustarRadioHandler(new FakeUsuarioRepository(jefe), new FakeRelevamientoRepository(rel), auditoria);

        var r = await handler.ManejarAsync(new AjustarRadioCommand(jefe.UsuarioId, rel.RelevamientoId, 25m));

        r.EsExito.Should().BeTrue();
        rel.RadioAgrupacionMetros.Should().Be(25m);
        auditoria.Registros.Should().Contain(x => x.StartsWith("AJUSTAR_RADIO:"));
    }

    [Fact] // RN-02: ajustar a un valor no positivo se rechaza
    public async Task Ajustar_radio_invalido_rechaza()
    {
        var jefe = Jefe();
        var rel = Relevamiento();
        var handler = new AjustarRadioHandler(new FakeUsuarioRepository(jefe), new FakeRelevamientoRepository(rel), new FakeAuditoria());

        var r = await handler.ManejarAsync(new AjustarRadioCommand(jefe.UsuarioId, rel.RelevamientoId, -1m));

        r.Codigo.Should().Be(CodigosError.RadioInvalido);
    }

    // --- Resolución (CU-12) ---

    private static ResolverConflictoHandler ResolverHandler(
        Usuario jefe, Relevamiento rel, FakeConflictoRepository conflictos, FakeMarcadorRepository marcadores,
        FakeObservacionRepository observaciones, FakeFotoRepository fotos, FakeComentarioRepository comentarios, FakeAuditoria? auditoria = null) =>
        new(new FakeUsuarioRepository(jefe), new FakeRelevamientoRepository(rel), conflictos, marcadores,
            observaciones, fotos, comentarios, auditoria ?? new FakeAuditoria());

    [Fact] // CU-12 CA-01: unificar reasigna observaciones, fotos y comentarios y elimina el absorbido
    public async Task Resolver_unificar_reasigna_y_elimina_absorbido()
    {
        var jefe = Jefe();
        var rel = Relevamiento();
        var resultante = Marcador.Crear(rel.RelevamientoId, PuntoA);
        var absorbido = Marcador.Crear(rel.RelevamientoId, PuntoCerca);
        resultante.MarcarConflicto();
        absorbido.MarcarConflicto();

        var obs = Observacion.Georreferenciada(rel.RelevamientoId, jefe.UsuarioId, new DateTime(2026, 6, 1), absorbido.MarcadorId);
        var foto = Foto.Crear(obs.ObservacionId, absorbido.MarcadorId, tieneMetadatos: true, FuenteCoordenada.Metadatos, "f.jpg");
        var com = Comentario.Crear(absorbido.MarcadorId, foto.FotoId, jefe.UsuarioId, "fisura", new DateTime(2026, 6, 1)).Valor!;

        var conflicto = ConflictoSync.MarcadoresEnRadio(rel.RelevamientoId, resultante.MarcadorId, absorbido.MarcadorId);
        var conflictos = new FakeConflictoRepository(conflicto);
        var marcadores = new FakeMarcadorRepository(resultante, absorbido);
        var observaciones = new FakeObservacionRepository(obs);
        var fotos = new FakeFotoRepository(foto);
        var comentarios = new FakeComentarioRepository(com);
        var handler = ResolverHandler(jefe, rel, conflictos, marcadores, observaciones, fotos, comentarios);

        var r = await handler.ManejarAsync(
            new ResolverConflictoCommand(jefe.UsuarioId, conflicto.ConflictoSyncId, DecisionConflicto.Unificar, resultante.MarcadorId));

        r.EsExito.Should().BeTrue();
        obs.MarcadorId.Should().Be(resultante.MarcadorId);
        foto.MarcadorId.Should().Be(resultante.MarcadorId);
        com.MarcadorId.Should().Be(resultante.MarcadorId);
        resultante.EnConflicto.Should().BeFalse();
        (await marcadores.ObtenerPorIdAsync(absorbido.MarcadorId)).Should().BeNull();
        conflicto.EstaPendiente.Should().BeFalse();
    }

    [Fact] // CU-12: el marcador resultante debe ser uno de los del conflicto
    public async Task Resolver_unificar_con_marcador_ajeno_rechaza()
    {
        var jefe = Jefe();
        var rel = Relevamiento();
        var mA = Marcador.Crear(rel.RelevamientoId, PuntoA);
        var mB = Marcador.Crear(rel.RelevamientoId, PuntoCerca);
        var conflicto = ConflictoSync.MarcadoresEnRadio(rel.RelevamientoId, mA.MarcadorId, mB.MarcadorId);
        var handler = ResolverHandler(jefe, rel, new FakeConflictoRepository(conflicto), new FakeMarcadorRepository(mA, mB),
            new FakeObservacionRepository(), new FakeFotoRepository(), new FakeComentarioRepository());

        var r = await handler.ManejarAsync(
            new ResolverConflictoCommand(jefe.UsuarioId, conflicto.ConflictoSyncId, DecisionConflicto.Unificar, Guid.NewGuid()));

        r.Codigo.Should().Be(CodigosError.UnificacionNoAutorizada);
        conflicto.EstaPendiente.Should().BeTrue();
    }

    [Fact] // CU-12 CA-02: mantener separados conserva ambos marcadores y levanta la marca
    public async Task Resolver_mantener_separados_conserva_ambos()
    {
        var jefe = Jefe();
        var rel = Relevamiento();
        var mA = Marcador.Crear(rel.RelevamientoId, PuntoA);
        var mB = Marcador.Crear(rel.RelevamientoId, PuntoCerca);
        mA.MarcarConflicto();
        mB.MarcarConflicto();
        var conflicto = ConflictoSync.MarcadoresEnRadio(rel.RelevamientoId, mA.MarcadorId, mB.MarcadorId);
        var marcadores = new FakeMarcadorRepository(mA, mB);
        var handler = ResolverHandler(jefe, rel, new FakeConflictoRepository(conflicto), marcadores,
            new FakeObservacionRepository(), new FakeFotoRepository(), new FakeComentarioRepository());

        var r = await handler.ManejarAsync(
            new ResolverConflictoCommand(jefe.UsuarioId, conflicto.ConflictoSyncId, DecisionConflicto.MantenerSeparados, null));

        r.EsExito.Should().BeTrue();
        mA.EnConflicto.Should().BeFalse();
        mB.EnConflicto.Should().BeFalse();
        (await marcadores.ObtenerPorIdAsync(mA.MarcadorId)).Should().NotBeNull();
        (await marcadores.ObtenerPorIdAsync(mB.MarcadorId)).Should().NotBeNull();
        conflicto.EstaPendiente.Should().BeFalse();
    }

    [Fact] // CU-12 CA-03: un conflicto inexistente o ya resuelto responde CONFLICTO_INEXISTENTE
    public async Task Resolver_conflicto_inexistente_rechaza()
    {
        var jefe = Jefe();
        var rel = Relevamiento();
        var handler = ResolverHandler(jefe, rel, new FakeConflictoRepository(), new FakeMarcadorRepository(),
            new FakeObservacionRepository(), new FakeFotoRepository(), new FakeComentarioRepository());

        var r = await handler.ManejarAsync(
            new ResolverConflictoCommand(jefe.UsuarioId, Guid.NewGuid(), DecisionConflicto.MantenerSeparados, null));

        r.Codigo.Should().Be(CodigosError.ConflictoInexistente);
    }

    [Fact] // concurrencia: un conflicto ya resuelto no se resuelve de nuevo
    public async Task Resolver_conflicto_ya_resuelto_rechaza()
    {
        var jefe = Jefe();
        var rel = Relevamiento();
        var conflicto = ConflictoSync.MarcadoresEnRadio(rel.RelevamientoId, Guid.NewGuid(), Guid.NewGuid());
        conflicto.Resolver(jefe.UsuarioId);
        var handler = ResolverHandler(jefe, rel, new FakeConflictoRepository(conflicto), new FakeMarcadorRepository(),
            new FakeObservacionRepository(), new FakeFotoRepository(), new FakeComentarioRepository());

        var r = await handler.ManejarAsync(
            new ResolverConflictoCommand(jefe.UsuarioId, conflicto.ConflictoSyncId, DecisionConflicto.MantenerSeparados, null));

        r.Codigo.Should().Be(CodigosError.ConflictoInexistente);
    }

    [Fact] // RN-05: resolver sobre un relevamiento cerrado se rechaza
    public async Task Resolver_sobre_cerrado_rechaza()
    {
        var jefe = Jefe();
        var rel = Relevamiento(EstadoRelevamiento.Cerrado);
        var conflicto = ConflictoSync.MarcadoresEnRadio(rel.RelevamientoId, Guid.NewGuid(), Guid.NewGuid());
        var handler = ResolverHandler(jefe, rel, new FakeConflictoRepository(conflicto), new FakeMarcadorRepository(),
            new FakeObservacionRepository(), new FakeFotoRepository(), new FakeComentarioRepository());

        var r = await handler.ManejarAsync(
            new ResolverConflictoCommand(jefe.UsuarioId, conflicto.ConflictoSyncId, DecisionConflicto.MantenerSeparados, null));

        r.Codigo.Should().Be(CodigosError.RelevamientoSoloLectura);
    }

    // --- Listado de pendientes (CU-12 §5.A) ---

    [Fact] // la consulta lista los conflictos pendientes del área
    public async Task Listar_pendientes_devuelve_los_del_relevamiento()
    {
        var jefe = Jefe();
        var rel = Relevamiento();
        var conflicto = ConflictoSync.MarcadoresEnRadio(rel.RelevamientoId, Guid.NewGuid(), Guid.NewGuid());
        var handler = new ConflictosPendientesHandler(
            new FakeUsuarioRepository(jefe), new FakeRelevamientoRepository(rel), new FakeConflictoRepository(conflicto));

        var lista = await handler.ManejarAsync(new ConflictosPendientesQuery(jefe.UsuarioId, rel.RelevamientoId));

        lista.Should().ContainSingle().Which.ConflictoSyncId.Should().Be(conflicto.ConflictoSyncId);
    }

    [Fact] // RN-01: un jefe de otra área no ve los conflictos
    public async Task Listar_pendientes_otra_area_vacio()
    {
        var rel = Relevamiento();
        var jefeSur = Usuario.Crear("js", RolJerarquico.JefeArea, AreaSur).Valor!;
        var conflicto = ConflictoSync.MarcadoresEnRadio(rel.RelevamientoId, Guid.NewGuid(), Guid.NewGuid());
        var handler = new ConflictosPendientesHandler(
            new FakeUsuarioRepository(jefeSur), new FakeRelevamientoRepository(rel), new FakeConflictoRepository(conflicto));

        var lista = await handler.ManejarAsync(new ConflictosPendientesQuery(jefeSur.UsuarioId, rel.RelevamientoId));

        lista.Should().BeEmpty();
    }
}
