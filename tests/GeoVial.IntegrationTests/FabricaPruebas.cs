using GeoVial.Infrastructure.Persistencia;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GeoVial.IntegrationTests;

/// <summary>
/// Fábrica de pruebas de integración: corre la API en entorno <c>Development</c> pero fuerza el proveedor
/// de base **en memoria**, aislado por instancia de fábrica. Esto desacopla las pruebas de la cadena de
/// conexión a SQL Server que <c>appsettings.Development.json</c> define para el desarrollo real (host DEV):
/// el gate sigue corriendo sin SQL Server, determinista y aislado, como antes de cablear la base real.
/// </summary>
public sealed class FabricaPruebas : WebApplicationFactory<Program>
{
    private readonly string _nombreBase = $"pruebas-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(servicios =>
        {
            // Quita el registro de SQL Server (DbContextOptions / configuración) que dejó AddInfrastructure
            // y vuelve a registrar el DbContext sobre el proveedor en memoria.
            var aQuitar = servicios.Where(d =>
                d.ServiceType == typeof(GeoVialDbContext) ||
                (d.ServiceType.FullName?.Contains("DbContextOptions", StringComparison.Ordinal) ?? false)).ToList();
            foreach (var d in aQuitar)
            {
                servicios.Remove(d);
            }

            servicios.AddDbContext<GeoVialDbContext>(o => o.UseInMemoryDatabase(_nombreBase));
        });
    }
}
