using GeoVial.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
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
            if (!await db.Database.CanConnectAsync(cancellationToken))
            {
                return HealthCheckResult.Unhealthy("No se puede conectar a la base de datos.");
            }

            // Readiness ampliado (hardening): no servir tráfico si el esquema no está al día. En producción la
            // migración es un paso de despliegue; hasta que corra, el readiness no debe dar OK.
            if (db.Database.IsRelational())
            {
                var pendientes = (await db.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
                if (pendientes.Count > 0)
                {
                    return HealthCheckResult.Unhealthy(
                        $"Hay {pendientes.Count} migración(es) pendiente(s); ejecutá el paso de migración antes de servir tráfico.");
                }
            }

            return HealthCheckResult.Healthy("Base de datos alcanzable y al día.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Error al consultar la base de datos.", ex);
        }
    }
}
