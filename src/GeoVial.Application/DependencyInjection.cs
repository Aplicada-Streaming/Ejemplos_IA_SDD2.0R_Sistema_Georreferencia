using GeoVial.Application.Servicios;
using Microsoft.Extensions.DependencyInjection;

namespace GeoVial.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection servicios)
    {
        servicios.AddScoped<GestionUsuariosService>();
        servicios.AddScoped<AccesoService>();
        servicios.AddScoped<AutorizacionService>();
        return servicios;
    }
}
