using FluentAssertions;
using GeoVial.Revision;
using GeoVial.Shared;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Cálculo de la vista del mapa interactivo de la revisión (US-21): pines, centro y bounds.</summary>
public class VistaMapaTests
{
    private static RevisionMarcadorDto Marcador(
        decimal lat, decimal lon, bool enConflicto = false, int fotos = 0, int comentarios = 0) =>
        new(Guid.NewGuid(), lat, lon, enConflicto,
            Enumerable.Range(0, fotos).Select(_ => new RevisionFotoDto(Guid.NewGuid(), "f.jpg", Array.Empty<string>())).ToArray(),
            Enumerable.Range(0, comentarios).Select(_ => new RevisionComentarioDto(Guid.NewGuid(), "c", null, Array.Empty<string>())).ToArray());

    [Fact] // sin marcadores: no hay pines y el centro es el por defecto
    public void Sin_marcadores_usa_centro_por_defecto()
    {
        var vista = new VistaMapa(Array.Empty<RevisionMarcadorDto>());

        vista.HayPins.Should().BeFalse();
        vista.Pins.Should().BeEmpty();
        vista.CentroLat.Should().Be(VistaMapa.CentroPorDefectoLat);
        vista.CentroLon.Should().Be(VistaMapa.CentroPorDefectoLon);
    }

    [Fact] // un marcador: el centro queda en él y los bounds lo encierran
    public void Un_marcador_centra_en_el()
    {
        var vista = new VistaMapa(new[] { Marcador(-34.6m, -58.4m) });

        vista.HayPins.Should().BeTrue();
        vista.Pins.Should().ContainSingle();
        vista.CentroLat.Should().BeApproximately(-34.6, 1e-9);
        vista.CentroLon.Should().BeApproximately(-58.4, 1e-9);
        vista.MinLat.Should().BeApproximately(-34.6, 1e-9);
        vista.MaxLat.Should().BeApproximately(-34.6, 1e-9);
    }

    [Fact] // varios marcadores: el centro es el medio de la caja y los bounds son los extremos
    public void Varios_marcadores_calculan_centro_y_bounds()
    {
        var vista = new VistaMapa(new[]
        {
            Marcador(-34.0m, -58.0m),
            Marcador(-36.0m, -60.0m),
            Marcador(-35.0m, -59.0m),
        });

        vista.Pins.Should().HaveCount(3);
        vista.MinLat.Should().BeApproximately(-36.0, 1e-9);
        vista.MaxLat.Should().BeApproximately(-34.0, 1e-9);
        vista.MinLon.Should().BeApproximately(-60.0, 1e-9);
        vista.MaxLon.Should().BeApproximately(-58.0, 1e-9);
        vista.CentroLat.Should().BeApproximately(-35.0, 1e-9);
        vista.CentroLon.Should().BeApproximately(-59.0, 1e-9);
    }

    [Fact] // la proyección conserva el conflicto y los conteos de fotos y comentarios
    public void Pin_proyecta_conflicto_y_conteos()
    {
        var m = Marcador(-34.6m, -58.4m, enConflicto: true, fotos: 2, comentarios: 3);

        var pin = new VistaMapa(new[] { m }).Pins.Single();

        pin.MarcadorId.Should().Be(m.MarcadorId);
        pin.EnConflicto.Should().BeTrue();
        pin.Fotos.Should().Be(2);
        pin.Comentarios.Should().Be(3);
    }

    [Fact] // marcadores nulos: falla con argumento nulo
    public void Marcadores_nulos_falla()
    {
        var crear = () => new VistaMapa(null!);
        crear.Should().Throw<ArgumentNullException>();
    }
}
