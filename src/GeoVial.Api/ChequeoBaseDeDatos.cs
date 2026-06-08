using GeoVial.Infrastructure.Persistencia;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GeoVial.Api;

/// <summary>
/// Health check de readiness (hardening): verifica que la base de datos sea alcanzable, para que un orquestador
/// (contenedor / balanceador) no mande tráfico al backend hasta que su dependencia crítica esté lista. Se expone
/// en <c>/health/ready</c>; el liveness (<c>/health</c>) sólo confirma que el proceso responde.
/// </summary>
internal sealed class ChequeoBaseDeDatos(GeoVialDbContext db) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            return await db.Database.CanConnectAsync(cancellationToken)
                ? HealthCheckResult.Healthy("Base de datos alcanzable.")
                : HealthCheckResult.Unhealthy("No se puede conectar a la base de datos.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Error al conectar a la base de datos.", ex);
        }
    }
}
