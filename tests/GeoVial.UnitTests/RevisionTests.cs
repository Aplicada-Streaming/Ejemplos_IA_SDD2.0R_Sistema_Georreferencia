using FluentAssertions;
using GeoVial.Application.Revision;
using GeoVial.Application.Servicios;
using GeoVial.Domain;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Provisión de credenciales (BT-23).</summary>
public class ProvisionCredencialTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();

    [Fact] // un jefe general provee la credencial de un jefe de área de su nivel administrable
    public async Task Establecer_credencial_exito_y_audita()
    {
        var admin = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        var objetivo = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var credenciales = new FakeCredencialRepository();
        var auditoria = new FakeAuditoria();
        var svc = new ProvisionCredencialService(new FakeUsuarioRepository(admin, objetivo), credenciales, new FakeHasher(), auditoria);

        var r = await svc.EstablecerCredencialAsync(admin.UsuarioId, objetivo.UsuarioId, "jefe.norte", "secreta");

        r.EsExito.Should().BeTrue();
        auditoria.Registros.Should().Contain(x => x.StartsWith("ESTABLECER_CREDENCIAL:"));
        (await credenciales.ObtenerPorNombreUsuarioAsync("jefe.norte")).Should().NotBeNull();
    }

    [Fact] // RN-01: no se puede proveer credencial a un nivel no administrable
    public async Task Establecer_credencial_no_autorizada()
    {
        var admin = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var objetivo = Usuario.Crear("otro", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var svc = new ProvisionCredencialService(new FakeUsuarioRepository(admin, objetivo), new FakeCredencialRepository(), new FakeHasher(), new FakeAuditoria());

        var r = await svc.EstablecerCredencialAsync(admin.UsuarioId, objetivo.UsuarioId, "x", "y");

        r.Codigo.Should().Be(CodigosError.AccesoNoAutorizado);
    }

    [Fact] // el nombre de usuario no se puede repetir
    public async Task Establecer_credencial_nombre_en_uso()
    {
        var admin = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        var objetivo = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var existente = new Credencial(Guid.NewGuid(), "jefe.norte", "h:x");
        var svc = new ProvisionCredencialService(new FakeUsuarioRepository(admin, objetivo), new FakeCredencialRepository(existente), new FakeHasher(), new FakeAuditoria());

        var r = await svc.EstablecerCredencialAsync(admin.UsuarioId, objetivo.UsuarioId, "jefe.norte", "secreta");

        r.Codigo.Should().Be(CodigosError.NombreUsuarioEnUso);
    }

    [Fact] // la clave es obligatoria
    public async Task Establecer_credencial_clave_requerida()
    {
        var admin = Usuario.Crear("jg", RolJerarquico.JefeGeneral, null).Valor!;
        var objetivo = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var svc = new ProvisionCredencialService(new FakeUsuarioRepository(admin, objetivo), new FakeCredencialRepository(), new FakeHasher(), new FakeAuditoria());

        var r = await svc.EstablecerCredencialAsync(admin.UsuarioId, objetivo.UsuarioId, "jefe.norte", "  ");

        r.Codigo.Should().Be(CodigosError.ClaveRequerida);
    }
}

/// <summary>Revisión sobre mapa y gestión de marcador (CU-08, CU-09; US-15/21/22).</summary>
public class RevisionTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();
    private static readonly Guid AreaSur = Guid.NewGuid();
    private static readonly Coordenada Punto = new(-34.6m, -58.4m);

    private static (Usuario Jefe, Relevamiento Rel, Marcador Marcador) Escenario(EstadoRelevamiento? estado = null)
    {
        var jefe = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var rel = Relevamiento.Crear("Puente Río 12", 15m, AreaNorte).Valor!;
        if (estado == EstadoRelevamiento.Cerrado)
        {
            rel.TransicionarA(EstadoRelevamiento.Revision);
            rel.TransicionarA(EstadoRelevamiento.Cerrado);
        }

        var marcador = Marcador.Crear(rel.RelevamientoId, Punto);
        return (jefe, rel, marcador);
    }

    private static AgregarComentarioHandler ComentarioHandler(Usuario jefe, Relevamiento rel, Marcador marcador, FakeComentarioRepository? comentarios = null, FakeAuditoria? auditoria = null) =>
        new(new FakeUsuarioRepository(jefe), new FakeMarcadorRepository(marcador), new FakeRelevamientoRepository(rel),
            comentarios ?? new FakeComentarioRepository(), auditoria ?? new FakeAuditoria(), new FakeReloj());

    [Fact] // CU-09 CA-01: agregar un comentario al marcador
    public async Task Agregar_comentario_exito()
    {
        var (jefe, rel, marcador) = Escenario();
        var comentarios = new FakeComentarioRepository();
        var handler = ComentarioHandler(jefe, rel, marcador, comentarios);

        var r = await handler.ManejarAsync(new AgregarComentarioCommand(jefe.UsuarioId, marcador.MarcadorId, null, "Fisura en la viga"));

        r.EsExito.Should().BeTrue();
        (await comentarios.ListarPorMarcadorAsync(marcador.MarcadorId)).Should().HaveCount(1);
    }

    [Fact] // CU-09 CA-03 / RN-05: comentar sobre un relevamiento cerrado se rechaza
    public async Task Agregar_comentario_sobre_cerrado_rechaza()
    {
        var (jefe, rel, marcador) = Escenario(EstadoRelevamiento.Cerrado);
        var handler = ComentarioHandler(jefe, rel, marcador);

        var r = await handler.ManejarAsync(new AgregarComentarioCommand(jefe.UsuarioId, marcador.MarcadorId, null, "x"));

        r.Codigo.Should().Be(CodigosError.RelevamientoSoloLectura);
    }

    [Fact] // el texto es obligatorio
    public async Task Agregar_comentario_sin_texto_rechaza()
    {
        var (jefe, rel, marcador) = Escenario();
        var handler = ComentarioHandler(jefe, rel, marcador);

        var r = await handler.ManejarAsync(new AgregarComentarioCommand(jefe.UsuarioId, marcador.MarcadorId, null, "   "));

        r.Codigo.Should().Be(CodigosError.TextoRequerido);
    }

    [Fact] // RN-01: un jefe de otra área no comenta el marcador
    public async Task Agregar_comentario_otra_area_rechaza()
    {
        var (_, rel, marcador) = Escenario();
        var jefeSur = Usuario.Crear("js", RolJerarquico.JefeArea, AreaSur).Valor!;
        var handler = new AgregarComentarioHandler(
            new FakeUsuarioRepository(jefeSur), new FakeMarcadorRepository(marcador), new FakeRelevamientoRepository(rel),
            new FakeComentarioRepository(), new FakeAuditoria(), new FakeReloj());

        var r = await handler.ManejarAsync(new AgregarComentarioCommand(jefeSur.UsuarioId, marcador.MarcadorId, null, "x"));

        r.Codigo.Should().Be(CodigosError.AccesoNoAutorizado);
    }

    [Fact] // marcador inexistente
    public async Task Agregar_comentario_marcador_inexistente_rechaza()
    {
        var (jefe, rel, _) = Escenario();
        var handler = new AgregarComentarioHandler(
            new FakeUsuarioRepository(jefe), new FakeMarcadorRepository(), new FakeRelevamientoRepository(rel),
            new FakeComentarioRepository(), new FakeAuditoria(), new FakeReloj());

        var r = await handler.ManejarAsync(new AgregarComentarioCommand(jefe.UsuarioId, Guid.NewGuid(), null, "x"));

        r.Codigo.Should().Be(CodigosError.MarcadorInexistente);
    }

    [Fact] // CU-09 CA-01: etiquetar una foto del marcador (RC-04 muchos a muchos)
    public async Task Etiquetar_foto_exito()
    {
        var (jefe, rel, marcador) = Escenario();
        var foto = Foto.Crear(Guid.NewGuid(), marcador.MarcadorId, tieneMetadatos: true, FuenteCoordenada.Metadatos, "f.jpg");
        var etiquetas = new FakeEtiquetaRepository();
        var handler = new EtiquetarFotoHandler(
            new FakeUsuarioRepository(jefe), new FakeMarcadorRepository(marcador), new FakeRelevamientoRepository(rel),
            new FakeFotoRepository(foto), etiquetas, new FakeAuditoria());

        var r = await handler.ManejarAsync(new EtiquetarFotoCommand(jefe.UsuarioId, foto.FotoId, "fisura"));

        r.EsExito.Should().BeTrue();
        (await etiquetas.ListarNombresDeFotoAsync(foto.FotoId)).Should().Contain("fisura");
    }

    [Fact] // foto inexistente
    public async Task Etiquetar_foto_inexistente_rechaza()
    {
        var (jefe, rel, marcador) = Escenario();
        var handler = new EtiquetarFotoHandler(
            new FakeUsuarioRepository(jefe), new FakeMarcadorRepository(marcador), new FakeRelevamientoRepository(rel),
            new FakeFotoRepository(), new FakeEtiquetaRepository(), new FakeAuditoria());

        var r = await handler.ManejarAsync(new EtiquetarFotoCommand(jefe.UsuarioId, Guid.NewGuid(), "x"));

        r.Codigo.Should().Be(CodigosError.FotoInexistente);
    }

    [Fact] // CU-08 CA-01: la revisión devuelve los marcadores con sus fotos, comentarios y etiquetas
    public async Task Revisar_devuelve_marcadores_con_contenido()
    {
        var (jefe, rel, marcador) = Escenario();
        var foto = Foto.Crear(Guid.NewGuid(), marcador.MarcadorId, tieneMetadatos: true, FuenteCoordenada.Metadatos, "f.jpg");
        var comentario = Comentario.Crear(marcador.MarcadorId, foto.FotoId, jefe.UsuarioId, "Fisura", new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc)).Valor!;
        var etiquetas = new FakeEtiquetaRepository();
        var etiqueta = Etiqueta.Crear("fisura").Valor!;
        await etiquetas.AgregarAsync(etiqueta);
        await etiquetas.AgregarFotoEtiquetaAsync(new FotoEtiqueta(foto.FotoId, etiqueta.EtiquetaId));

        var handler = new RevisarRelevamientoHandler(
            new FakeUsuarioRepository(jefe), new FakeRelevamientoRepository(rel), new FakeMarcadorRepository(marcador),
            new FakeObservacionRepository(), new FakeFotoRepository(foto), new FakeComentarioRepository(comentario), etiquetas);

        var revision = await handler.ManejarAsync(new RevisarRelevamientoQuery(jefe.UsuarioId, rel.RelevamientoId));

        revision.Should().NotBeNull();
        revision!.Marcadores.Should().ContainSingle();
        var rm = revision.Marcadores[0];
        rm.Fotos.Should().ContainSingle().Which.Etiquetas.Should().Contain("fisura");
        rm.Comentarios.Should().ContainSingle().Which.Texto.Should().Be("Fisura");
    }

    [Fact] // S50 / RN-03: la revisión enriquece la bandeja sin georreferenciar con momento y referencia de foto
    public async Task Revisar_devuelve_bandeja_enriquecida()
    {
        var (jefe, rel, _) = Escenario();
        var momento = new DateTime(2026, 6, 1, 9, 30, 0, DateTimeKind.Utc);
        var obs = Observacion.EnBandejaSinGeorreferenciar(rel.RelevamientoId, jefe.UsuarioId, momento);
        var foto = Foto.Crear(obs.ObservacionId, null, tieneMetadatos: false, fuente: null, "obra/sin-gps.jpg");
        var handler = new RevisarRelevamientoHandler(
            new FakeUsuarioRepository(jefe), new FakeRelevamientoRepository(rel), new FakeMarcadorRepository(),
            new FakeObservacionRepository(obs), new FakeFotoRepository(foto), new FakeComentarioRepository(), new FakeEtiquetaRepository());

        var revision = await handler.ManejarAsync(new RevisarRelevamientoQuery(jefe.UsuarioId, rel.RelevamientoId));

        revision.Should().NotBeNull();
        revision!.ObservacionesSinGeorreferenciar.Should().ContainSingle().Which.Should().Be(obs.ObservacionId);
        var entrada = revision.Bandeja.Should().ContainSingle().Subject;
        entrada.ObservacionId.Should().Be(obs.ObservacionId);
        entrada.MomentoCaptura.Should().Be(momento);
        entrada.ReferenciaArchivo.Should().Be("obra/sin-gps.jpg");
    }

    [Fact] // CU-08 CA-03 / RN-01: un jefe de otra área no revisa el relevamiento
    public async Task Revisar_otra_area_devuelve_null()
    {
        var (_, rel, marcador) = Escenario();
        var jefeSur = Usuario.Crear("js", RolJerarquico.JefeArea, AreaSur).Valor!;
        var handler = new RevisarRelevamientoHandler(
            new FakeUsuarioRepository(jefeSur), new FakeRelevamientoRepository(rel), new FakeMarcadorRepository(marcador),
            new FakeObservacionRepository(), new FakeFotoRepository(), new FakeComentarioRepository(), new FakeEtiquetaRepository());

        var revision = await handler.ManejarAsync(new RevisarRelevamientoQuery(jefeSur.UsuarioId, rel.RelevamientoId));

        revision.Should().BeNull();
    }
}
