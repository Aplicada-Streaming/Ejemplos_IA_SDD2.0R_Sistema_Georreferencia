using FluentAssertions;
using GeoVial.Application.Revision;
using GeoVial.Domain;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>US-15 / CU-09 §5.A — Quitar una foto de su marcador: borra foto + observación + binario y desvincula sus comentarios.</summary>
public class EliminarFotoHandlerTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();
    private static readonly Coordenada Punto = new(-34.6m, -58.4m);
    private static readonly DateTime Momento = new(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

    private sealed record Escenario(
        Usuario Jefe, Relevamiento Rel, Marcador Marcador, Observacion Obs, Foto Foto,
        FakeFotoRepository Fotos, FakeObservacionRepository Observaciones, FakeComentarioRepository Comentarios,
        FakeAlmacenFotos Almacen, FakeAuditoria Auditoria, EliminarFotoHandler Handler);

    private static Escenario Armar(EstadoRelevamiento? estado = null, params Comentario[] comentarios)
    {
        var jefe = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var rel = Relevamiento.Crear("Obra", 15m, AreaNorte).Valor!;
        if (estado == EstadoRelevamiento.Cerrado)
        {
            rel.TransicionarA(EstadoRelevamiento.Revision);
            rel.TransicionarA(EstadoRelevamiento.Cerrado);
        }

        var marcador = Marcador.Crear(rel.RelevamientoId, Punto);
        var obs = Observacion.Georreferenciada(rel.RelevamientoId, jefe.UsuarioId, Momento, marcador.MarcadorId);
        var foto = Foto.Crear(obs.ObservacionId, marcador.MarcadorId, tieneMetadatos: true, FuenteCoordenada.Metadatos, "ref-x.jpg");

        var fotos = new FakeFotoRepository(foto);
        var observaciones = new FakeObservacionRepository(obs);
        var comentariosRepo = new FakeComentarioRepository(comentarios);
        var almacen = new FakeAlmacenFotos();
        almacen.Sembrar("ref-x.jpg", new byte[] { 1, 2, 3 });
        var auditoria = new FakeAuditoria();

        var handler = new EliminarFotoHandler(
            new FakeUsuarioRepository(jefe), new FakeMarcadorRepository(marcador), new FakeRelevamientoRepository(rel),
            fotos, observaciones, comentariosRepo, new FakeEtiquetaRepository(), almacen, auditoria);

        return new Escenario(jefe, rel, marcador, obs, foto, fotos, observaciones, comentariosRepo, almacen, auditoria, handler);
    }

    [Fact] // borra la foto, su observación y el binario; audita la acción
    public async Task Quitar_foto_borra_foto_observacion_y_binario()
    {
        var e = Armar();

        var r = await e.Handler.ManejarAsync(new EliminarFotoCommand(e.Jefe.UsuarioId, e.Foto.FotoId));

        r.EsExito.Should().BeTrue();
        (await e.Fotos.ObtenerPorIdAsync(e.Foto.FotoId)).Should().BeNull();
        (await e.Observaciones.ObtenerPorIdAsync(e.Obs.ObservacionId)).Should().BeNull();
        e.Almacen.Datos.Should().NotContainKey("ref-x.jpg"); // binario eliminado
        e.Auditoria.Registros.Should().Contain(reg => reg.StartsWith("ELIMINAR_FOTO:"));
    }

    [Fact] // CU-09 §5.A: el comentario que referenciaba la foto sobrevive a nivel marcador (se desvincula, no se borra)
    public async Task Quitar_foto_desvincula_sus_comentarios()
    {
        // se arma el escenario aparte para conocer el MarcadorId al crear los comentarios
        var jefe = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var rel = Relevamiento.Crear("Obra", 15m, AreaNorte).Valor!;
        var marcador = Marcador.Crear(rel.RelevamientoId, Punto);
        var obs = Observacion.Georreferenciada(rel.RelevamientoId, jefe.UsuarioId, Momento, marcador.MarcadorId);
        var foto = Foto.Crear(obs.ObservacionId, marcador.MarcadorId, true, FuenteCoordenada.Metadatos, "ref-x.jpg");
        var comentarioDeFoto = Comentario.Crear(marcador.MarcadorId, foto.FotoId, jefe.UsuarioId, "fisura en la foto", Momento).Valor!;
        var comentarioDeMarcador = Comentario.Crear(marcador.MarcadorId, null, jefe.UsuarioId, "nota del punto", Momento).Valor!;
        var comentarios = new FakeComentarioRepository(comentarioDeFoto, comentarioDeMarcador);
        var almacen = new FakeAlmacenFotos();
        var handler = new EliminarFotoHandler(
            new FakeUsuarioRepository(jefe), new FakeMarcadorRepository(marcador), new FakeRelevamientoRepository(rel),
            new FakeFotoRepository(foto), new FakeObservacionRepository(obs), comentarios, new FakeEtiquetaRepository(), almacen, new FakeAuditoria());

        var r = await handler.ManejarAsync(new EliminarFotoCommand(jefe.UsuarioId, foto.FotoId));

        r.EsExito.Should().BeTrue();
        (await comentarios.ObtenerPorIdAsync(comentarioDeFoto.ComentarioId))!.FotoId.Should().BeNull(); // desvinculado
        (await comentarios.ObtenerPorIdAsync(comentarioDeMarcador.ComentarioId))!.Texto.Should().Be("nota del punto"); // intacto
    }

    [Fact]
    public async Task Quitar_foto_inexistente_rechaza()
    {
        var e = Armar();

        (await e.Handler.ManejarAsync(new EliminarFotoCommand(e.Jefe.UsuarioId, Guid.NewGuid()))).Codigo
            .Should().Be(CodigosError.FotoInexistente);
    }

    [Fact] // una foto sin marcador (bandeja sin georreferenciar) no es "quitar de un marcador"
    public async Task Quitar_foto_sin_marcador_rechaza()
    {
        var jefe = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var rel = Relevamiento.Crear("Obra", 15m, AreaNorte).Valor!;
        var marcador = Marcador.Crear(rel.RelevamientoId, Punto);
        var foto = Foto.Crear(Guid.NewGuid(), null, tieneMetadatos: false, fuente: null, "f.jpg");
        var handler = new EliminarFotoHandler(
            new FakeUsuarioRepository(jefe), new FakeMarcadorRepository(marcador), new FakeRelevamientoRepository(rel),
            new FakeFotoRepository(foto), new FakeObservacionRepository(), new FakeComentarioRepository(), new FakeEtiquetaRepository(),
            new FakeAlmacenFotos(), new FakeAuditoria());

        (await handler.ManejarAsync(new EliminarFotoCommand(jefe.UsuarioId, foto.FotoId))).Codigo
            .Should().Be(CodigosError.MarcadorInexistente);
    }

    [Fact] // RN-05: no se puede quitar una foto de un relevamiento cerrado
    public async Task Quitar_foto_sobre_cerrado_rechaza()
    {
        var e = Armar(EstadoRelevamiento.Cerrado);

        (await e.Handler.ManejarAsync(new EliminarFotoCommand(e.Jefe.UsuarioId, e.Foto.FotoId))).Codigo
            .Should().Be(CodigosError.RelevamientoSoloLectura);
        (await e.Fotos.ObtenerPorIdAsync(e.Foto.FotoId)).Should().NotBeNull(); // no se borró nada
    }

    [Fact] // RN-01: un usuario de otra área no puede quitar la foto
    public async Task Quitar_foto_no_autorizado_rechaza()
    {
        var e = Armar();
        var ajeno = Usuario.Crear("otro", RolJerarquico.JefeArea, Guid.NewGuid()).Valor!;
        var handler = new EliminarFotoHandler(
            new FakeUsuarioRepository(ajeno), new FakeMarcadorRepository(e.Marcador), new FakeRelevamientoRepository(e.Rel),
            e.Fotos, e.Observaciones, e.Comentarios, new FakeEtiquetaRepository(), e.Almacen, e.Auditoria);

        (await handler.ManejarAsync(new EliminarFotoCommand(ajeno.UsuarioId, e.Foto.FotoId))).Codigo
            .Should().Be(CodigosError.AccesoNoAutorizado);
    }
}
