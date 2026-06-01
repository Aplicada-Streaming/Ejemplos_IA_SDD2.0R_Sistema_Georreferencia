using FluentAssertions;
using GeoVial.Application.ExportImport;
using GeoVial.Domain;
using GeoVial.Infrastructure.ExportImport;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Empaquetado físico del manifiesto en ZIP (CU-08 §4.1; EmpaquetadorZip).</summary>
public class EmpaquetadorZipTests
{
    private static ManifiestoRelevamiento Manifiesto() => new(
        ManifiestoRelevamiento.VersionActual,
        "Puente Río 12",
        (int)EstadoRelevamiento.Revision,
        15m,
        Guid.NewGuid(),
        new[] { new ManifiestoMarcador(Guid.NewGuid(), -34.6m, -58.4m, false) },
        new[] { new ManifiestoObservacion(Guid.NewGuid(), null, Guid.NewGuid(), new DateTime(2026, 6, 1), true) },
        Array.Empty<ManifiestoFoto>(),
        Array.Empty<ManifiestoComentario>());

    [Fact] // round-trip: empaquetar y desempaquetar devuelve el mismo manifiesto
    public void Empaquetar_y_desempaquetar_round_trip()
    {
        var emp = new EmpaquetadorZip();
        var original = Manifiesto();

        var bytes = emp.Empaquetar(original);
        var leido = emp.Desempaquetar(bytes);

        leido.Should().BeEquivalentTo(original);
    }

    [Fact] // un archivo que no es ZIP devuelve null (lo trata el handler como inválido)
    public void Desempaquetar_basura_devuelve_null() =>
        new EmpaquetadorZip().Desempaquetar(new byte[] { 1, 2, 3, 4, 5 }).Should().BeNull();
}

/// <summary>Reconstrucción del relevamiento al importar (CU-08 §5.B; Relevamiento.Importar).</summary>
public class RelevamientoImportarTests
{
    private static readonly Guid Area = Guid.NewGuid();

    [Fact] // la importación preserva el estado del relevamiento
    public void Importar_preserva_estado()
    {
        var r = Relevamiento.Importar("Obra X", 20m, Area, EstadoRelevamiento.Cerrado).Valor!;
        r.Estado.Should().Be(EstadoRelevamiento.Cerrado);
        r.RadioAgrupacionMetros.Should().Be(20m);
        r.EsSoloLectura.Should().BeTrue();
    }

    [Fact] // RN-02: un radio inválido en el archivo se rechaza
    public void Importar_con_radio_invalido_falla() =>
        Relevamiento.Importar("Obra X", 0m, Area, EstadoRelevamiento.Recoleccion).Codigo.Should().Be(CodigosError.RadioInvalido);
}

/// <summary>Exportación e importación del relevamiento completo (CU-08 §5.A/§5.B; US-27/US-28).</summary>
public class ExportImportAplicacionTests
{
    private static readonly Guid AreaNorte = Guid.NewGuid();
    private static readonly Guid AreaSur = Guid.NewGuid();

    private static Usuario Jefe(Guid area) => Usuario.Crear("ja", RolJerarquico.JefeArea, area).Valor!;

    private sealed record Escenario(
        Usuario Jefe,
        Relevamiento Rel,
        FakeRelevamientoRepository Relevamientos,
        FakeMarcadorRepository Marcadores,
        FakeObservacionRepository Observaciones,
        FakeFotoRepository Fotos,
        FakeComentarioRepository Comentarios,
        FakeEtiquetaRepository Etiquetas,
        Guid MarcadorId);

    private static async Task<Escenario> ArmarEscenarioAsync(EstadoRelevamiento estado = EstadoRelevamiento.Revision)
    {
        var jefe = Jefe(AreaNorte);
        var rel = Relevamiento.Importar("Puente Río 12", 15m, AreaNorte, estado).Valor!;
        var marcador = Marcador.Crear(rel.RelevamientoId, new Coordenada(-34.6m, -58.4m));
        var obs = Observacion.Georreferenciada(rel.RelevamientoId, jefe.UsuarioId, new DateTime(2026, 6, 1), marcador.MarcadorId);
        var foto = Foto.Crear(obs.ObservacionId, marcador.MarcadorId, tieneMetadatos: true, FuenteCoordenada.Metadatos, "f.jpg");
        var com = Comentario.Crear(marcador.MarcadorId, foto.FotoId, jefe.UsuarioId, "fisura en viga", new DateTime(2026, 6, 1)).Valor!;

        var etiquetas = new FakeEtiquetaRepository();
        var etiqueta = Etiqueta.Crear("fisura").Valor!;
        await etiquetas.AgregarAsync(etiqueta);
        await etiquetas.AgregarFotoEtiquetaAsync(new FotoEtiqueta(foto.FotoId, etiqueta.EtiquetaId));
        await etiquetas.AgregarComentarioEtiquetaAsync(new ComentarioEtiqueta(com.ComentarioId, etiqueta.EtiquetaId));

        return new Escenario(jefe, rel, new FakeRelevamientoRepository(rel), new FakeMarcadorRepository(marcador),
            new FakeObservacionRepository(obs), new FakeFotoRepository(foto), new FakeComentarioRepository(com), etiquetas, marcador.MarcadorId);
    }

    private static ExportarRelevamientoHandler ExportHandler(Escenario e, FakeAuditoria? auditoria = null) =>
        new(new FakeUsuarioRepository(e.Jefe), e.Relevamientos, e.Marcadores, e.Observaciones, e.Fotos, e.Comentarios,
            e.Etiquetas, new EmpaquetadorZip(), auditoria ?? new FakeAuditoria());

    [Fact] // US-27 CA-01: exportar entrega un archivo y audita
    public async Task Exportar_entrega_archivo_y_audita()
    {
        var e = await ArmarEscenarioAsync();
        var auditoria = new FakeAuditoria();
        var handler = ExportHandler(e, auditoria);

        var r = await handler.ManejarAsync(new ExportarRelevamientoCommand(e.Jefe.UsuarioId, e.Rel.RelevamientoId));

        r.EsExito.Should().BeTrue();
        r.Valor!.Contenido.Should().NotBeEmpty();
        r.Valor.NombreArchivo.Should().EndWith(".zip");
        auditoria.Registros.Should().Contain(x => x.StartsWith("EXPORTAR_RELEVAMIENTO:"));
    }

    [Fact] // RN-01: un jefe de otra área no exporta
    public async Task Exportar_otra_area_rechaza()
    {
        var e = await ArmarEscenarioAsync();
        var jefeSur = Jefe(AreaSur);
        var handler = new ExportarRelevamientoHandler(
            new FakeUsuarioRepository(jefeSur), e.Relevamientos, e.Marcadores, e.Observaciones, e.Fotos, e.Comentarios,
            e.Etiquetas, new EmpaquetadorZip(), new FakeAuditoria());

        var r = await handler.ManejarAsync(new ExportarRelevamientoCommand(jefeSur.UsuarioId, e.Rel.RelevamientoId));

        r.Codigo.Should().Be(CodigosError.AccesoNoAutorizado);
    }

    [Fact] // relevamiento inexistente
    public async Task Exportar_inexistente_rechaza()
    {
        var e = await ArmarEscenarioAsync();
        var handler = new ExportarRelevamientoHandler(
            new FakeUsuarioRepository(e.Jefe), new FakeRelevamientoRepository(), e.Marcadores, e.Observaciones, e.Fotos,
            e.Comentarios, e.Etiquetas, new EmpaquetadorZip(), new FakeAuditoria());

        var r = await handler.ManejarAsync(new ExportarRelevamientoCommand(e.Jefe.UsuarioId, Guid.NewGuid()));

        r.Codigo.Should().Be(CodigosError.RelevamientoInexistente);
    }

    [Fact] // RN-07: si no se puede auditar, no entrega el archivo
    public async Task Exportar_sin_auditoria_falla()
    {
        var e = await ArmarEscenarioAsync();
        var handler = ExportHandler(e, new FakeAuditoria(exito: false));

        var r = await handler.ManejarAsync(new ExportarRelevamientoCommand(e.Jefe.UsuarioId, e.Rel.RelevamientoId));

        r.Codigo.Should().Be(CodigosError.AccionNoAuditada);
    }

    [Fact] // US-27 + US-28 CA-01: el ciclo exportar → importar reconstruye el relevamiento completo
    public async Task Exportar_luego_importar_reconstruye_round_trip()
    {
        var origen = await ArmarEscenarioAsync(EstadoRelevamiento.Revision);
        var exportador = ExportHandler(origen);
        var exportado = await exportador.ManejarAsync(new ExportarRelevamientoCommand(origen.Jefe.UsuarioId, origen.Rel.RelevamientoId));
        exportado.EsExito.Should().BeTrue();

        // Instancia destino: repositorios vacíos, mismo jefe de área.
        var relevamientos = new FakeRelevamientoRepository();
        var marcadores = new FakeMarcadorRepository();
        var observaciones = new FakeObservacionRepository();
        var fotos = new FakeFotoRepository();
        var comentarios = new FakeComentarioRepository();
        var etiquetas = new FakeEtiquetaRepository();
        var auditoria = new FakeAuditoria();
        var importador = new ImportarRelevamientoHandler(
            new FakeUsuarioRepository(origen.Jefe), relevamientos, marcadores, observaciones, fotos, comentarios,
            etiquetas, new EmpaquetadorZip(), auditoria);

        var importado = await importador.ManejarAsync(new ImportarRelevamientoCommand(origen.Jefe.UsuarioId, exportado.Valor!.Contenido));

        importado.EsExito.Should().BeTrue();
        var nuevoId = importado.Valor;

        var relImportado = await relevamientos.ObtenerPorIdAsync(nuevoId);
        relImportado.Should().NotBeNull();
        relImportado!.Estado.Should().Be(EstadoRelevamiento.Revision);
        relImportado.AreaId.Should().Be(AreaNorte);

        var marcadoresImportados = await marcadores.ListarPorRelevamientoAsync(nuevoId);
        marcadoresImportados.Should().ContainSingle();
        var nuevoMarcadorId = marcadoresImportados[0].MarcadorId;
        nuevoMarcadorId.Should().NotBe(origen.MarcadorId); // identificador remapeado

        (await observaciones.ListarPorRelevamientoAsync(nuevoId)).Should().ContainSingle();
        var fotosImportadas = await fotos.ListarPorMarcadorAsync(nuevoMarcadorId);
        fotosImportadas.Should().ContainSingle();
        var comentariosImportados = await comentarios.ListarPorMarcadorAsync(nuevoMarcadorId);
        comentariosImportados.Should().ContainSingle().Which.Texto.Should().Be("fisura en viga");

        (await etiquetas.ListarNombresDeFotoAsync(fotosImportadas[0].FotoId)).Should().Contain("fisura");
        auditoria.Registros.Should().Contain(x => x.StartsWith("IMPORTAR_RELEVAMIENTO:"));
    }

    [Fact] // US-28 CA-02: un archivo que no es un relevamiento coherente se rechaza sin tocar datos
    public async Task Importar_archivo_invalido_rechaza()
    {
        var importador = new ImportarRelevamientoHandler(
            new FakeUsuarioRepository(Jefe(AreaNorte)), new FakeRelevamientoRepository(), new FakeMarcadorRepository(),
            new FakeObservacionRepository(), new FakeFotoRepository(), new FakeComentarioRepository(),
            new FakeEtiquetaRepository(), new EmpaquetadorZip(), new FakeAuditoria());

        var r = await importador.ManejarAsync(new ImportarRelevamientoCommand(Jefe(AreaNorte).UsuarioId, new byte[] { 9, 9, 9 }));

        r.Codigo.Should().Be(CodigosError.ArchivoExportacionInvalido);
    }

    [Fact] // US-28: un manifiesto con integridad referencial rota se rechaza
    public async Task Importar_manifiesto_incoherente_rechaza()
    {
        var jefe = Jefe(AreaNorte);
        // Comentario que referencia un marcador inexistente en el manifiesto.
        var manifiesto = new ManifiestoRelevamiento(
            ManifiestoRelevamiento.VersionActual, "Obra", (int)EstadoRelevamiento.Recoleccion, 15m, AreaNorte,
            Array.Empty<ManifiestoMarcador>(),
            Array.Empty<ManifiestoObservacion>(),
            Array.Empty<ManifiestoFoto>(),
            new[] { new ManifiestoComentario(Guid.NewGuid(), null, jefe.UsuarioId, "x", new DateTime(2026, 6, 1), Array.Empty<string>()) });
        var bytes = new EmpaquetadorZip().Empaquetar(manifiesto);

        var importador = new ImportarRelevamientoHandler(
            new FakeUsuarioRepository(jefe), new FakeRelevamientoRepository(), new FakeMarcadorRepository(),
            new FakeObservacionRepository(), new FakeFotoRepository(), new FakeComentarioRepository(),
            new FakeEtiquetaRepository(), new EmpaquetadorZip(), new FakeAuditoria());

        var r = await importador.ManejarAsync(new ImportarRelevamientoCommand(jefe.UsuarioId, bytes));

        r.Codigo.Should().Be(CodigosError.ArchivoExportacionInvalido);
    }

    [Fact] // RN-01: un jefe de otra área no importa
    public async Task Importar_otra_area_rechaza()
    {
        var origen = await ArmarEscenarioAsync();
        var exportado = await ExportHandler(origen).ManejarAsync(new ExportarRelevamientoCommand(origen.Jefe.UsuarioId, origen.Rel.RelevamientoId));
        var jefeSur = Jefe(AreaSur);

        var importador = new ImportarRelevamientoHandler(
            new FakeUsuarioRepository(jefeSur), new FakeRelevamientoRepository(), new FakeMarcadorRepository(),
            new FakeObservacionRepository(), new FakeFotoRepository(), new FakeComentarioRepository(),
            new FakeEtiquetaRepository(), new EmpaquetadorZip(), new FakeAuditoria());

        var r = await importador.ManejarAsync(new ImportarRelevamientoCommand(jefeSur.UsuarioId, exportado.Valor!.Contenido));

        r.Codigo.Should().Be(CodigosError.AccesoNoAutorizado);
    }

    [Fact] // RN-07: si no se puede auditar, no importa
    public async Task Importar_sin_auditoria_falla()
    {
        var origen = await ArmarEscenarioAsync();
        var exportado = await ExportHandler(origen).ManejarAsync(new ExportarRelevamientoCommand(origen.Jefe.UsuarioId, origen.Rel.RelevamientoId));

        var importador = new ImportarRelevamientoHandler(
            new FakeUsuarioRepository(origen.Jefe), new FakeRelevamientoRepository(), new FakeMarcadorRepository(),
            new FakeObservacionRepository(), new FakeFotoRepository(), new FakeComentarioRepository(),
            new FakeEtiquetaRepository(), new EmpaquetadorZip(), new FakeAuditoria(exito: false));

        var r = await importador.ManejarAsync(new ImportarRelevamientoCommand(origen.Jefe.UsuarioId, exportado.Valor!.Contenido));

        r.Codigo.Should().Be(CodigosError.AccionNoAuditada);
    }
}
