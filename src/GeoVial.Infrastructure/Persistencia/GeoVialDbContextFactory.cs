using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GeoVial.Infrastructure.Persistencia;

/// <summary>
/// Factory de diseño para las herramientas de EF Core (migraciones). Usa SQL Server sobre el host
/// local "DEV" (README §16: desarrollo íntegramente local). La cadena se puede sobrescribir con la
/// variable de entorno GEOVIAL_DB.
/// </summary>
public sealed class GeoVialDbContextFactory : IDesignTimeDbContextFactory<GeoVialDbContext>
{
    public const string CadenaPorDefecto =
        "Server=DEV;Database=GeoVial;Trusted_Connection=True;TrustServerCertificate=True";

    public GeoVialDbContext CreateDbContext(string[] args)
    {
        var cadena = Environment.GetEnvironmentVariable("GEOVIAL_DB") ?? CadenaPorDefecto;
        var opciones = new DbContextOptionsBuilder<GeoVialDbContext>()
            .UseSqlServer(cadena)
            .Options;
        return new GeoVialDbContext(opciones);
    }
}
