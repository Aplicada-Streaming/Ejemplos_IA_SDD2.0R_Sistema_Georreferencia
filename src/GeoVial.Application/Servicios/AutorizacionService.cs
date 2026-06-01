using GeoVial.Application.Abstracciones;
using GeoVial.Domain;

namespace GeoVial.Application.Servicios;

/// <summary>
/// Servicio transversal de autorización por rol y área (CU-14, US-31). Bloquea y registra todo
/// acceso fuera de alcance (RN-01, RN-08) de forma uniforme, sin filtrar información del recurso.
/// </summary>
public sealed class AutorizacionService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IServicioAuditoria _auditoria;

    public AutorizacionService(IUsuarioRepository usuarios, IServicioAuditoria auditoria)
    {
        _usuarios = usuarios;
        _auditoria = auditoria;
    }

    /// <summary>Autoriza el acceso a un recurso de un área (CU-14 CA-01/CA-03).</summary>
    public async Task<Resultado> EvaluarAccesoAreaAsync(Guid usuarioId, Guid? areaRecurso, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(usuarioId, ct);
        if (usuario is not null && Autorizacion.PuedeAccederArea(usuario, areaRecurso))
        {
            return Resultado.Exito();
        }

        await _auditoria.RegistrarAsync(usuarioId, "ACCESO_RECHAZADO", $"area={areaRecurso}", ct);
        return Resultado.Fallo(CodigosError.AccesoNoAutorizado);
    }

    /// <summary>Autoriza el acceso a datos personales de otro usuario (CU-14 CA-02, RN-08).</summary>
    public async Task<Resultado> EvaluarAccesoDatoPersonalAsync(Guid solicitanteId, Guid objetivoId, CancellationToken ct = default)
    {
        var solicitante = await _usuarios.ObtenerPorIdAsync(solicitanteId, ct);
        var objetivo = await _usuarios.ObtenerPorIdAsync(objetivoId, ct);
        if (solicitante is not null && objetivo is not null && Autorizacion.PuedeAccederDatoPersonal(solicitante, objetivo))
        {
            return Resultado.Exito();
        }

        await _auditoria.RegistrarAsync(solicitanteId, "ACCESO_DATO_PERSONAL_RECHAZADO", $"objetivo={objetivoId}", ct);
        return Resultado.Fallo(CodigosError.AccesoDatoPersonalNoAutorizado);
    }
}
