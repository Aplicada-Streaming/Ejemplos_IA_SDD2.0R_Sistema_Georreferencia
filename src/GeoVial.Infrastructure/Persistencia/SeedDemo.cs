using GeoVial.Domain;
using Microsoft.EntityFrameworkCore;

namespace GeoVial.Infrastructure.Persistencia;

/// <summary>
/// Seed de datos de demostración (solo Development): crea un relevamiento en recolección sobre el área
/// sembrada, para que la app móvil tenga un destino de sincronización al probar el ciclo capturar → sync.
/// Idempotente: no crea uno nuevo si ya existe algún relevamiento.
/// </summary>
public static class SeedDemo
{
    public static async Task EjecutarAsync(GeoVialDbContext db, CancellationToken ct = default)
    {
        if (await db.Relevamientos.AnyAsync(ct))
        {
            return;
        }

        var area = await db.Areas.FirstOrDefaultAsync(ct);
        if (area is null)
        {
            return;
        }

        var relevamiento = Relevamiento.Crear("Obra demo — Puente Río 12", 15m, area.AreaId).Valor!;
        await db.Relevamientos.AddAsync(relevamiento, ct);
        await db.SaveChangesAsync(ct);
    }
}
