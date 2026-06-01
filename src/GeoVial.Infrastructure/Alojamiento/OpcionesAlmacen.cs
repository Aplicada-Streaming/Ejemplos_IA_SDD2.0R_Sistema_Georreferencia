namespace GeoVial.Infrastructure.Alojamiento;

/// <summary>
/// Configuración del backend de alojamiento de fotos que selecciona el usuario raíz (ADR-08, extensibilidad §4).
/// Los secretos del backend (credenciales de S3) viven en el secret store del entorno, no en git.
/// </summary>
public sealed class OpcionesAlmacen
{
    public const string Seccion = "Almacen";

    /// <summary>Backend activo: <c>Local</c> (por defecto) o <c>S3</c>.</summary>
    public string Backend { get; set; } = "Local";

    /// <summary>Carpeta raíz del backend local. Por defecto, una carpeta bajo el directorio de la aplicación.</summary>
    public string RutaLocal { get; set; } = "almacen-fotos";

    /// <summary>Nombre del bucket de S3 cuando el backend activo es <c>S3</c>.</summary>
    public string BucketS3 { get; set; } = string.Empty;

    /// <summary>Región de AWS cuando el backend activo es <c>S3</c> (por ejemplo, <c>us-east-1</c>).</summary>
    public string RegionS3 { get; set; } = string.Empty;

    public bool EsS3 => string.Equals(Backend, "S3", StringComparison.OrdinalIgnoreCase);
}
