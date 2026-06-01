using FluentAssertions;
using GeoVial.Application.Captura;
using GeoVial.Application.Revision;
using GeoVial.Domain;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Filtrado de la revisión por etiquetas (US-23, CU-08 §5.C).</summary>
public class RevisionFiltroTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();

    private sealed record Escenario(Usuario Jefe, Relevamiento Rel, Guid MarcadorFisura, Guid MarcadorGrieta,
        FakeUsuarioRepository Usuarios, FakeRelevamientoRepository Relevamientos, FakeMarcadorRepository Marcadores,
        FakeFotoRepository Fotos, FakeEtiquetaRepository Etiquetas);

    private static async Task<Escenario> ArmarAsync()
    {
        var jefe = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var rel = Relevamiento.Crear("Puente Río 12", 15m, AreaNorte).Valor!;
        var mFisura = Marcador.Crear(rel.RelevamientoId, new Coordenada(-34.60m, -58.40m));
        var mGrieta = Marcador.Crear(rel.RelevamientoId, new Coordenada(-34.70m, -58.50m));
        var fFisura = Foto.Crear(Guid.NewGuid(), mFisura.MarcadorId, tieneMetadatos: true, FuenteCoordenada.Metadatos, "fisura.jpg");
        var fGrieta = Foto.Crear(Guid.NewGuid(), mGrieta.MarcadorId, tieneMetadatos: true, FuenteCoordenada.Metadatos, "grieta.jpg");

        var etiquetas = new FakeEtiquetaRepository();
        var etFisura = Etiqueta.Crear("fisura").Valor!;
        var etGrieta = Etiqueta.Crear("grieta").Valor!;
        await etiquetas.AgregarAsync(etFisura);
        await etiquetas.AgregarAsync(etGrieta);
        await etiquetas.AgregarFotoEtiquetaAsync(new FotoEtiqueta(fFisura.FotoId, etFisura.EtiquetaId));
        await etiquetas.AgregarFotoEtiquetaAsync(new FotoEtiqueta(fGrieta.FotoId, etGrieta.EtiquetaId));

        return new Escenario(jefe, rel, mFisura.MarcadorId, mGrieta.MarcadorId,
            new FakeUsuarioRepository(jefe), new FakeRelevamientoRepository(rel), new FakeMarcadorRepository(mFisura, mGrieta),
            new FakeFotoRepository(fFisura, fGrieta), etiquetas);
    }

    private static RevisarRelevamientoHandler Handler(Escenario e) =>
        new(e.Usuarios, e.Relevamientos, e.Marcadores, new FakeObservacionRepository(), e.Fotos, new FakeComentarioRepository(), e.Etiquetas);

    [Fact] // US-23 CA-01: filtrar por "fisura" deja solo el marcador con esa etiqueta
    public async Task Filtrar_por_etiqueta_muestra_solo_coincidencias()
    {
        var e = await ArmarAsync();
        var revision = await Handler(e).ManejarAsync(
            new RevisarRelevamientoQuery(e.Jefe.UsuarioId, e.Rel.RelevamientoId, new[] { "fisura" }));

        revision.Should().NotBeNull();
        revision!.Marcadores.Should().ContainSingle().Which.MarcadorId.Should().Be(e.MarcadorFisura);
        revision.Marcadores[0].Fotos.Should().ContainSingle();
    }

    [Fact] // US-23 CA-02: un filtro sin coincidencias devuelve la revisión vacía
    public async Task Filtrar_sin_coincidencias_vuelve_vacio()
    {
        var e = await ArmarAsync();
        var revision = await Handler(e).ManejarAsync(
            new RevisarRelevamientoQuery(e.Jefe.UsuarioId, e.Rel.RelevamientoId, new[] { "inexistente" }));

        revision.Should().NotBeNull();
        revision!.Marcadores.Should().BeEmpty();
    }

    [Fact] // sin filtro se devuelven todos los marcadores
    public async Task Sin_filtro_devuelve_todos()
    {
        var e = await ArmarAsync();
        var revision = await Handler(e).ManejarAsync(new RevisarRelevamientoQuery(e.Jefe.UsuarioId, e.Rel.RelevamientoId));

        revision!.Marcadores.Should().HaveCount(2);
    }
}

/// <summary>Descarga del binario de una foto para el visor a pantalla completa (US-24, CU-09).</summary>
public class DescargarContenidoFotoTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();
    private static readonly Guid AreaSur = Guid.NewGuid();
    private static readonly byte[] Binario = { 5, 6, 7, 8 };

    private sealed record Escenario(Usuario Jefe, Foto Foto, FakeUsuarioRepository Usuarios,
        FakeRelevamientoRepository Relevamientos, FakeObservacionRepository Observaciones, FakeFotoRepository Fotos, FakeAlmacenFotos Almacen);

    private static Escenario Armar(bool conReferencia = true)
    {
        var jefe = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var rel = Relevamiento.Crear("Obra", 15m, AreaNorte).Valor!;
        var obs = Observacion.Georreferenciada(rel.RelevamientoId, jefe.UsuarioId, new DateTime(2026, 6, 1), Guid.NewGuid());
        var almacen = new FakeAlmacenFotos();
        var referencia = conReferencia ? "ref-foto" : string.Empty;
        if (conReferencia)
        {
            almacen.Sembrar(referencia, Binario);
        }

        var foto = Foto.Crear(obs.ObservacionId, obs.MarcadorId, tieneMetadatos: true, FuenteCoordenada.Metadatos, referencia);
        return new Escenario(jefe, foto, new FakeUsuarioRepository(jefe), new FakeRelevamientoRepository(rel),
            new FakeObservacionRepository(obs), new FakeFotoRepository(foto), almacen);
    }

    private static DescargarContenidoFotoHandler Handler(Escenario e, Usuario? solicitante = null) =>
        new(new FakeUsuarioRepository(solicitante ?? e.Jefe), e.Relevamientos, e.Observaciones, e.Fotos, e.Almacen);

    [Fact] // US-24: el jefe del área descarga el binario de la foto
    public async Task Descargar_devuelve_el_binario()
    {
        var e = Armar();
        var contenido = await Handler(e).ManejarAsync(new DescargarContenidoFotoQuery(e.Jefe.UsuarioId, e.Foto.FotoId));
        contenido.Should().Equal(Binario);
    }

    [Fact] // RN-01: un jefe de otra área no descarga el binario
    public async Task Descargar_otra_area_devuelve_null()
    {
        var e = Armar();
        var jefeSur = Usuario.Crear("js", RolJerarquico.JefeArea, AreaSur).Valor!;
        var contenido = await Handler(e, jefeSur).ManejarAsync(new DescargarContenidoFotoQuery(jefeSur.UsuarioId, e.Foto.FotoId));
        contenido.Should().BeNull();
    }

    [Fact] // una foto sin contenido subido no devuelve binario
    public async Task Descargar_foto_sin_referencia_devuelve_null()
    {
        var e = Armar(conReferencia: false);
        var contenido = await Handler(e).ManejarAsync(new DescargarContenidoFotoQuery(e.Jefe.UsuarioId, e.Foto.FotoId));
        contenido.Should().BeNull();
    }
}
