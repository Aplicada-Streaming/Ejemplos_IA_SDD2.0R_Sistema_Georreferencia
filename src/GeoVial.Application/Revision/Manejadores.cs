using GeoVial.Application.Abstracciones;
using GeoVial.Application.Cqrs;
using GeoVial.Domain;

namespace GeoVial.Application.Revision;

/// <summary>Carga del relevamiento de un marcador y autorización por rol y área (RN-01).</summary>
internal static class AccesoMarcador
{
    public static async Task<(Relevamiento? Relevamiento, string? Error)> CargarPorMarcadorAsync(
        IUsuarioRepository usuarios, IMarcadorRepository marcadores, IRelevamientoRepository relevamientos,
        Guid usuarioId, Guid marcadorId, CancellationToken ct)
    {
        var marcador = await marcadores.ObtenerPorIdAsync(marcadorId, ct);
        if (marcador is null)
        {
            return (null, CodigosError.MarcadorInexistente);
        }

        var relevamiento = await relevamientos.ObtenerPorIdAsync(marcador.RelevamientoId, ct);
        if (relevamiento is null)
        {
            return (null, CodigosError.RelevamientoInexistente);
        }

        var usuario = await usuarios.ObtenerPorIdAsync(usuarioId, ct);
        if (usuario is null || !Autorizacion.PuedeAccederArea(usuario, relevamiento.AreaId))
        {
            return (null, CodigosError.AccesoNoAutorizado);
        }

        return (relevamiento, null);
    }
}

public sealed class AgregarComentarioHandler : IManejador<AgregarComentarioCommand, Resultado>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IMarcadorRepository _marcadores;
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IComentarioRepository _comentarios;
    private readonly IServicioAuditoria _auditoria;
    private readonly IRelojUtc _reloj;

    public AgregarComentarioHandler(
        IUsuarioRepository usuarios, IMarcadorRepository marcadores, IRelevamientoRepository relevamientos,
        IComentarioRepository comentarios, IServicioAuditoria auditoria, IRelojUtc reloj)
    {
        _usuarios = usuarios;
        _marcadores = marcadores;
        _relevamientos = relevamientos;
        _comentarios = comentarios;
        _auditoria = auditoria;
        _reloj = reloj;
    }

    public async Task<Resultado> ManejarAsync(AgregarComentarioCommand cmd, CancellationToken ct = default)
    {
        var (relevamiento, error) = await AccesoMarcador.CargarPorMarcadorAsync(
            _usuarios, _marcadores, _relevamientos, cmd.UsuarioId, cmd.MarcadorId, ct);
        if (error is not null)
        {
            return Resultado.Fallo(error);
        }

        if (relevamiento!.EsSoloLectura)
        {
            return Resultado.Fallo(CodigosError.RelevamientoSoloLectura);
        }

        var creado = Comentario.Crear(cmd.MarcadorId, cmd.FotoId, cmd.UsuarioId, cmd.Texto, _reloj.AhoraUtc);
        if (!creado.EsExito)
        {
            return Resultado.Fallo(creado.Codigo!);
        }

        if (!await _auditoria.RegistrarAsync(cmd.UsuarioId, "AGREGAR_COMENTARIO", $"marcador={cmd.MarcadorId}", ct))
        {
            return Resultado.Fallo(CodigosError.AccionNoAuditada);
        }

        await _comentarios.AgregarAsync(creado.Valor!, ct);
        await _comentarios.GuardarCambiosAsync(ct);
        return Resultado.Exito();
    }
}

public sealed class EtiquetarFotoHandler : IManejador<EtiquetarFotoCommand, Resultado>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IMarcadorRepository _marcadores;
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IFotoRepository _fotos;
    private readonly IEtiquetaRepository _etiquetas;
    private readonly IServicioAuditoria _auditoria;

    public EtiquetarFotoHandler(
        IUsuarioRepository usuarios, IMarcadorRepository marcadores, IRelevamientoRepository relevamientos,
        IFotoRepository fotos, IEtiquetaRepository etiquetas, IServicioAuditoria auditoria)
    {
        _usuarios = usuarios;
        _marcadores = marcadores;
        _relevamientos = relevamientos;
        _fotos = fotos;
        _etiquetas = etiquetas;
        _auditoria = auditoria;
    }

    public async Task<Resultado> ManejarAsync(EtiquetarFotoCommand cmd, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(cmd.Etiqueta))
        {
            return Resultado.Fallo(CodigosError.EtiquetaRequerida);
        }

        var foto = await _fotos.ObtenerPorIdAsync(cmd.FotoId, ct);
        if (foto is null)
        {
            return Resultado.Fallo(CodigosError.FotoInexistente);
        }

        if (foto.MarcadorId is null)
        {
            return Resultado.Fallo(CodigosError.MarcadorInexistente);
        }

        var (relevamiento, error) = await AccesoMarcador.CargarPorMarcadorAsync(
            _usuarios, _marcadores, _relevamientos, cmd.UsuarioId, foto.MarcadorId.Value, ct);
        if (error is not null)
        {
            return Resultado.Fallo(error);
        }

        if (relevamiento!.EsSoloLectura)
        {
            return Resultado.Fallo(CodigosError.RelevamientoSoloLectura);
        }

        var etiqueta = await ObtenerOCrearAsync(_etiquetas, cmd.Etiqueta, ct);
        if (!await _etiquetas.ExisteFotoEtiquetaAsync(foto.FotoId, etiqueta.EtiquetaId, ct))
        {
            await _etiquetas.AgregarFotoEtiquetaAsync(new FotoEtiqueta(foto.FotoId, etiqueta.EtiquetaId), ct);
        }

        if (!await _auditoria.RegistrarAsync(cmd.UsuarioId, "ETIQUETAR_FOTO", $"foto={cmd.FotoId}", ct))
        {
            return Resultado.Fallo(CodigosError.AccionNoAuditada);
        }

        await _etiquetas.GuardarCambiosAsync(ct);
        return Resultado.Exito();
    }

    internal static async Task<Etiqueta> ObtenerOCrearAsync(IEtiquetaRepository etiquetas, string nombre, CancellationToken ct)
    {
        var existente = await etiquetas.ObtenerPorNombreAsync(nombre.Trim(), ct);
        if (existente is not null)
        {
            return existente;
        }

        var creada = Etiqueta.Crear(nombre).Valor!;
        await etiquetas.AgregarAsync(creada, ct);
        return creada;
    }
}

public sealed class EtiquetarComentarioHandler : IManejador<EtiquetarComentarioCommand, Resultado>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IMarcadorRepository _marcadores;
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IComentarioRepository _comentarios;
    private readonly IEtiquetaRepository _etiquetas;
    private readonly IServicioAuditoria _auditoria;

    public EtiquetarComentarioHandler(
        IUsuarioRepository usuarios, IMarcadorRepository marcadores, IRelevamientoRepository relevamientos,
        IComentarioRepository comentarios, IEtiquetaRepository etiquetas, IServicioAuditoria auditoria)
    {
        _usuarios = usuarios;
        _marcadores = marcadores;
        _relevamientos = relevamientos;
        _comentarios = comentarios;
        _etiquetas = etiquetas;
        _auditoria = auditoria;
    }

    public async Task<Resultado> ManejarAsync(EtiquetarComentarioCommand cmd, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(cmd.Etiqueta))
        {
            return Resultado.Fallo(CodigosError.EtiquetaRequerida);
        }

        var comentario = await _comentarios.ObtenerPorIdAsync(cmd.ComentarioId, ct);
        if (comentario is null)
        {
            return Resultado.Fallo(CodigosError.ComentarioInexistente);
        }

        var (relevamiento, error) = await AccesoMarcador.CargarPorMarcadorAsync(
            _usuarios, _marcadores, _relevamientos, cmd.UsuarioId, comentario.MarcadorId, ct);
        if (error is not null)
        {
            return Resultado.Fallo(error);
        }

        if (relevamiento!.EsSoloLectura)
        {
            return Resultado.Fallo(CodigosError.RelevamientoSoloLectura);
        }

        var etiqueta = await EtiquetarFotoHandler.ObtenerOCrearAsync(_etiquetas, cmd.Etiqueta, ct);
        if (!await _etiquetas.ExisteComentarioEtiquetaAsync(comentario.ComentarioId, etiqueta.EtiquetaId, ct))
        {
            await _etiquetas.AgregarComentarioEtiquetaAsync(new ComentarioEtiqueta(comentario.ComentarioId, etiqueta.EtiquetaId), ct);
        }

        if (!await _auditoria.RegistrarAsync(cmd.UsuarioId, "ETIQUETAR_COMENTARIO", $"comentario={cmd.ComentarioId}", ct))
        {
            return Resultado.Fallo(CodigosError.AccionNoAuditada);
        }

        await _etiquetas.GuardarCambiosAsync(ct);
        return Resultado.Exito();
    }
}

public sealed class RevisarRelevamientoHandler : IManejador<RevisarRelevamientoQuery, RevisionRelevamiento?>
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IRelevamientoRepository _relevamientos;
    private readonly IMarcadorRepository _marcadores;
    private readonly IObservacionRepository _observaciones;
    private readonly IFotoRepository _fotos;
    private readonly IComentarioRepository _comentarios;
    private readonly IEtiquetaRepository _etiquetas;

    public RevisarRelevamientoHandler(
        IUsuarioRepository usuarios, IRelevamientoRepository relevamientos, IMarcadorRepository marcadores,
        IObservacionRepository observaciones, IFotoRepository fotos, IComentarioRepository comentarios, IEtiquetaRepository etiquetas)
    {
        _usuarios = usuarios;
        _relevamientos = relevamientos;
        _marcadores = marcadores;
        _observaciones = observaciones;
        _fotos = fotos;
        _comentarios = comentarios;
        _etiquetas = etiquetas;
    }

    public async Task<RevisionRelevamiento?> ManejarAsync(RevisarRelevamientoQuery query, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(query.SolicitanteId, ct);
        var relevamiento = await _relevamientos.ObtenerPorIdAsync(query.RelevamientoId, ct);
        if (usuario is null || relevamiento is null || !Autorizacion.PuedeAccederArea(usuario, relevamiento.AreaId))
        {
            return null;
        }

        var marcadores = await _marcadores.ListarPorRelevamientoAsync(query.RelevamientoId, ct);
        var revisionMarcadores = new List<RevisionMarcador>();
        foreach (var marcador in marcadores)
        {
            var fotos = await _fotos.ListarPorMarcadorAsync(marcador.MarcadorId, ct);
            var revisionFotos = new List<RevisionFoto>();
            foreach (var foto in fotos)
            {
                revisionFotos.Add(new RevisionFoto(foto.FotoId, foto.ReferenciaArchivo, await _etiquetas.ListarNombresDeFotoAsync(foto.FotoId, ct)));
            }

            var comentarios = await _comentarios.ListarPorMarcadorAsync(marcador.MarcadorId, ct);
            var revisionComentarios = new List<RevisionComentario>();
            foreach (var comentario in comentarios)
            {
                revisionComentarios.Add(new RevisionComentario(comentario.ComentarioId, comentario.Texto, comentario.FotoId,
                    await _etiquetas.ListarNombresDeComentarioAsync(comentario.ComentarioId, ct)));
            }

            revisionMarcadores.Add(new RevisionMarcador(
                marcador.MarcadorId, marcador.Latitud, marcador.Longitud, marcador.EnConflicto, revisionFotos, revisionComentarios));
        }

        var observaciones = await _observaciones.ListarPorRelevamientoAsync(query.RelevamientoId, ct);
        var sinGeorreferenciar = observaciones.Where(o => o.SinGeorreferenciar).Select(o => o.ObservacionId).ToList();

        if (query.Etiquetas.Count > 0)
        {
            // US-23 / CU-08 §5.C: filtra fotos y comentarios por etiqueta y descarta los marcadores sin coincidencias.
            revisionMarcadores = FiltrarPorEtiquetas(revisionMarcadores, query.Etiquetas);
            sinGeorreferenciar = new List<Guid>(); // las observaciones sin georreferenciar no tienen etiquetas
        }

        return new RevisionRelevamiento((relevamiento.RelevamientoId), (int)relevamiento.Estado, revisionMarcadores, sinGeorreferenciar);
    }

    private static List<RevisionMarcador> FiltrarPorEtiquetas(List<RevisionMarcador> marcadores, IReadOnlyList<string> etiquetas)
    {
        var buscadas = new HashSet<string>(etiquetas, StringComparer.OrdinalIgnoreCase);
        bool Coincide(IReadOnlyList<string> propias) => propias.Any(buscadas.Contains);

        var filtrados = new List<RevisionMarcador>();
        foreach (var m in marcadores)
        {
            var fotos = m.Fotos.Where(f => Coincide(f.Etiquetas)).ToList();
            var comentarios = m.Comentarios.Where(c => Coincide(c.Etiquetas)).ToList();
            if (fotos.Count == 0 && comentarios.Count == 0)
            {
                continue;
            }

            filtrados.Add(m with { Fotos = fotos, Comentarios = comentarios });
        }

        return filtrados;
    }
}
