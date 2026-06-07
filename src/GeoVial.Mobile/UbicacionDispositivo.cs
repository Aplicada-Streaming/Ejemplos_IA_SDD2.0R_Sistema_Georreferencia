using GeoVial.Revision;

namespace GeoVial.Mobile;

/// <summary>
/// Obtiene la posición del dispositivo para "Centrar por GPS" (H-02 de la auditoría UX; experiencia §6). Pide el
/// permiso de ubicación en runtime (Android 6+) y consulta el GPS con MAUI Geolocation. No lanza: devuelve
/// <c>null</c> si el permiso se deniega o no hay una lectura, para que la UI lo informe sin tumbar la app. Glue de
/// plataforma (estático sobre las API de MAUI); la lógica de centrado del mapa es el núcleo testeable
/// <see cref="ScriptUbicacionDispositivo"/>.
/// </summary>
public static class UbicacionDispositivo
{
    public static async Task<CoordenadaElegida?> ObtenerAsync()
    {
        try
        {
            var estado = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (estado != PermissionStatus.Granted)
            {
                estado = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (estado != PermissionStatus.Granted)
            {
                return null;
            }

            var ubicacion = await Geolocation.Default.GetLocationAsync(
                new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)));
            // Si no hay lectura fresca, se intenta la última conocida (rápida en terreno).
            ubicacion ??= await Geolocation.Default.GetLastKnownLocationAsync();

            return ubicacion is null
                ? null
                : new CoordenadaElegida((decimal)ubicacion.Latitude, (decimal)ubicacion.Longitude);
        }
        catch
        {
            return null;
        }
    }
}
