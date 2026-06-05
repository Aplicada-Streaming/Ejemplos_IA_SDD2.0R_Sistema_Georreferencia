using FluentAssertions;
using GeoVial.Application.Captura;
using GeoVial.Domain;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Dominio de captura: distancia, agrupación por radio (RN-02) y bandeja sin georreferenciar (RN-03).</summary>
public class CapturaDominioTests
{
    private static readonly Coordenada Base = new(-34.6000m, -58.4000m);

    [Fact]
    public void Distancia_de_un_punto_a_si_mismo_es_cero() =>
        Base.DistanciaMetrosA(Base).Should().BeLessThan(0.001);

    [Fact] // CU-04 CA-02: una coordenada cercana cae dentro del radio y reutiliza el marcador
    public void Marcador_dentro_del_radio_se_reutiliza()
    {
        var rel = Guid.NewGuid();
        var marcador = Marcador.Crear(rel, Base);
        var cercana = new Coordenada(-34.60007m, -58.4000m); // ~7,8 m

        AgrupacionMarcador.MarcadorEnRadio(new[] { marcador }, cercana, 15m).Should().Be(marcador);
    }

    [Fact] // CU-04 §5.B: una coordenada lejana no reutiliza marcador (inicia uno nuevo)
    public void Marcador_fuera_del_radio_no_se_reutiliza()
    {
        var rel = Guid.NewGuid();
        var marcador = Marcador.Crear(rel, Base);
        var lejana = new Coordenada(-34.6003m, -58.4000m); // ~33 m

        AgrupacionMarcador.MarcadorEnRadio(new[] { marcador }, lejana, 15m).Should().BeNull();
    }

    [Fact]
    public void Observacion_georreferenciada_tiene_marcador()
    {
        var o = Observacion.Georreferenciada(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid());
        o.SinGeorreferenciar.Should().BeFalse();
        o.MarcadorId.Should().NotBeNull();
    }

    [Fact] // RC-02: asociar un marcador saca la observación de la bandeja
    public void Observacion_en_bandeja_y_asociar_marcador()
    {
        var o = Observacion.EnBandejaSinGeorreferenciar(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
        o.SinGeorreferenciar.Should().BeTrue();
        var marcador = Guid.NewGuid();
        o.AsociarMarcador(marcador);
        o.SinGeorreferenciar.Should().BeFalse();
        o.MarcadorId.Should().Be(marcador);
    }
}

/// <summary>Manejadores del módulo de captura (CU-04, CU-05; RN-01/RN-02/RN-03/RN-05/RN-07).</summary>
public class CapturaAplicacionTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();
    private static readonly Coordenada Punto = new(-34.6000m, -58.4000m);

    private static (Usuario Agente, Relevamiento Relevamiento) Escenario(EstadoRelevamiento? estado = null, bool asignar = true)
    {
        var agente = Usuario.Crear("agente", RolJerarquico.AgenteCampo, AreaNorte).Valor!;
        var rel = Relevamiento.Crear("Puente Río 12", 15m, AreaNorte).Valor!;
        if (asignar)
        {
            rel.AsignarAgente(agente);
        }

        if (estado == EstadoRelevamiento.Cerrado)
        {
            rel.TransicionarA(EstadoRelevamiento.Revision);
            rel.TransicionarA(EstadoRelevamiento.Cerrado);
        }

        return (agente, rel);
    }

    private static CapturarObservacionHandler CapturaHandler(
        Usuario agente, Relevamiento rel, FakeMarcadorRepository? marcadores = null, FakeAuditoria? auditoria = null) =>
        new(new FakeRelevamientoRepository(rel), new FakeUsuarioRepository(agente),
            marcadores ?? new FakeMarcadorRepository(), new FakeObservacionRepository(), new FakeFotoRepository(),
            auditoria ?? new FakeAuditoria(), new FakeReloj());

    [Fact] // CU-04 CA-01: captura con metadatos crea marcador y georreferencia
    public async Task Captura_con_metadatos_georreferencia()
    {
        var (agente, rel) = Escenario();
        var handler = CapturaHandler(agente, rel);

        var r = await handler.ManejarAsync(new CapturarObservacionCommand(agente.UsuarioId, rel.RelevamientoId, "foto1.jpg", Punto.Latitud, Punto.Longitud));

        r.EsExito.Should().BeTrue();
        r.Valor!.SinGeorreferenciar.Should().BeFalse();
        r.Valor!.MarcadorId.Should().NotBeNull();
    }

    [Fact] // S15 / ADR-08: la captura expone el FotoId de la foto creada para encadenar la subida de su binario
    public async Task Captura_devuelve_el_foto_id()
    {
        var (agente, rel) = Escenario();
        var handler = CapturaHandler(agente, rel);

        var r = await handler.ManejarAsync(new CapturarObservacionCommand(agente.UsuarioId, rel.RelevamientoId, "foto.jpg", Punto.Latitud, Punto.Longitud));

        r.EsExito.Should().BeTrue();
        r.Valor!.FotoId.Should().NotBe(Guid.Empty);
    }

    [Fact] // CU-04 CA-02: una segunda captura dentro del radio reutiliza el marcador existente
    public async Task Captura_dentro_del_radio_reutiliza_marcador()
    {
        var (agente, rel) = Escenario();
        var marcadorExistente = Marcador.Crear(rel.RelevamientoId, Punto);
        var handler = CapturaHandler(agente, rel, new FakeMarcadorRepository(marcadorExistente));

        var cercano = new Coordenada(-34.60007m, -58.4000m); // ~7,8 m
        var r = await handler.ManejarAsync(new CapturarObservacionCommand(agente.UsuarioId, rel.RelevamientoId, "foto2.jpg", cercano.Latitud, cercano.Longitud));

        r.Valor!.MarcadorId.Should().Be(marcadorExistente.MarcadorId);
    }

    [Fact] // CU-04 CA-03: captura sin metadatos cae en la bandeja sin georreferenciar
    public async Task Captura_sin_metadatos_va_a_bandeja()
    {
        var (agente, rel) = Escenario();
        var handler = CapturaHandler(agente, rel);

        var r = await handler.ManejarAsync(new CapturarObservacionCommand(agente.UsuarioId, rel.RelevamientoId, "foto3.jpg", null, null));

        r.EsExito.Should().BeTrue();
        r.Valor!.SinGeorreferenciar.Should().BeTrue();
        r.Valor!.MarcadorId.Should().BeNull();
    }

    [Fact] // RN-01: un agente no asignado no puede capturar
    public async Task Captura_por_agente_no_asignado_rechaza()
    {
        var (agente, rel) = Escenario(asignar: false);
        var handler = CapturaHandler(agente, rel);

        var r = await handler.ManejarAsync(new CapturarObservacionCommand(agente.UsuarioId, rel.RelevamientoId, "f.jpg", Punto.Latitud, Punto.Longitud));

        r.Codigo.Should().Be(CodigosError.AccesoNoAutorizado);
    }

    [Fact] // RN-05: captura sobre un relevamiento cerrado se rechaza
    public async Task Captura_sobre_cerrado_rechaza()
    {
        var (agente, rel) = Escenario(EstadoRelevamiento.Cerrado);
        var handler = CapturaHandler(agente, rel);

        var r = await handler.ManejarAsync(new CapturarObservacionCommand(agente.UsuarioId, rel.RelevamientoId, "f.jpg", Punto.Latitud, Punto.Longitud));

        r.Codigo.Should().Be(CodigosError.RelevamientoSoloLectura);
    }

    [Fact] // S46: reenviar la misma CapturaId devuelve la observación original (mismo ObservacionId y FotoId)
    public async Task Reenvio_con_misma_captura_id_devuelve_la_original()
    {
        var (agente, rel) = Escenario();
        var handler = CapturaHandler(agente, rel);
        var capturaId = Guid.NewGuid();
        var cmd = new CapturarObservacionCommand(agente.UsuarioId, rel.RelevamientoId, "foto.jpg", Punto.Latitud, Punto.Longitud, capturaId);

        var primera = await handler.ManejarAsync(cmd);
        var reenvio = await handler.ManejarAsync(cmd);

        reenvio.EsExito.Should().BeTrue();
        reenvio.Valor!.ObservacionId.Should().Be(primera.Valor!.ObservacionId);
        reenvio.Valor!.FotoId.Should().Be(primera.Valor!.FotoId);
        reenvio.Valor!.MarcadorId.Should().Be(primera.Valor!.MarcadorId);
    }

    [Fact] // S46: el reenvío idempotente no crea una segunda observación ni reaudita la captura
    public async Task Reenvio_con_misma_captura_id_no_duplica_ni_reaudita()
    {
        var (agente, rel) = Escenario();
        var observaciones = new FakeObservacionRepository();
        var fotos = new FakeFotoRepository();
        var auditoria = new FakeAuditoria();
        var handler = new CapturarObservacionHandler(
            new FakeRelevamientoRepository(rel), new FakeUsuarioRepository(agente),
            new FakeMarcadorRepository(), observaciones, fotos, auditoria, new FakeReloj());
        var cmd = new CapturarObservacionCommand(agente.UsuarioId, rel.RelevamientoId, "foto.jpg", Punto.Latitud, Punto.Longitud, Guid.NewGuid());

        await handler.ManejarAsync(cmd);
        await handler.ManejarAsync(cmd);

        (await observaciones.ListarPorRelevamientoAsync(rel.RelevamientoId)).Should().HaveCount(1);
        auditoria.Registros.Count(r => r.StartsWith("CAPTURA_OBSERVACION")).Should().Be(1);
    }

    [Fact] // S46 (compatibilidad): sin CapturaId no hay dedup — dos capturas crean dos observaciones (comportamiento previo)
    public async Task Sin_captura_id_no_hay_dedup()
    {
        var (agente, rel) = Escenario();
        var handler = CapturaHandler(agente, rel);
        var cmd = new CapturarObservacionCommand(agente.UsuarioId, rel.RelevamientoId, "foto.jpg", Punto.Latitud, Punto.Longitud);

        var primera = await handler.ManejarAsync(cmd);
        var segunda = await handler.ManejarAsync(cmd);

        segunda.Valor!.ObservacionId.Should().NotBe(primera.Valor!.ObservacionId);
    }

    [Fact] // CU-05 CA-01: ubicación manual de una observación sin georreferenciar
    public async Task Ubicar_manual_georreferencia_la_observacion()
    {
        var (agente, rel) = Escenario();
        var observacion = Observacion.EnBandejaSinGeorreferenciar(rel.RelevamientoId, agente.UsuarioId, new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc));
        var foto = Foto.Crear(observacion.ObservacionId, null, tieneMetadatos: false, fuente: null, "f.jpg");
        var handler = new UbicarObservacionManualHandler(
            new FakeRelevamientoRepository(rel), new FakeUsuarioRepository(agente),
            new FakeMarcadorRepository(), new FakeObservacionRepository(observacion), new FakeFotoRepository(foto), new FakeAuditoria());

        var r = await handler.ManejarAsync(new UbicarObservacionManualCommand(agente.UsuarioId, observacion.ObservacionId, Punto.Latitud, Punto.Longitud));

        r.EsExito.Should().BeTrue();
        observacion.SinGeorreferenciar.Should().BeFalse();
        observacion.MarcadorId.Should().NotBeNull();
    }

    [Fact] // CU-05 CA-02 / RN-03: no se ubica manualmente una foto que trae metadatos
    public async Task Ubicar_manual_con_metadatos_rechaza()
    {
        var (agente, rel) = Escenario();
        var observacion = Observacion.EnBandejaSinGeorreferenciar(rel.RelevamientoId, agente.UsuarioId, new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc));
        var foto = Foto.Crear(observacion.ObservacionId, null, tieneMetadatos: true, FuenteCoordenada.Metadatos, "f.jpg");
        var handler = new UbicarObservacionManualHandler(
            new FakeRelevamientoRepository(rel), new FakeUsuarioRepository(agente),
            new FakeMarcadorRepository(), new FakeObservacionRepository(observacion), new FakeFotoRepository(foto), new FakeAuditoria());

        var r = await handler.ManejarAsync(new UbicarObservacionManualCommand(agente.UsuarioId, observacion.ObservacionId, Punto.Latitud, Punto.Longitud));

        r.Codigo.Should().Be(CodigosError.FuenteUbicacionIncorrecta);
    }

    [Fact] // observación inexistente
    public async Task Ubicar_manual_observacion_inexistente_rechaza()
    {
        var (agente, rel) = Escenario();
        var handler = new UbicarObservacionManualHandler(
            new FakeRelevamientoRepository(rel), new FakeUsuarioRepository(agente),
            new FakeMarcadorRepository(), new FakeObservacionRepository(), new FakeFotoRepository(), new FakeAuditoria());

        var r = await handler.ManejarAsync(new UbicarObservacionManualCommand(agente.UsuarioId, Guid.NewGuid(), Punto.Latitud, Punto.Longitud));

        r.Codigo.Should().Be(CodigosError.ObservacionInexistente);
    }
}
