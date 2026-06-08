using FluentAssertions;
using GeoVial.Application.Configuracion;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Cabeceras de seguridad que el backend agrega a toda respuesta (hardening).</summary>
public class CabecerasSeguridadTests
{
    [Theory] // están las defensas de bajo costo esperadas, con su valor
    [InlineData("X-Content-Type-Options", "nosniff")]
    [InlineData("X-Frame-Options", "DENY")]
    [InlineData("Referrer-Policy", "no-referrer")]
    [InlineData("Cross-Origin-Resource-Policy", "same-origin")]
    public void Incluye_las_cabeceras_de_seguridad(string nombre, string valor)
    {
        CabecerasSeguridad.Predeterminadas.Should().ContainKey(nombre).WhoseValue.Should().Be(valor);
    }
}
