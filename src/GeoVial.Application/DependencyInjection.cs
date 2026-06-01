using GeoVial.Application.Cqrs;
using GeoVial.Application.Relevamientos;
using GeoVial.Application.Servicios;
using GeoVial.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace GeoVial.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection servicios)
    {
        // Servicios de aplicación directos (módulo de usuarios y acceso).
        servicios.AddScoped<GestionUsuariosService>();
        servicios.AddScoped<AccesoService>();
        servicios.AddScoped<AutorizacionService>();

        // CQRS ligero del módulo de relevamientos (ADR-01, PROJECT-README §3).
        servicios.AddScoped<IMediador, Mediador>();
        servicios.AddScoped<IManejador<CrearRelevamientoCommand, Resultado<Relevamiento>>, CrearRelevamientoHandler>();
        servicios.AddScoped<IManejador<AsignarAgentesCommand, Resultado>, AsignarAgentesHandler>();
        servicios.AddScoped<IManejador<ReasignarAgentesCommand, Resultado>, ReasignarAgentesHandler>();
        servicios.AddScoped<IManejador<TransicionarEstadoCommand, Resultado>, TransicionarEstadoHandler>();
        servicios.AddScoped<IManejador<ReabrirRelevamientoCommand, Resultado>, ReabrirRelevamientoHandler>();
        servicios.AddScoped<IManejador<ListarRelevamientosQuery, IReadOnlyList<Relevamiento>>, ListarRelevamientosHandler>();

        return servicios;
    }
}
