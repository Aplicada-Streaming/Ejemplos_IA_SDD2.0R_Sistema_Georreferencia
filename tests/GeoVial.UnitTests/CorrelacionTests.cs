using FluentAssertions;
using GeoVial.Application.Observabilidad;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Identificador de correlación de request (observabilidad): reusa un id entrante válido o genera uno nuevo,
/// para rastrear una petición de punta a punta en los logs.
/// </summary>
public class CorrelacionTests
{
    [Fact] // un id entrante válido se reusa (recortado), para correlacionar a través de saltos
    public void Reusa_el_id_entrante_valido()
    {
        Correlacion.Resolver("  abc-123  ").Should().Be("abc-123");
    }

    [Theory] // entrante inválido o ausente → genera uno nuevo, no vacío y válido
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Genera_uno_nuevo_si_falta(string? entrante)
    {
        var id = Correlacion.Resolver(entrante);

        id.Should().NotBeNullOrWhiteSpace();
        Correlacion.EsValido(id).Should().BeTrue();
    }

    [Fact] // un id demasiado largo se descarta y se genera uno nuevo (no se propaga una cabecera abusiva)
    public void Descarta_un_id_demasiado_largo()
    {
        var largo = new string('x', Correlacion.LargoMaximo + 1);

        Correlacion.EsValido(largo).Should().BeFalse();
        Correlacion.Resolver(largo).Should().NotBe(largo);
    }

    [Fact] // un id con caracteres de control se descarta (evita inyección en logs)
    public void Descarta_un_id_con_caracteres_de_control()
    {
        Correlacion.EsValido("abc\n123").Should().BeFalse();
    }

    [Fact] // el id generado es un GUID compacto (sin guiones), apto para logs y cabeceras
    public void El_id_generado_es_un_guid_compacto()
    {
        var id = Correlacion.Resolver(null);

        id.Should().HaveLength(32);
        id.Should().MatchRegex("^[0-9a-f]{32}$");
    }
}
