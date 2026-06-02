using GeoVial.Application.Captura;
using GeoVial.Application.Conflictos;
using GeoVial.Application.Cqrs;
using GeoVial.Application.ExportImport;
using GeoVial.Application.Relevamientos;
using GeoVial.Application.Revision;
using GeoVial.Application.Servicios;
using GeoVial.Application.Sincronizacion;
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
        servicios.AddScoped<IManejador<SubirContenidoFotoCommand, Resultado>, SubirContenidoFotoHandler>();
        servicios.AddScoped<IManejador<DescargarContenidoFotoQuery, byte[]?>, DescargarContenidoFotoHandler>();

        // Módulo de revisión sobre mapa y gestión de marcador (CU-08, CU-09).
        servicios.AddScoped<IManejador<AgregarComentarioCommand, Resultado>, AgregarComentarioHandler>();
        servicios.AddScoped<IManejador<EtiquetarFotoCommand, Resultado>, EtiquetarFotoHandler>();
        servicios.AddScoped<IManejador<EtiquetarComentarioCommand, Resultado>, EtiquetarComentarioHandler>();
        servicios.AddScoped<IManejador<RevisarRelevamientoQuery, RevisionRelevamiento?>, RevisarRelevamientoHandler>();

        // Módulo de detección y resolución de conflictos por radio (CU-11, CU-12; EP-06).
        servicios.AddScoped<IManejador<DetectarConflictosCommand, Resultado<IReadOnlyList<ConflictoDetectado>>>, DetectarConflictosHandler>();
        servicios.AddScoped<IManejador<AjustarRadioCommand, Resultado>, AjustarRadioHandler>();
        servicios.AddScoped<IManejador<ResolverConflictoCommand, Resultado>, ResolverConflictoHandler>();
        servicios.AddScoped<IManejador<ConflictosPendientesQuery, IReadOnlyList<ConflictoPendiente>>, ConflictosPendientesHandler>();

        // Módulo de exportación e importación del relevamiento completo (CU-08 §5.A/§5.B; EP-07).
        servicios.AddScoped<IManejador<ExportarRelevamientoCommand, Resultado<ArchivoExportado>>, ExportarRelevamientoHandler>();
        servicios.AddScoped<IManejador<ImportarRelevamientoCommand, Resultado<Guid>>, ImportarRelevamientoHandler>();

        // Módulo de sincronización: consolidación backend (CU-07; EP-04).
        servicios.AddScoped<IManejador<SincronizarCommand, Resultado<ResultadoSincronizacion>>, SincronizarHandler>();

        return servicios;
    }
}
