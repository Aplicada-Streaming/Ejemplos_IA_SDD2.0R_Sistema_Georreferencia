using GeoVial.Application.Captura;
using GeoVial.Application.Cqrs;
using GeoVial.Application.Relevamientos;
using GeoVial.Application.Revision;
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
        servicios.AddScoped<ProvisionCredencialService>();

        // CQRS ligero del módulo de relevamientos (ADR-01, PROJECT-README §3).
        servicios.AddScoped<IMediador, Mediador>();
        servicios.AddScoped<IManejador<CrearRelevamientoCommand, Resultado<Relevamiento>>, CrearRelevamientoHandler>();
        servicios.AddScoped<IManejador<AsignarAgentesCommand, Resultado>, AsignarAgentesHandler>();
        servicios.AddScoped<IManejador<ReasignarAgentesCommand, Resultado>, ReasignarAgentesHandler>();
        servicios.AddScoped<IManejador<TransicionarEstadoCommand, Resultado>, TransicionarEstadoHandler>();
        servicios.AddScoped<IManejador<ReabrirRelevamientoCommand, Resultado>, ReabrirRelevamientoHandler>();
        servicios.AddScoped<IManejador<ListarRelevamientosQuery, IReadOnlyList<Relevamiento>>, ListarRelevamientosHandler>();

        // Módulo de captura y georreferenciación (CU-04, CU-05).
        servicios.AddScoped<IManejador<CapturarObservacionCommand, Resultado<ResultadoCaptura>>, CapturarObservacionHandler>();
        servicios.AddScoped<IManejador<UbicarObservacionManualCommand, Resultado>, UbicarObservacionManualHandler>();
        servicios.AddScoped<IManejador<ListarObservacionesQuery, IReadOnlyList<Observacion>>, ListarObservacionesHandler>();

        // Módulo de revisión sobre mapa y gestión de marcador (CU-08, CU-09).
        servicios.AddScoped<IManejador<AgregarComentarioCommand, Resultado>, AgregarComentarioHandler>();
        servicios.AddScoped<IManejador<EtiquetarFotoCommand, Resultado>, EtiquetarFotoHandler>();
        servicios.AddScoped<IManejador<EtiquetarComentarioCommand, Resultado>, EtiquetarComentarioHandler>();
        servicios.AddScoped<IManejador<RevisarRelevamientoQuery, RevisionRelevamiento?>, RevisarRelevamientoHandler>();

        return servicios;
    }
}
