using GeoVial.Shared;

namespace GeoVial.CapturaCampo;

/// <summary>
/// Arma la petición de ubicación manual del punto de una observación sin georreferenciar (US-13, CU-05). Valida
/// que la coordenada esté en rango geográfico (latitud −90..90, longitud −180..180, RN-03); si está fuera de
/// rango devuelve null y la observación permanece en la bandeja sin georreferenciar.
/// </summary>
public sealed class ArmadorUbicacionManual
{
    public UbicarManualRequest? Armar(decimal latitud, decimal longitud)
    {
        if (latitud is < -90m or > 90m || longitud is < -180m or > 180m)
        {
            return null;
        }

        return new UbicarManualRequest(latitud, longitud);
    }
}
