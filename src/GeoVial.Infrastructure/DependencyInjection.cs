using GeoVial.Application.Abstracciones;
using GeoVial.Infrastructure.Persistencia;
using GeoVial.Infrastructure.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeoVial.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection servicios, IConfiguration configuracion)
    {
        // ADR-09: SQL Server vía EF Core. En desarrollo/tests sin SQL Server se usa el proveedor en memoria.
        var cadena = configuracion.GetConnectionString("GeoVial");
        if (string.IsNullOrWhiteSpace(cadena))
        {
            servicios.AddDbContext<GeoVialDbContext>(o => o.UseInMemoryDatabase("geovial-dev"));
        }
        else
        {
            servicios.AddDbContext<GeoVialDbContext>(o => o.UseSqlServer(cadena));
        }

        servicios.Configure<JwtOptions>(configuracion.GetSection(JwtOptions.Seccion));

        servicios.AddScoped<IUsuarioRepository, UsuarioRepository>();
        servicios.AddScoped<IAreaRepository, AreaRepository>();
        servicios.AddScoped<IRelevamientoRepository, RelevamientoRepository>();
        servicios.AddScoped<IMarcadorRepository, MarcadorRepository>();
        servicios.AddScoped<IObservacionRepository, ObservacionRepository>();
        servicios.AddScoped<IFotoRepository, FotoRepository>();
        servicios.AddScoped<IComentarioRepository, ComentarioRepository>();
        servicios.AddScoped<IEtiquetaRepository, EtiquetaRepository>();
        servicios.AddScoped<IConflictoRepository, ConflictoRepository>();
        servicios.AddScoped<ICredencialRepository, CredencialRepository>();
        servicios.AddScoped<IServicioAuditoria, ServicioAuditoria>();
        servicios.AddSingleton<IHasherClave, HasherClavePbkdf2>();
        servicios.AddScoped<IServicioToken, ServicioTokenJwt>();
        servicios.AddSingleton<IRelojUtc, RelojUtc>();

        return servicios;
    }
}
