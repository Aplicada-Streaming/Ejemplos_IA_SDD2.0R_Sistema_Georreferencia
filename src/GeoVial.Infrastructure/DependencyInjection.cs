using Amazon;
using Amazon.S3;
using GeoVial.Application.Abstracciones;
using GeoVial.FileHosting;
using GeoVial.Infrastructure.Alojamiento;
using GeoVial.Infrastructure.ExportImport;
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
        servicios.AddSingleton<IEmpaquetadorRelevamiento, EmpaquetadorZip>();
        RegistrarAlojamiento(servicios, configuracion);
        RegistrarPipelineImagen(servicios, configuracion);
        servicios.AddSingleton<IHasherClave, HasherClavePbkdf2>();
        servicios.AddScoped<IServicioToken, ServicioTokenJwt>();
        servicios.AddSingleton<IRelojUtc, RelojUtc>();

        return servicios;
    }

    /// <summary>
    /// Registra el backend de alojamiento de fotos que el usuario raíz selecciona por configuración (ADR-08).
    /// Por defecto, sin configuración, usa el backend local; con <c>Almacen:Backend=S3</c> usa AWS S3.
    /// </summary>
    private static void RegistrarAlojamiento(IServiceCollection servicios, IConfiguration configuracion)
    {
        var seccion = configuracion.GetSection(OpcionesAlmacen.Seccion);
        var opciones = new OpcionesAlmacen
        {
            Backend = seccion["Backend"] ?? "Local",
            RutaLocal = seccion["RutaLocal"] ?? "almacen-fotos",
            BucketS3 = seccion["BucketS3"] ?? string.Empty,
            RegionS3 = seccion["RegionS3"] ?? string.Empty,
        };

        if (opciones.EsS3)
        {
            servicios.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(RegionEndpoint.GetBySystemName(opciones.RegionS3)));
            servicios.AddSingleton<IAlmacenFotos>(sp => new AlmacenS3(sp.GetRequiredService<IAmazonS3>(), opciones.BucketS3));
        }
        else
        {
            servicios.AddSingleton<IAlmacenFotos>(new AlmacenLocal(opciones.RutaLocal));
        }
    }

    /// <summary>Registra el pipeline de imágenes que comprime/redimensiona las fotos al subirlas (BT-19).</summary>
    private static void RegistrarPipelineImagen(IServiceCollection servicios, IConfiguration configuracion)
    {
        var seccion = configuracion.GetSection(OpcionesImagen.Seccion);
        var opciones = new OpcionesImagen();
        if (int.TryParse(seccion["MaxDimension"], out var maxDimension) && maxDimension > 0)
        {
            opciones.MaxDimension = maxDimension;
        }

        if (int.TryParse(seccion["CalidadJpeg"], out var calidad) && calidad is > 0 and <= 100)
        {
            opciones.CalidadJpeg = calidad;
        }

        servicios.AddSingleton<IPipelineImagen>(new PipelineImagenSkia(opciones.MaxDimension, opciones.CalidadJpeg));
    }
}
