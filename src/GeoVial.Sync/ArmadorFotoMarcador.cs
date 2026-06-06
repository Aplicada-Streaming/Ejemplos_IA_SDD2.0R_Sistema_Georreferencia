namespace GeoVial.Sync;

/// <summary>
/// Arma la captura para "agregar una foto a un marcador existente" (US-15, CU-09) desde el carrusel de revisión.
/// A diferencia de <see cref="GeoVial.CapturaCampo.ArmadorCapturaCampo"/> (que deriva la coordenada del EXIF de la
/// foto, RN-03), aquí la coordenada SIEMPRE es la del marcador en foco: así una foto del catálogo tomada en otro
/// lugar igual cae en <i>este</i> marcador. La agrupación por radio del backend (RN-02), al recibir la coordenada
/// exacta del marcador (distancia 0), reusa ese mismo marcador en vez de crear uno nuevo. Núcleo puro y testeable;
/// la página móvil sólo toma el binario, llama a <see cref="Armar"/> y encola el <see cref="CapturaPendiente"/>.
/// </summary>
public sealed class ArmadorFotoMarcador
{
    /// <summary>
    /// Construye la captura pendiente con la coordenada del marcador. Devuelve <c>null</c> si el binario está
    /// vacío o la coordenada del marcador está fuera de rango geográfico (lat −90..90, lon −180..180, RN-03).
    /// <paramref name="capturaId"/> y <paramref name="momento"/> los provee el llamador (se mantienen como
    /// parámetros para que el armado sea determinista y testeable).
    /// </summary>
    public CapturaPendiente? Armar(
        Guid capturaId,
        Guid relevamientoId,
        decimal latitudMarcador,
        decimal longitudMarcador,
        byte[]? foto,
        string? referenciaArchivo,
        DateTime momento)
    {
        if (foto is null || foto.Length == 0)
        {
            return null;
        }

        if (latitudMarcador is < -90m or > 90m || longitudMarcador is < -180m or > 180m)
        {
            return null;
        }

        var nombre = string.IsNullOrWhiteSpace(referenciaArchivo) ? "foto.jpg" : referenciaArchivo!;
        return new CapturaPendiente(capturaId, relevamientoId, nombre, latitudMarcador, longitudMarcador, foto, momento);
    }
}
