using GeoVial.Application.Abstracciones;
using GeoVial.Domain;

namespace GeoVial.Application.Servicios;

/// <summary>
/// Consulta del historial de auditoría con retención (US-30, CU-13). Restringida al rol raíz (RN-01);
/// por defecto acota al período de retención mínimo de doce meses (RN-07). El registro es inmutable.
/// </summary>
public sealed class ConsultaAuditoriaService
{
    private const int MesesRetencion = 12;

    private readonly IUsuarioRepository _usuarios;
    private readonly IConsultaAuditoria _consulta;
    private readonly IServicioAuditoria _auditoria;
    private readonly IRelojUtc _reloj;

    public ConsultaAuditoriaService(
        IUsuarioRepository usuarios, IConsultaAuditoria consulta, IServicioAuditoria auditoria, IRelojUtc reloj)
    {
        _usuarios = usuarios;
        _consulta = consulta;
        _auditoria = auditoria;
        _reloj = reloj;
    }

    public async Task<Resultado<IReadOnlyList<RegistroAuditoria>>> ConsultarAsync(
        Guid solicitanteId, Guid? autorUsuarioId, string? recurso, DateTime? desde, DateTime? hasta, CancellationToken ct = default)
    {
        var solicitante = await _usuarios.ObtenerPorIdAsync(solicitanteId, ct);
        if (solicitante is null || solicitante.Rol != RolJerarquico.Raiz)
        {
            // CU-13 CA-02 (lado consulta): el intento fuera de alcance queda registrado.
            await _auditoria.RegistrarAsync(solicitanteId, "CONSULTA_AUDITORIA_RECHAZADA", "auditoria", ct);
            return Resultado<IReadOnlyList<RegistroAuditoria>>.Fallo(CodigosError.AccesoNoAutorizado);
        }

        var ahora = _reloj.AhoraUtc;
        var hastaEfectivo = hasta ?? ahora;
        var desdeEfectivo = desde ?? ahora.AddMonths(-MesesRetencion);

        var registros = await _consulta.ConsultarAsync(autorUsuarioId, recurso, desdeEfectivo, hastaEfectivo, ct);
        return Resultado<IReadOnlyList<RegistroAuditoria>>.Exito(registros);
    }
}
