namespace GeoVial.Infrastructure.Seguridad;

/// <summary>Configuración del emisor de JWT (ADR-03). La clave secreta vive fuera de git (README §8).</summary>
public sealed class JwtOptions
{
    public const string Seccion = "Jwt";

    public string Issuer { get; set; } = "geovial";
    public string Audience { get; set; } = "geovial-clients";
    public string ClaveSecreta { get; set; } = string.Empty;
    public int MinutosAcceso { get; set; } = 60;
}
