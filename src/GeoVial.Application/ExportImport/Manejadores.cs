using GeoVial.Application.Abstracciones;
using GeoVial.Application.Cqrs;
using GeoVial.Domain;

namespace GeoVial.Application.ExportImport;

/// <summary>
/// US-27 / CU-08 §5.A: arma el manifiesto del relevamiento completo, lo empaqueta en un único archivo ZIP
/// y lo entrega, autorizando por área (RN-01, RN-08) y auditando (RN-07). Opera en cualquier estado.
/// </summary>
public sealed class ExportarRelevamientoHandler : IManejador<ExportarRelevamientoCommand, Resultado<ArchivoExportado>>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IMarcadorRepository _marcadores;
    private readonly IObservacionRepository _observaciones;
    private readonly IFotoRepository _fotos;
    private readonly IComentarioRepository _comentarios;
    private readonly IEtiquetaRepository _etiquetas;
    private readonly IEmpaquetadorRelevamiento _empaquetador;
    private readonly IServicioAuditoria _auditoria;

    public ExportarRelevamientoHandler(
        IUsuarioRepository usuarios, IRelevamientoRepository relevamientos, IMarcadorRepository marcadores,
        IObservacionRepository observaciones, IFotoRepository fotos, IComentarioRepository comentarios,
        IEtiquetaRepository etiquetas, IEmpaquetadorRelevamiento empaquetador, IServicioAuditoria auditoria)
    {
        _usuarios = usuarios;
        _relevamientos = relevamientos;
        _marcadores = marcadores;
        _observaciones = observaciones;
        _fotos = fotos;
        _comentarios = comentarios;
        _etiquetas = etiquetas;
        _empaquetador = empaquetador;
        _auditoria = auditoria;
    }

    public async Task<Resultado<ArchivoExportado>> ManejarAsync(ExportarRelevamientoCommand cmd, CancellationToken ct = default)
    {
        var relevamiento = await _relevamientos.ObtenerPorIdAsync(cmd.RelevamientoId, ct);
        if (relevamiento is null)
        {
            return Resultado<ArchivoExportado>.Fallo(CodigosError.RelevamientoInexistente);
        }

        var usuario = await _usuarios.ObtenerPorIdAsync(cmd.UsuarioId, ct);
        if (usuario is null || !Autorizacion.PuedeAccederArea(usuario, relevamiento.AreaId))
        {
            return Resultado<ArchivoExportado>.Fallo(CodigosError.AccesoNoAutorizado);
        }

        var manifiesto = await ArmarManifiestoAsync(relevamiento, ct);
        var contenido = _empaquetador.Empaquetar(manifiesto);

        if (!await _auditoria.RegistrarAsync(cmd.UsuarioId, "EXPORTAR_RELEVAMIENTO", $"relevamiento={cmd.RelevamientoId}", ct))
        {
            return Resultado<ArchivoExportado>.Fallo(CodigosError.AccionNoAuditada);
        }

        return Resultado<ArchivoExportado>.Exito(new ArchivoExportado($"relevamiento-{cmd.RelevamientoId}.zip", contenido));
    }

    private async Task<ManifiestoRelevamiento> ArmarManifiestoAsync(Relevamiento relevamiento, CancellationToken ct)
    {
        var marcadores = await _marcadores.ListarPorRelevamientoAsync(relevamiento.RelevamientoId, ct);
        var observaciones = await _observaciones.ListarPorRelevamientoAsync(relevamiento.RelevamientoId, ct);

        var manifiestoMarcadores = marcadores
            .Select(m => new ManifiestoMarcador(m.MarcadorId, m.Latitud, m.Longitud, m.EnConflicto))
            .ToList();

        var manifiestoObservaciones = observaciones
            .Select(o => new ManifiestoObservacion(o.ObservacionId, o.MarcadorId, o.AgenteUsuarioId, o.MomentoCaptura, o.SinGeorreferenciar))
            .ToList();

        var manifiestoFotos = new List<ManifiestoFoto>();
        foreach (var observacion in observaciones)
        {
            var foto = await _fotos.ObtenerPorObservacionAsync(observacion.ObservacionId, ct);
            if (foto is null)
            {
                continue;
            }

            var etiquetas = await _etiquetas.ListarNombresDeFotoAsync(foto.FotoId, ct);
            manifiestoFotos.Add(new ManifiestoFoto(
                foto.FotoId, foto.ObservacionId, foto.MarcadorId, foto.TieneMetadatosUbicacion, (int?)foto.Fuente, foto.ReferenciaArchivo, etiquetas));
        }

        var manifiestoComentarios = new List<ManifiestoComentario>();
        foreach (var marcador in marcadores)
        {
            foreach (var comentario in await _comentarios.ListarPorMarcadorAsync(marcador.MarcadorId, ct))
            {
                var etiquetas = await _etiquetas.ListarNombresDeComentarioAsync(comentario.ComentarioId, ct);
                manifiestoComentarios.Add(new ManifiestoComentario(
                    comentario.MarcadorId, comentario.FotoId, comentario.AutorUsuarioId, comentario.Texto, comentario.Momento, etiquetas));
            }
        }

        return new ManifiestoRelevamiento(
            ManifiestoRelevamiento.VersionActual,
            relevamiento.IdentificacionObra,
            (int)relevamiento.Estado,
            relevamiento.RadioAgrupacionMetros,
            relevamiento.AreaId,
            manifiestoMarcadores,
            manifiestoObservaciones,
            manifiestoFotos,
            manifiestoComentarios);
    }
}

/// <summary>
/// US-28 / CU-08 §5.B: desempaqueta y valida el archivo, autoriza por área (RN-01) y reconstruye el
/// relevamiento con identificadores nuevos, auditando (RN-07). Ante un archivo incoherente responde
/// ARCHIVO_EXPORTACION_INVALIDO sin alterar datos existentes.
/// </summary>
public sealed class ImportarRelevamientoHandler : IManejador<ImportarRelevamientoCommand, Resultado<Guid>>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IMarcadorRepository _marcadores;
    private readonly IObservacionRepository _observaciones;
    private readonly IFotoRepository _fotos;
    private readonly IComentarioRepository _comentarios;
    private readonly IEtiquetaRepository _etiquetas;
    private readonly IEmpaquetadorRelevamiento _empaquetador;
    private readonly IServicioAuditoria _auditoria;

    public ImportarRelevamientoHandler(
        IUsuarioRepository usuarios, IRelevamientoRepository relevamientos, IMarcadorRepository marcadores,
        IObservacionRepository observaciones, IFotoRepository fotos, IComentarioRepository comentarios,
        IEtiquetaRepository etiquetas, IEmpaquetadorRelevamiento empaquetador, IServicioAuditoria auditoria)
    {
        _usuarios = usuarios;
        _relevamientos = relevamientos;
        _marcadores = marcadores;
        _observaciones = observaciones;
        _fotos = fotos;
        _comentarios = comentarios;
        _etiquetas = etiquetas;
        _empaquetador = empaquetador;
        _auditoria = auditoria;
    }

    public async Task<Resultado<Guid>> ManejarAsync(ImportarRelevamientoCommand cmd, CancellationToken ct = default)
    {
        var manifiesto = _empaquetador.Desempaquetar(cmd.Archivo);
        if (manifiesto is null || !EsCoherente(manifiesto))
        {
            return Resultado<Guid>.Fallo(CodigosError.ArchivoExportacionInvalido);
        }

        var usuario = await _usuarios.ObtenerPorIdAsync(cmd.UsuarioId, ct);
        if (usuario is null || !Autorizacion.PuedeAccederArea(usuario, manifiesto.AreaId))
        {
            return Resultado<Guid>.Fallo(CodigosError.AccesoNoAutorizado);
        }

        // Audita antes de tocar la base: el asiento de auditoría se persiste solo, sin arrastrar la reconstrucción.
        if (!await _auditoria.RegistrarAsync(cmd.UsuarioId, "IMPORTAR_RELEVAMIENTO", $"obra={manifiesto.IdentificacionObra}", ct))
        {
            return Resultado<Guid>.Fallo(CodigosError.AccionNoAuditada);
        }

        return await ReconstruirAsync(manifiesto, ct);
    }

    private async Task<Resultado<Guid>> ReconstruirAsync(ManifiestoRelevamiento m, CancellationToken ct)
    {
        var creado = Relevamiento.Importar(m.IdentificacionObra, m.RadioAgrupacionMetros, m.AreaId, (EstadoRelevamiento)m.Estado);
        if (!creado.EsExito)
        {
            return Resultado<Guid>.Fallo(CodigosError.ArchivoExportacionInvalido);
        }

        var relevamiento = creado.Valor!;
        await _relevamientos.AgregarAsync(relevamiento, ct);

        var mapaMarcadores = new Dictionary<Guid, Guid>();
        foreach (var mm in m.Marcadores)
        {
            var marcador = Marcador.Crear(relevamiento.RelevamientoId, new Coordenada(mm.Latitud, mm.Longitud));
            if (mm.EnConflicto)
            {
                marcador.MarcarConflicto();
            }

            await _marcadores.AgregarAsync(marcador, ct);
            mapaMarcadores[mm.ClaveLocal] = marcador.MarcadorId;
        }

        var mapaObservaciones = new Dictionary<Guid, Guid>();
        foreach (var mo in m.Observaciones)
        {
            var observacion = mo.SinGeorreferenciar
                ? Observacion.EnBandejaSinGeorreferenciar(relevamiento.RelevamientoId, mo.AgenteUsuarioId, mo.MomentoCaptura)
                : Observacion.Georreferenciada(relevamiento.RelevamientoId, mo.AgenteUsuarioId, mo.MomentoCaptura, mapaMarcadores[mo.MarcadorClaveLocal!.Value]);
            await _observaciones.AgregarAsync(observacion, ct);
            mapaObservaciones[mo.ClaveLocal] = observacion.ObservacionId;
        }

        var mapaFotos = new Dictionary<Guid, Guid>();
        foreach (var mf in m.Fotos)
        {
            Guid? marcadorId = mf.MarcadorClaveLocal is { } cl ? mapaMarcadores[cl] : null;
            var foto = Foto.Crear(mapaObservaciones[mf.ObservacionClaveLocal], marcadorId, mf.TieneMetadatos, (FuenteCoordenada?)mf.Fuente, mf.ReferenciaArchivo);
            await _fotos.AgregarAsync(foto, ct);
            mapaFotos[mf.ClaveLocal] = foto.FotoId;
            await VincularEtiquetasFotoAsync(foto.FotoId, mf.Etiquetas, ct);
        }

        foreach (var mc in m.Comentarios)
        {
            Guid? fotoId = mc.FotoClaveLocal is { } cl ? mapaFotos[cl] : null;
            var creadoComentario = Comentario.Crear(mapaMarcadores[mc.MarcadorClaveLocal], fotoId, mc.AutorUsuarioId, mc.Texto, mc.Momento);
            if (!creadoComentario.EsExito)
            {
                return Resultado<Guid>.Fallo(CodigosError.ArchivoExportacionInvalido);
            }

            await _comentarios.AgregarAsync(creadoComentario.Valor!, ct);
            await VincularEtiquetasComentarioAsync(creadoComentario.Valor!.ComentarioId, mc.Etiquetas, ct);
        }

        await _relevamientos.GuardarCambiosAsync(ct);
        return Resultado<Guid>.Exito(relevamiento.RelevamientoId);
    }

    private async Task VincularEtiquetasFotoAsync(Guid fotoId, IReadOnlyList<string> nombres, CancellationToken ct)
    {
        foreach (var nombre in nombres)
        {
            var etiqueta = await ObtenerOCrearEtiquetaAsync(nombre, ct);
            await _etiquetas.AgregarFotoEtiquetaAsync(new FotoEtiqueta(fotoId, etiqueta.EtiquetaId), ct);
        }
    }

    private async Task VincularEtiquetasComentarioAsync(Guid comentarioId, IReadOnlyList<string> nombres, CancellationToken ct)
    {
        foreach (var nombre in nombres)
        {
            var etiqueta = await ObtenerOCrearEtiquetaAsync(nombre, ct);
            await _etiquetas.AgregarComentarioEtiquetaAsync(new ComentarioEtiqueta(comentarioId, etiqueta.EtiquetaId), ct);
        }
    }

    private async Task<Etiqueta> ObtenerOCrearEtiquetaAsync(string nombre, CancellationToken ct)
    {
        var existente = await _etiquetas.ObtenerPorNombreAsync(nombre.Trim(), ct);
        if (existente is not null)
        {
            return existente;
        }

        var creada = Etiqueta.Crear(nombre).Valor!;
        await _etiquetas.AgregarAsync(creada, ct);
        return creada;
    }

    /// <summary>Valida la integridad referencial interna del manifiesto antes de tocar la base (CU-08, US-28 CA-02).</summary>
    private static bool EsCoherente(ManifiestoRelevamiento m)
    {
        if (m.Version != ManifiestoRelevamiento.VersionActual
            || string.IsNullOrWhiteSpace(m.IdentificacionObra)
            || m.RadioAgrupacionMetros <= 0
            || !Enum.IsDefined(typeof(EstadoRelevamiento), m.Estado))
        {
            return false;
        }

        var marcadores = m.Marcadores.Select(x => x.ClaveLocal).ToHashSet();
        var observaciones = m.Observaciones.Select(x => x.ClaveLocal).ToHashSet();
        var fotos = m.Fotos.Select(x => x.ClaveLocal).ToHashSet();

        foreach (var o in m.Observaciones)
        {
            if (o.SinGeorreferenciar)
            {
                if (o.MarcadorClaveLocal is not null)
                {
                    return false;
                }
            }
            else if (o.MarcadorClaveLocal is not { } mid || !marcadores.Contains(mid))
            {
                return false;
            }
        }

        foreach (var f in m.Fotos)
        {
            if (!observaciones.Contains(f.ObservacionClaveLocal))
            {
                return false;
            }

            if (f.MarcadorClaveLocal is { } mid && !marcadores.Contains(mid))
            {
                return false;
            }
        }

        foreach (var c in m.Comentarios)
        {
            if (!marcadores.Contains(c.MarcadorClaveLocal))
            {
                return false;
            }

            if (c.FotoClaveLocal is { } fid && !fotos.Contains(fid))
            {
                return false;
            }
        }

        return true;
    }
}
