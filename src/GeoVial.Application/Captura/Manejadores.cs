using GeoVial.Application.Abstracciones;
using GeoVial.Application.Cqrs;
using GeoVial.Domain;
using GeoVial.FileHosting;

namespace GeoVial.Application.Captura;

/// <summary>Resolución del marcador por radio (RN-02): reusa uno existente dentro del radio o crea uno nuevo.</summary>
internal static class ResolucionMarcador
{
    public static async Task<Marcador> ResolverAsync(
        IMarcadorRepository marcadores, Relevamiento relevamiento, Coordenada coordenada, CancellationToken ct)
    {
        var existentes = await marcadores.ListarPorRelevamientoAsync(relevamiento.RelevamientoId, ct);
        var enRadio = AgrupacionMarcador.MarcadorEnRadio(existentes, coordenada, relevamiento.RadioAgrupacionMetros);
        if (enRadio is not null)
        {
            return enRadio;
        }

        var nuevo = Marcador.Crear(relevamiento.RelevamientoId, coordenada);
        await marcadores.AgregarAsync(nuevo, ct);
        return nuevo;
    }
}

public sealed class CapturarObservacionHandler : IManejador<CapturarObservacionCommand, Resultado<ResultadoCaptura>>
{
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IUsuarioRepository _usuarios;
    private readonly IMarcadorRepository _marcadores;
    private readonly IObservacionRepository _observaciones;
    private readonly IFotoRepository _fotos;
    private readonly IServicioAuditoria _auditoria;
    private readonly IRelojUtc _reloj;

    public CapturarObservacionHandler(
        IRelevamientoRepository relevamientos,
        IUsuarioRepository usuarios,
        IMarcadorRepository marcadores,
        IObservacionRepository observaciones,
        IFotoRepository fotos,
        IServicioAuditoria auditoria,
        IRelojUtc reloj)
    {
        _relevamientos = relevamientos;
        _usuarios = usuarios;
        _marcadores = marcadores;
        _observaciones = observaciones;
        _fotos = fotos;
        _auditoria = auditoria;
        _reloj = reloj;
    }

    public async Task<Resultado<ResultadoCaptura>> ManejarAsync(CapturarObservacionCommand cmd, CancellationToken ct = default)
    {
        var relevamiento = await _relevamientos.ObtenerPorIdAsync(cmd.RelevamientoId, ct);
        if (relevamiento is null)
        {
            return Resultado<ResultadoCaptura>.Fallo(CodigosError.RelevamientoInexistente);
        }

        if (relevamiento.EsSoloLectura)
        {
            return Resultado<ResultadoCaptura>.Fallo(CodigosError.RelevamientoSoloLectura);
        }

        var agente = await _usuarios.ObtenerPorIdAsync(cmd.AgenteId, ct);
        if (agente is null)
        {
            return Resultado<ResultadoCaptura>.Fallo(CodigosError.UsuarioInexistente);
        }

        // CU-04 / RN-01: el agente debe pertenecer al área del relevamiento y estar asignado.
        if (agente.AreaId != relevamiento.AreaId || !relevamiento.AgentesVigentes().Contains(agente.UsuarioId))
        {
            await _auditoria.RegistrarAsync(cmd.AgenteId, "CAPTURA_RECHAZADA", $"relevamiento={cmd.RelevamientoId}", ct);
            return Resultado<ResultadoCaptura>.Fallo(CodigosError.AccesoNoAutorizado);
        }

        var momento = _reloj.AhoraUtc;
        Observacion observacion;
        Foto foto;

        if (cmd.LatitudExif is decimal lat && cmd.LongitudExif is decimal lon)
        {
            // RN-03: los metadatos de la foto son la fuente primaria. RN-02: agrupación por radio.
            var marcador = await ResolucionMarcador.ResolverAsync(_marcadores, relevamiento, new Coordenada(lat, lon), ct);
            observacion = Observacion.Georreferenciada(relevamiento.RelevamientoId, agente.UsuarioId, momento, marcador.MarcadorId);
            foto = Foto.Crear(observacion.ObservacionId, marcador.MarcadorId, tieneMetadatos: true, FuenteCoordenada.Metadatos, cmd.ReferenciaArchivo);
        }
        else
        {
            // RN-03: sin metadatos, la observación va a la bandeja sin georreferenciar a la espera de ubicación manual.
            observacion = Observacion.EnBandejaSinGeorreferenciar(relevamiento.RelevamientoId, agente.UsuarioId, momento);
            foto = Foto.Crear(observacion.ObservacionId, null, tieneMetadatos: false, fuente: null, cmd.ReferenciaArchivo);
        }

        if (!await _auditoria.RegistrarAsync(cmd.AgenteId, "CAPTURA_OBSERVACION", $"observacion={observacion.ObservacionId}", ct))
        {
            return Resultado<ResultadoCaptura>.Fallo(CodigosError.AccionNoAuditada);
        }

        await _observaciones.AgregarAsync(observacion, ct);
        await _fotos.AgregarAsync(foto, ct);
        await _observaciones.GuardarCambiosAsync(ct);

        return Resultado<ResultadoCaptura>.Exito(
            new ResultadoCaptura(observacion.ObservacionId, observacion.MarcadorId, observacion.SinGeorreferenciar, foto.FotoId));
    }
}

public sealed class UbicarObservacionManualHandler : IManejador<UbicarObservacionManualCommand, Resultado>
{
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IUsuarioRepository _usuarios;
    private readonly IMarcadorRepository _marcadores;
    private readonly IObservacionRepository _observaciones;
    private readonly IFotoRepository _fotos;
    private readonly IServicioAuditoria _auditoria;

    public UbicarObservacionManualHandler(
        IRelevamientoRepository relevamientos,
        IUsuarioRepository usuarios,
        IMarcadorRepository marcadores,
        IObservacionRepository observaciones,
        IFotoRepository fotos,
        IServicioAuditoria auditoria)
    {
        _relevamientos = relevamientos;
        _usuarios = usuarios;
        _marcadores = marcadores;
        _observaciones = observaciones;
        _fotos = fotos;
        _auditoria = auditoria;
    }

    public async Task<Resultado> ManejarAsync(UbicarObservacionManualCommand cmd, CancellationToken ct = default)
    {
        var observacion = await _observaciones.ObtenerPorIdAsync(cmd.ObservacionId, ct);
        if (observacion is null)
        {
            return Resultado.Fallo(CodigosError.ObservacionInexistente);
        }

        var relevamiento = await _relevamientos.ObtenerPorIdAsync(observacion.RelevamientoId, ct);
        if (relevamiento is null)
        {
            return Resultado.Fallo(CodigosError.RelevamientoInexistente);
        }

        if (relevamiento.EsSoloLectura)
        {
            return Resultado.Fallo(CodigosError.RelevamientoSoloLectura);
        }

        var usuario = await _usuarios.ObtenerPorIdAsync(cmd.UsuarioId, ct);
        if (usuario is null || !Autorizacion.PuedeAccederArea(usuario, relevamiento.AreaId))
        {
            return Resultado.Fallo(CodigosError.AccesoNoAutorizado);
        }

        var foto = await _fotos.ObtenerPorObservacionAsync(observacion.ObservacionId, ct);
        // RN-03: no se ubica manualmente una foto que sí trae metadatos de ubicación.
        if (foto is not null && foto.TieneMetadatosUbicacion)
        {
            return Resultado.Fallo(CodigosError.FuenteUbicacionIncorrecta);
        }

        var marcador = await ResolucionMarcador.ResolverAsync(_marcadores, relevamiento, new Coordenada(cmd.Latitud, cmd.Longitud), ct);
        observacion.AsociarMarcador(marcador.MarcadorId);
        foto?.Georreferenciar(FuenteCoordenada.Manual, marcador.MarcadorId);

        if (!await _auditoria.RegistrarAsync(cmd.UsuarioId, "UBICAR_MANUAL", $"observacion={observacion.ObservacionId}", ct))
        {
            return Resultado.Fallo(CodigosError.AccionNoAuditada);
        }

        await _observaciones.GuardarCambiosAsync(ct);
        return Resultado.Exito();
    }
}

public sealed class SubirContenidoFotoHandler : IManejador<SubirContenidoFotoCommand, Resultado>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IObservacionRepository _observaciones;
    private readonly IFotoRepository _fotos;
    private readonly IAlmacenFotos _almacen;
    private readonly IPipelineImagen _pipeline;
    private readonly IServicioAuditoria _auditoria;

    public SubirContenidoFotoHandler(
        IUsuarioRepository usuarios, IRelevamientoRepository relevamientos, IObservacionRepository observaciones,
        IFotoRepository fotos, IAlmacenFotos almacen, IPipelineImagen pipeline, IServicioAuditoria auditoria)
    {
        _usuarios = usuarios;
        _relevamientos = relevamientos;
        _observaciones = observaciones;
        _fotos = fotos;
        _almacen = almacen;
        _pipeline = pipeline;
        _auditoria = auditoria;
    }

    public async Task<Resultado> ManejarAsync(SubirContenidoFotoCommand cmd, CancellationToken ct = default)
    {
        if (cmd.Contenido is null || cmd.Contenido.Length == 0)
        {
            return Resultado.Fallo(CodigosError.ContenidoFotoRequerido);
        }

        var foto = await _fotos.ObtenerParaEdicionAsync(cmd.FotoId, ct);
        if (foto is null)
        {
            return Resultado.Fallo(CodigosError.FotoInexistente);
        }

        var observacion = await _observaciones.ObtenerPorIdAsync(foto.ObservacionId, ct);
        if (observacion is null)
        {
            return Resultado.Fallo(CodigosError.ObservacionInexistente);
        }

        var relevamiento = await _relevamientos.ObtenerPorIdAsync(observacion.RelevamientoId, ct);
        if (relevamiento is null)
        {
            return Resultado.Fallo(CodigosError.RelevamientoInexistente);
        }

        if (relevamiento.EsSoloLectura)
        {
            return Resultado.Fallo(CodigosError.RelevamientoSoloLectura);
        }

        var usuario = await _usuarios.ObtenerPorIdAsync(cmd.UsuarioId, ct);
        if (usuario is null || !Autorizacion.PuedeAccederArea(usuario, relevamiento.AreaId))
        {
            return Resultado.Fallo(CodigosError.AccesoNoAutorizado);
        }

        // BT-19: se comprime/redimensiona el binario antes de alojarlo para acotar el payload.
        var procesado = await _pipeline.ProcesarAsync(cmd.Contenido, ct);
        var referencia = await _almacen.GuardarAsync(cmd.NombreArchivo, procesado, ct);
        foto.AsignarReferencia(referencia);

        if (!await _auditoria.RegistrarAsync(cmd.UsuarioId, "SUBIR_CONTENIDO_FOTO", $"foto={cmd.FotoId}", ct))
        {
            return Resultado.Fallo(CodigosError.AccionNoAuditada);
        }

        await _fotos.GuardarCambiosAsync(ct);
        return Resultado.Exito();
    }
}

public sealed class DescargarContenidoFotoHandler : IManejador<DescargarContenidoFotoQuery, byte[]?>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IObservacionRepository _observaciones;
    private readonly IFotoRepository _fotos;
    private readonly IAlmacenFotos _almacen;

    public DescargarContenidoFotoHandler(
        IUsuarioRepository usuarios, IRelevamientoRepository relevamientos, IObservacionRepository observaciones,
        IFotoRepository fotos, IAlmacenFotos almacen)
    {
        _usuarios = usuarios;
        _relevamientos = relevamientos;
        _observaciones = observaciones;
        _fotos = fotos;
        _almacen = almacen;
    }

    public async Task<byte[]?> ManejarAsync(DescargarContenidoFotoQuery query, CancellationToken ct = default)
    {
        var foto = await _fotos.ObtenerPorIdAsync(query.FotoId, ct);
        if (foto is null || string.IsNullOrEmpty(foto.ReferenciaArchivo))
        {
            return null;
        }

        var observacion = await _observaciones.ObtenerPorIdAsync(foto.ObservacionId, ct);
        if (observacion is null)
        {
            return null;
        }

        var relevamiento = await _relevamientos.ObtenerPorIdAsync(observacion.RelevamientoId, ct);
        var usuario = await _usuarios.ObtenerPorIdAsync(query.SolicitanteId, ct);
        if (relevamiento is null || usuario is null || !Autorizacion.PuedeAccederArea(usuario, relevamiento.AreaId))
        {
            return null;
        }

        return await _almacen.RecuperarAsync(foto.ReferenciaArchivo, ct);
    }
}

public sealed class ListarObservacionesHandler : IManejador<ListarObservacionesQuery, IReadOnlyList<Observacion>>
{
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IUsuarioRepository _usuarios;
    private readonly IObservacionRepository _observaciones;

    public ListarObservacionesHandler(IRelevamientoRepository relevamientos, IUsuarioRepository usuarios, IObservacionRepository observaciones)
    {
        _relevamientos = relevamientos;
        _usuarios = usuarios;
        _observaciones = observaciones;
    }

    public async Task<IReadOnlyList<Observacion>> ManejarAsync(ListarObservacionesQuery query, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(query.SolicitanteId, ct);
        var relevamiento = await _relevamientos.ObtenerPorIdAsync(query.RelevamientoId, ct);
        if (usuario is null || relevamiento is null || !Autorizacion.PuedeAccederArea(usuario, relevamiento.AreaId))
        {
            return Array.Empty<Observacion>();
        }

        return await _observaciones.ListarPorRelevamientoAsync(query.RelevamientoId, ct);
    }
}
