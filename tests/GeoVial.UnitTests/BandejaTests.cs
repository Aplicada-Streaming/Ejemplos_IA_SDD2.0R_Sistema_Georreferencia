using FluentAssertions;
using GeoVial.Revision;
using GeoVial.Shared;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Presentador de la bandeja sin georreferenciar (S50, RN-03): ordena las observaciones que esperan ubicación
/// manual (más recientes primero) y arma una etiqueta legible (momento + nombre de la foto) para la app.
/// </summary>
public class PresentadorBandejaTests
{
    private static ObservacionSinGeoDto Entrada(DateTime momento, string? foto) =>
        new(Guid.NewGuid(), momento, foto);

    [Fact] // ordena de la más reciente a la más antigua
    public void Ordena_por_momento_descendente()
    {
        var vieja = Entrada(new DateTime(2026, 6, 1, 8, 0, 0, DateTimeKind.Utc), "obra/a.jpg");
        var nueva = Entrada(new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc), "obra/b.jpg");

        var filas = PresentadorBandeja.Presentar(new[] { vieja, nueva });

        filas.Select(f => f.ObservacionId).Should().ContainInOrder(nueva.ObservacionId, vieja.ObservacionId);
    }

    [Fact] // la etiqueta lleva el momento y el nombre de archivo (sin la ruta)
    public void Etiqueta_muestra_momento_y_nombre_de_archivo()
    {
        var entrada = Entrada(new DateTime(2026, 6, 1, 9, 30, 0, DateTimeKind.Utc), "obra/2026/sin-gps.jpg");

        var fila = PresentadorBandeja.Presentar(new[] { entrada }).Single();

        fila.Etiqueta.Should().Be("Sin ubicar · 01/06 09:30 · sin-gps.jpg");
    }

    [Fact] // sin referencia de foto, la etiqueta lo indica
    public void Etiqueta_sin_foto_lo_indica()
    {
        var entrada = Entrada(new DateTime(2026, 6, 1, 9, 30, 0, DateTimeKind.Utc), null);

        PresentadorBandeja.Presentar(new[] { entrada }).Single().Etiqueta.Should().EndWith("(sin foto)");
    }

    [Fact] // bandeja nula o vacía → sin filas
    public void Bandeja_vacia_no_produce_filas()
    {
        PresentadorBandeja.Presentar(null).Should().BeEmpty();
        PresentadorBandeja.Presentar(Array.Empty<ObservacionSinGeoDto>()).Should().BeEmpty();
    }
}
