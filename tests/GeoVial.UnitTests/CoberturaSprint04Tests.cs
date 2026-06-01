using FluentAssertions;
using GeoVial.Application.Revision;
using GeoVial.Application.Servicios;
using GeoVial.Domain;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Tests complementarios de borde del Sprint 04 para alcanzar el gate de cobertura.</summary>
public class CoberturaSprint04Tests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();
    private static readonly Coordenada Punto = new(-34.6m, -58.4m);
    private static readonly DateTime Momento = new(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

    private static (Usuario Jefe, Relevamiento Rel, Marcador Marcador) Escenario(EstadoRelevamiento? estado = null)
    {
        var jefe = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var rel = Relevamiento.Crear("Obra", 15m, AreaNorte).Valor!;
        if (estado == EstadoRelevamiento.Cerrado)
        {
            rel.TransicionarA(EstadoRelevamiento.Revision);
            rel.TransicionarA(EstadoRelevamiento.Cerrado);
        }

        return (jefe, rel, Marcador.Crear(rel.RelevamientoId, Punto));
    }

    // --- Provisión de credenciales: bordes ---

    [Fact]
    public async Task Credencial_usuario_inexistente()
    {
        var admin = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        var svc = new ProvisionCredencialService(new FakeUsuarioRepository(admin), new FakeCredencialRepository(), new FakeHasher(), new FakeAuditoria());
        (await svc.EstablecerCredencialAsync(admin.UsuarioId, Guid.NewGuid(), "x", "y")).Codigo.Should().Be(CodigosError.UsuarioInexistente);
    }

    [Fact]
    public async Task Credencial_nombre_requerido()
    {
        var admin = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        var objetivo = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var svc = new ProvisionCredencialService(new FakeUsuarioRepository(admin, objetivo), new FakeCredencialRepository(), new FakeHasher(), new FakeAuditoria());
        (await svc.EstablecerCredencialAsync(admin.UsuarioId, objetivo.UsuarioId, "  ", "clave")).Codigo.Should().Be(CodigosError.NombreUsuarioRequerido);
    }

    // --- Etiquetar comentario ---

    [Fact] // CU-09: etiquetar un comentario (RC-04)
    public async Task Etiquetar_comentario_exito()
    {
        var (jefe, rel, marcador) = Escenario();
        var comentario = Comentario.Crear(marcador.MarcadorId, null, jefe.UsuarioId, "x", Momento).Valor!;
        var etiquetas = new FakeEtiquetaRepository();
        var handler = new EtiquetarComentarioHandler(
            new FakeUsuarioRepository(jefe), new FakeMarcadorRepository(marcador), new FakeRelevamientoRepository(rel),
            new FakeComentarioRepository(comentario), etiquetas, new FakeAuditoria());

        var r = await handler.ManejarAsync(new EtiquetarComentarioCommand(jefe.UsuarioId, comentario.ComentarioId, "fisura"));

        r.EsExito.Should().BeTrue();
        (await etiquetas.ListarNombresDeComentarioAsync(comentario.ComentarioId)).Should().Contain("fisura");
    }

    [Fact]
    public async Task Etiquetar_comentario_inexistente_rechaza()
    {
        var (jefe, rel, marcador) = Escenario();
        var handler = new EtiquetarComentarioHandler(
            new FakeUsuarioRepository(jefe), new FakeMarcadorRepository(marcador), new FakeRelevamientoRepository(rel),
            new FakeComentarioRepository(), new FakeEtiquetaRepository(), new FakeAuditoria());

        (await handler.ManejarAsync(new EtiquetarComentarioCommand(jefe.UsuarioId, Guid.NewGuid(), "x"))).Codigo
            .Should().Be(CodigosError.ComentarioInexistente);
    }

    [Fact]
    public async Task Etiquetar_comentario_sin_etiqueta_rechaza()
    {
        var (jefe, rel, marcador) = Escenario();
        var comentario = Comentario.Crear(marcador.MarcadorId, null, jefe.UsuarioId, "x", Momento).Valor!;
        var handler = new EtiquetarComentarioHandler(
            new FakeUsuarioRepository(jefe), new FakeMarcadorRepository(marcador), new FakeRelevamientoRepository(rel),
            new FakeComentarioRepository(comentario), new FakeEtiquetaRepository(), new FakeAuditoria());

        (await handler.ManejarAsync(new EtiquetarComentarioCommand(jefe.UsuarioId, comentario.ComentarioId, "  "))).Codigo
            .Should().Be(CodigosError.EtiquetaRequerida);
    }

    // --- Etiquetar foto: bordes ---

    [Fact] // una foto sin marcador (en bandeja) no se etiqueta
    public async Task Etiquetar_foto_sin_marcador_rechaza()
    {
        var (jefe, rel, marcador) = Escenario();
        var foto = Foto.Crear(Guid.NewGuid(), null, tieneMetadatos: false, fuente: null, "f.jpg");
        var handler = new EtiquetarFotoHandler(
            new FakeUsuarioRepository(jefe), new FakeMarcadorRepository(marcador), new FakeRelevamientoRepository(rel),
            new FakeFotoRepository(foto), new FakeEtiquetaRepository(), new FakeAuditoria());

        (await handler.ManejarAsync(new EtiquetarFotoCommand(jefe.UsuarioId, foto.FotoId, "x"))).Codigo
            .Should().Be(CodigosError.MarcadorInexistente);
    }

    [Fact]
    public async Task Etiquetar_foto_sin_etiqueta_rechaza()
    {
        var (jefe, rel, marcador) = Escenario();
        var foto = Foto.Crear(Guid.NewGuid(), marcador.MarcadorId, tieneMetadatos: true, FuenteCoordenada.Metadatos, "f.jpg");
        var handler = new EtiquetarFotoHandler(
            new FakeUsuarioRepository(jefe), new FakeMarcadorRepository(marcador), new FakeRelevamientoRepository(rel),
            new FakeFotoRepository(foto), new FakeEtiquetaRepository(), new FakeAuditoria());

        (await handler.ManejarAsync(new EtiquetarFotoCommand(jefe.UsuarioId, foto.FotoId, " "))).Codigo
            .Should().Be(CodigosError.EtiquetaRequerida);
    }

    [Fact] // RN-05: etiquetar una foto de un relevamiento cerrado se rechaza
    public async Task Etiquetar_foto_sobre_cerrado_rechaza()
    {
        var (jefe, rel, marcador) = Escenario(EstadoRelevamiento.Cerrado);
        var foto = Foto.Crear(Guid.NewGuid(), marcador.MarcadorId, tieneMetadatos: true, FuenteCoordenada.Metadatos, "f.jpg");
        var handler = new EtiquetarFotoHandler(
            new FakeUsuarioRepository(jefe), new FakeMarcadorRepository(marcador), new FakeRelevamientoRepository(rel),
            new FakeFotoRepository(foto), new FakeEtiquetaRepository(), new FakeAuditoria());

        (await handler.ManejarAsync(new EtiquetarFotoCommand(jefe.UsuarioId, foto.FotoId, "x"))).Codigo
            .Should().Be(CodigosError.RelevamientoSoloLectura);
    }

    // --- Reusar etiqueta existente ---

    [Fact] // la segunda foto reutiliza la etiqueta ya creada (no duplica el catálogo)
    public async Task Etiquetar_reusa_etiqueta_existente()
    {
        var (jefe, rel, marcador) = Escenario();
        var foto1 = Foto.Crear(Guid.NewGuid(), marcador.MarcadorId, true, FuenteCoordenada.Metadatos, "f1.jpg");
        var foto2 = Foto.Crear(Guid.NewGuid(), marcador.MarcadorId, true, FuenteCoordenada.Metadatos, "f2.jpg");
        var etiquetas = new FakeEtiquetaRepository();
        var handler = new EtiquetarFotoHandler(
            new FakeUsuarioRepository(jefe), new FakeMarcadorRepository(marcador), new FakeRelevamientoRepository(rel),
            new FakeFotoRepository(foto1, foto2), etiquetas, new FakeAuditoria());

        await handler.ManejarAsync(new EtiquetarFotoCommand(jefe.UsuarioId, foto1.FotoId, "fisura"));
        await handler.ManejarAsync(new EtiquetarFotoCommand(jefe.UsuarioId, foto2.FotoId, "fisura"));

        (await etiquetas.ListarNombresDeFotoAsync(foto1.FotoId)).Should().Contain("fisura");
        (await etiquetas.ListarNombresDeFotoAsync(foto2.FotoId)).Should().Contain("fisura");
    }

    // --- Revisión: bandeja sin georreferenciar ---

    [Fact] // CU-08: la revisión reporta las observaciones en la bandeja sin georreferenciar
    public async Task Revisar_incluye_bandeja_sin_georreferenciar()
    {
        var (jefe, rel, marcador) = Escenario();
        var obs = Observacion.EnBandejaSinGeorreferenciar(rel.RelevamientoId, jefe.UsuarioId, Momento);
        var handler = new RevisarRelevamientoHandler(
            new FakeUsuarioRepository(jefe), new FakeRelevamientoRepository(rel), new FakeMarcadorRepository(marcador),
            new FakeObservacionRepository(obs), new FakeFotoRepository(), new FakeComentarioRepository(), new FakeEtiquetaRepository());

        var revision = await handler.ManejarAsync(new RevisarRelevamientoQuery(jefe.UsuarioId, rel.RelevamientoId));

        revision!.ObservacionesSinGeorreferenciar.Should().Contain(obs.ObservacionId);
    }
}
