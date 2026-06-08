using FluentAssertions;
using GeoVial.Application.Configuracion;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Política de migración al desplegar: auto-migrar sólo en Development; en producción la migración es un paso
/// explícito del despliegue (comando "migrate").
/// </summary>
public class PoliticaMigracionTests
{
    [Fact] // Development: auto-migra al arrancar (instancia única)
    public void Development_auto_migra()
    {
        PoliticaMigracion.DebeAutoMigrarAlArrancar("Development").Should().BeTrue();
    }

    [Theory] // producción / staging / tests: NO auto-migra (migración por paso de despliegue)
    [InlineData("Production")]
    [InlineData("Staging")]
    [InlineData("")]
    public void Fuera_de_development_no_auto_migra(string entorno)
    {
        PoliticaMigracion.DebeAutoMigrarAlArrancar(entorno).Should().BeFalse();
    }

    [Fact] // "migrate" como primer argumento → modo migrar y salir
    public void Reconoce_el_comando_migrate()
    {
        PoliticaMigracion.EsComandoMigrar(new[] { "migrate" }).Should().BeTrue();
        PoliticaMigracion.EsComandoMigrar(new[] { "MIGRATE", "extra" }).Should().BeTrue();
    }

    [Fact] // sin argumentos → arranque normal (no migra y sale)
    public void Sin_argumentos_no_es_comando_migrar()
    {
        PoliticaMigracion.EsComandoMigrar(Array.Empty<string>()).Should().BeFalse();
    }

    [Fact] // otro comando → arranque normal
    public void Otro_comando_no_es_migrar()
    {
        PoliticaMigracion.EsComandoMigrar(new[] { "otro" }).Should().BeFalse();
    }
}
