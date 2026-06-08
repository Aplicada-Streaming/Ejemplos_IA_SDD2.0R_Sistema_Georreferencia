using FluentAssertions;
using GeoVial.Application.Configuracion;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Guardrails de arranque para producción (hardening): el backend no debe levantar inseguro (base en memoria o
/// clave JWT de desarrollo). En entornos no productivos la validación es permisiva.
/// </summary>
public class GuardrailsProduccionTests
{
    private const string ClaveProdValida = "una-clave-de-produccion-bien-larga-y-secreta-123456";

    [Theory] // Development/Staging/tests: permisivo aunque falten cadena y clave (ahí InMemory es válido)
    [InlineData("Development")]
    [InlineData("Staging")]
    [InlineData("")]
    public void Fuera_de_produccion_no_hay_errores(string entorno)
    {
        GuardrailsProduccion.Validar(entorno, cadenaConexion: null, claveJwt: null).Should().BeEmpty();
    }

    [Fact] // Producción con configuración válida: sin errores
    public void Produccion_con_config_valida_no_tiene_errores()
    {
        var errores = GuardrailsProduccion.Validar("Production", "Server=db;Database=GeoVial;User Id=app;Password=x;", ClaveProdValida);

        errores.Should().BeEmpty();
    }

    [Fact] // Producción sin cadena de conexión: error (no se permite InMemory en producción)
    public void Produccion_sin_cadena_de_conexion_falla()
    {
        var errores = GuardrailsProduccion.Validar("Production", cadenaConexion: " ", claveJwt: ClaveProdValida);

        errores.Should().ContainSingle().Which.Should().Contain("ConnectionStrings:GeoVial");
    }

    [Fact] // Producción con la clave JWT de desarrollo: error (es pública)
    public void Produccion_con_clave_de_desarrollo_falla()
    {
        var errores = GuardrailsProduccion.Validar("Production", "Server=db;", GuardrailsProduccion.ClaveJwtDesarrollo);

        errores.Should().Contain(e => e.Contains("clave de desarrollo"));
    }

    [Fact] // Producción con clave JWT corta: error (HS256 exige ≥ 32 bytes)
    public void Produccion_con_clave_corta_falla()
    {
        var errores = GuardrailsProduccion.Validar("Production", "Server=db;", "corta");

        errores.Should().Contain(e => e.Contains("al menos"));
    }

    [Fact] // Producción sin clave: error
    public void Produccion_sin_clave_falla()
    {
        var errores = GuardrailsProduccion.Validar("Production", "Server=db;", claveJwt: null);

        errores.Should().Contain(e => e.Contains("Jwt:ClaveSecreta es obligatoria"));
    }

    [Fact] // "production" en minúsculas también es producción (comparación sin distinguir mayúsculas)
    public void Produccion_es_insensible_a_mayusculas()
    {
        GuardrailsProduccion.Validar("production", null, null).Should().NotBeEmpty();
    }
}
