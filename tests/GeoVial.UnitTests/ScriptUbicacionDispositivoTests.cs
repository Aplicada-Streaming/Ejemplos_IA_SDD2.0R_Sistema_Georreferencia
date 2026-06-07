using FluentAssertions;
using GeoVial.Revision;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>H-02 (auditoría UX) — el script de "Centrar por GPS" centra el mapa y marca la posición del dispositivo.</summary>
public class ScriptUbicacionDispositivoTests
{
    [Fact] // centra el mapa en la coordenada dada
    public void Centra_el_mapa_en_la_coordenada()
    {
        var js = ScriptUbicacionDispositivo.Centrar(-34.6037, -58.3816);

        js.Should().Contain("mapa.setView");
        js.Should().Contain("-34.6037");
        js.Should().Contain("-58.3816");
    }

    [Fact] // deja una marca distinta de "tu posición" (no se confunde con los pines de marcadores)
    public void Coloca_la_marca_de_tu_posicion()
    {
        var js = ScriptUbicacionDispositivo.Centrar(0, 0);

        js.Should().Contain("circleMarker");
        js.Should().Contain("Tu posición");
        js.Should().Contain("__posDisp"); // marca reutilizable: se reemplaza al recentrar, no se acumula
    }

    [Fact] // usa punto decimal (InvariantCulture), no coma, para que el JS sea válido
    public void Usa_punto_decimal()
    {
        var js = ScriptUbicacionDispositivo.Centrar(-34.5, -58.5);

        js.Should().Contain("-34.5");
        js.Should().NotContain("-34,5");
    }

    [Fact] // es defensivo: si la variable del mapa no existe, no rompe
    public void Es_defensivo_si_no_hay_mapa()
    {
        ScriptUbicacionDispositivo.Centrar(1, 2).Should().Contain("typeof mapa==='undefined'");
    }
}
