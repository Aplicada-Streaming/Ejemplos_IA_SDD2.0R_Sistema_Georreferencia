using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using FluentAssertions;
using GeoVial.Application.Captura;
using GeoVial.Domain;
using GeoVial.FileHosting;
using GeoVial.Infrastructure.Alojamiento;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Backend de alojamiento local sobre el sistema de archivos (ADR-08, BT-20).</summary>
public sealed class AlmacenLocalTests : IDisposable
{
    private readonly string _raiz = Path.Combine(Path.GetTempPath(), $"geovial-almacen-{Guid.NewGuid():N}");

    [Fact] // Guardar devuelve una referencia y Recuperar trae el mismo binario
    public async Task Guardar_y_recuperar_round_trip()
    {
        var almacen = new AlmacenLocal(_raiz);
        var contenido = new byte[] { 1, 2, 3, 4, 5 };

        var referencia = await almacen.GuardarAsync("foto.jpg", contenido);

        referencia.Should().NotBeNullOrWhiteSpace();
        (await almacen.RecuperarAsync(referencia)).Should().Equal(contenido);
        (await almacen.ExisteAsync(referencia)).Should().BeTrue();
    }

    [Fact] // Recuperar una referencia inexistente devuelve null
    public async Task Recuperar_inexistente_devuelve_null() =>
        (await new AlmacenLocal(_raiz).RecuperarAsync("no-existe")).Should().BeNull();

    [Fact] // Eliminar quita el binario (idempotente)
    public async Task Eliminar_quita_el_binario()
    {
        var almacen = new AlmacenLocal(_raiz);
        var referencia = await almacen.GuardarAsync("foto.jpg", new byte[] { 1 });

        await almacen.EliminarAsync(referencia);
        await almacen.EliminarAsync(referencia); // idempotente: no falla la segunda vez

        (await almacen.ExisteAsync(referencia)).Should().BeFalse();
    }

    public void Dispose()
    {
        if (Directory.Exists(_raiz))
        {
            Directory.Delete(_raiz, recursive: true);
        }
    }
}

/// <summary>Backend de alojamiento sobre AWS S3 con cliente inyectado y testeable (ADR-08, BT-20).</summary>
public class AlmacenS3Tests
{
    private const string Bucket = "geovial-fotos";

    [Fact] // Guardar sube el objeto al bucket y devuelve la clave
    public async Task Guardar_sube_al_bucket_y_devuelve_clave()
    {
        var s3 = Substitute.For<IAmazonS3>();
        var almacen = new AlmacenS3(s3, Bucket);

        var referencia = await almacen.GuardarAsync("foto.jpg", new byte[] { 1, 2, 3 });

        referencia.Should().EndWith("-foto.jpg");
        await s3.Received(1).PutObjectAsync(
            Arg.Is<PutObjectRequest>(r => r.BucketName == Bucket && r.Key == referencia), Arg.Any<CancellationToken>());
    }

    [Fact] // Recuperar trae el binario del objeto
    public async Task Recuperar_trae_el_binario()
    {
        var s3 = Substitute.For<IAmazonS3>();
        s3.GetObjectAsync(Bucket, "ref", Arg.Any<CancellationToken>())
            .Returns(new GetObjectResponse { ResponseStream = new MemoryStream(new byte[] { 7, 7, 7 }) });

        var resultado = await new AlmacenS3(s3, Bucket).RecuperarAsync("ref");

        resultado.Should().Equal(7, 7, 7);
    }

    [Fact] // Recuperar un objeto inexistente (404) devuelve null
    public async Task Recuperar_inexistente_devuelve_null()
    {
        var s3 = Substitute.For<IAmazonS3>();
        s3.GetObjectAsync(Bucket, "falta", Arg.Any<CancellationToken>())
            .Throws(new AmazonS3Exception("no existe") { StatusCode = HttpStatusCode.NotFound });

        (await new AlmacenS3(s3, Bucket).RecuperarAsync("falta")).Should().BeNull();
    }

    [Fact] // Existe consulta los metadatos: presente → true, 404 → false
    public async Task Existe_segun_metadatos()
    {
        var s3 = Substitute.For<IAmazonS3>();
        s3.GetObjectMetadataAsync(Bucket, "hay", Arg.Any<CancellationToken>()).Returns(new GetObjectMetadataResponse());
        s3.GetObjectMetadataAsync(Bucket, "no", Arg.Any<CancellationToken>())
            .Throws(new AmazonS3Exception("404") { StatusCode = HttpStatusCode.NotFound });

        var almacen = new AlmacenS3(s3, Bucket);
        (await almacen.ExisteAsync("hay")).Should().BeTrue();
        (await almacen.ExisteAsync("no")).Should().BeFalse();
    }
}

/// <summary>Subida del binario de una foto al backend de alojamiento (CU-04; ADR-08, BT-20).</summary>
public class SubirContenidoFotoTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();
    private static readonly Guid AreaSur = Guid.NewGuid();
    private static readonly byte[] Contenido = { 1, 2, 3, 4 };

    private sealed record Escenario(Usuario Jefe, Foto Foto, FakeFotoRepository Fotos, FakeAlmacenFotos Almacen,
        FakeObservacionRepository Observaciones, FakeRelevamientoRepository Relevamientos);

    private static Escenario Armar(EstadoRelevamiento estado = EstadoRelevamiento.Recoleccion)
    {
        var jefe = Usuario.Crear("ja", RolJerarquico.JefeArea, AreaNorte).Valor!;
        var rel = Relevamiento.Importar("Obra", 15m, AreaNorte, estado).Valor!;
        var obs = Observacion.Georreferenciada(rel.RelevamientoId, jefe.UsuarioId, new DateTime(2026, 6, 1), Guid.NewGuid());
        var foto = Foto.Crear(obs.ObservacionId, obs.MarcadorId, tieneMetadatos: true, FuenteCoordenada.Metadatos, "pendiente");
        return new Escenario(jefe, foto, new FakeFotoRepository(foto), new FakeAlmacenFotos(),
            new FakeObservacionRepository(obs), new FakeRelevamientoRepository(rel));
    }

    private static SubirContenidoFotoHandler Handler(Escenario e, FakeAuditoria? auditoria = null) =>
        new(new FakeUsuarioRepository(e.Jefe), e.Relevamientos, e.Observaciones, e.Fotos, e.Almacen, auditoria ?? new FakeAuditoria());

    [Fact] // CU-04: subir el contenido persiste el binario y asienta la referencia en la foto
    public async Task Subir_contenido_persiste_y_asienta_referencia()
    {
        var e = Armar();
        var auditoria = new FakeAuditoria();
        var handler = Handler(e, auditoria);

        var r = await handler.ManejarAsync(new SubirContenidoFotoCommand(e.Jefe.UsuarioId, e.Foto.FotoId, "foto.jpg", Contenido));

        r.EsExito.Should().BeTrue();
        e.Foto.ReferenciaArchivo.Should().NotBe("pendiente");
        (await e.Almacen.RecuperarAsync(e.Foto.ReferenciaArchivo)).Should().Equal(Contenido);
        auditoria.Registros.Should().Contain(x => x.StartsWith("SUBIR_CONTENIDO_FOTO:"));
    }

    [Fact] // el contenido es obligatorio
    public async Task Subir_contenido_vacio_rechaza()
    {
        var e = Armar();
        var r = await Handler(e).ManejarAsync(new SubirContenidoFotoCommand(e.Jefe.UsuarioId, e.Foto.FotoId, "f.jpg", Array.Empty<byte>()));
        r.Codigo.Should().Be(CodigosError.ContenidoFotoRequerido);
    }

    [Fact] // foto inexistente
    public async Task Subir_contenido_foto_inexistente_rechaza()
    {
        var e = Armar();
        var r = await Handler(e).ManejarAsync(new SubirContenidoFotoCommand(e.Jefe.UsuarioId, Guid.NewGuid(), "f.jpg", Contenido));
        r.Codigo.Should().Be(CodigosError.FotoInexistente);
    }

    [Fact] // RN-05: sobre un relevamiento cerrado se rechaza
    public async Task Subir_contenido_sobre_cerrado_rechaza()
    {
        var e = Armar(EstadoRelevamiento.Cerrado);
        var r = await Handler(e).ManejarAsync(new SubirContenidoFotoCommand(e.Jefe.UsuarioId, e.Foto.FotoId, "f.jpg", Contenido));
        r.Codigo.Should().Be(CodigosError.RelevamientoSoloLectura);
    }

    [Fact] // RN-01: un jefe de otra área no sube contenido
    public async Task Subir_contenido_otra_area_rechaza()
    {
        var e = Armar();
        var jefeSur = Usuario.Crear("js", RolJerarquico.JefeArea, AreaSur).Valor!;
        var handler = new SubirContenidoFotoHandler(
            new FakeUsuarioRepository(jefeSur), e.Relevamientos, e.Observaciones, e.Fotos, e.Almacen, new FakeAuditoria());

        var r = await handler.ManejarAsync(new SubirContenidoFotoCommand(jefeSur.UsuarioId, e.Foto.FotoId, "f.jpg", Contenido));

        r.Codigo.Should().Be(CodigosError.AccesoNoAutorizado);
    }

    [Fact] // RN-07: si no se puede auditar, no se sube
    public async Task Subir_contenido_sin_auditoria_falla()
    {
        var e = Armar();
        var r = await Handler(e, new FakeAuditoria(exito: false))
            .ManejarAsync(new SubirContenidoFotoCommand(e.Jefe.UsuarioId, e.Foto.FotoId, "f.jpg", Contenido));
        r.Codigo.Should().Be(CodigosError.AccionNoAuditada);
    }
}
