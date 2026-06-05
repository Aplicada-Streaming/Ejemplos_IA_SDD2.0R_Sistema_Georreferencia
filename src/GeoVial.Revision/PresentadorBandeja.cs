using System.Globalization;
using GeoVial.Shared;

namespace GeoVial.Revision;

/// <summary>Fila de la bandeja sin georreferenciar lista para mostrar: la observación y una etiqueta legible.</summary>
public sealed record FilaBandeja(Guid ObservacionId, string Etiqueta);

/// <summary>
/// Presenta la bandeja sin georreferenciar (RN-03, S50): ordena las observaciones que esperan ubicación
/// manual (CU-05) de la más reciente a la más antigua y arma una etiqueta legible (momento + nombre de la
/// foto). Núcleo testeable del listado del móvil, al estilo de <see cref="NavegadorRevision"/>; la pantalla
/// sólo enlaza las filas. No depende de la plataforma (vive en GeoVial.Revision, en el gate).
/// </summary>
public static class PresentadorBandeja
{
    /// <summary>Convierte las entradas de la bandeja en filas ordenadas (más recientes primero) con su etiqueta.</summary>
    public static IReadOnlyList<FilaBandeja> Presentar(IReadOnlyList<ObservacionSinGeoDto>? bandeja)
    {
        if (bandeja is null || bandeja.Count == 0)
        {
            return Array.Empty<FilaBandeja>();
        }

        return bandeja
            .OrderByDescending(o => o.MomentoCaptura)
            .Select(o => new FilaBandeja(o.ObservacionId, Etiqueta(o)))
            .ToList();
    }

    private static string Etiqueta(ObservacionSinGeoDto o) =>
        $"Sin ubicar · {o.MomentoCaptura.ToString("dd/MM HH:mm", CultureInfo.InvariantCulture)} · {NombreArchivo(o.ReferenciaArchivo)}";

    private static string NombreArchivo(string? referencia)
    {
        if (string.IsNullOrWhiteSpace(referencia))
        {
            return "(sin foto)";
        }

        var corte = referencia.LastIndexOfAny(new[] { '/', '\\' });
        return corte >= 0 && corte < referencia.Length - 1 ? referencia[(corte + 1)..] : referencia;
    }
}
