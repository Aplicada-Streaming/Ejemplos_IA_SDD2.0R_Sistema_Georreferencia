using GeoVial.Shared;

namespace GeoVial.Revision;

/// <summary>Proyección de un marcador para el mapa interactivo (US-21): lo mínimo que el front pinta y muestra.</summary>
public sealed record PinMapa(
    Guid MarcadorId,
    double Latitud,
    double Longitud,
    bool EnConflicto,
    int Fotos,
    int Comentarios);

/// <summary>
/// Vista del mapa interactivo de la revisión (US-21, CU-08): a partir de los marcadores georreferenciados
/// calcula los pines a dibujar, el centro y la caja contenedora (bounds) para encuadrar el mapa. Lógica pura
/// (sin dependencia de Leaflet ni de plataforma), reutilizable por el front web. Con cero marcadores usa un
/// centro por defecto para que el mapa muestre algo razonable.
/// </summary>
public sealed class VistaMapa
{
    // Centro aproximado de Argentina, usado cuando el relevamiento aún no tiene marcadores georreferenciados.
    public const double CentroPorDefectoLat = -38.0;
    public const double CentroPorDefectoLon = -63.0;

    public IReadOnlyList<PinMapa> Pins { get; }

    public bool HayPins => Pins.Count > 0;

    public double CentroLat { get; }
    public double CentroLon { get; }

    public double MinLat { get; }
    public double MinLon { get; }
    public double MaxLat { get; }
    public double MaxLon { get; }

    public VistaMapa(IReadOnlyList<RevisionMarcadorDto> marcadores)
    {
        ArgumentNullException.ThrowIfNull(marcadores);

        var pins = new List<PinMapa>(marcadores.Count);
        foreach (var m in marcadores)
        {
            pins.Add(new PinMapa(
                m.MarcadorId, (double)m.Latitud, (double)m.Longitud, m.EnConflicto, m.Fotos.Count, m.Comentarios.Count));
        }

        Pins = pins;

        if (pins.Count == 0)
        {
            CentroLat = MinLat = MaxLat = CentroPorDefectoLat;
            CentroLon = MinLon = MaxLon = CentroPorDefectoLon;
            return;
        }

        MinLat = pins.Min(p => p.Latitud);
        MaxLat = pins.Max(p => p.Latitud);
        MinLon = pins.Min(p => p.Longitud);
        MaxLon = pins.Max(p => p.Longitud);
        CentroLat = (MinLat + MaxLat) / 2.0;
        CentroLon = (MinLon + MaxLon) / 2.0;
    }
}
