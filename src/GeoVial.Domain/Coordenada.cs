namespace GeoVial.Domain;

/// <summary>
/// Objeto de valor de una coordenada geográfica (latitud, longitud en grados decimales).
/// El cálculo de distancia se hace en dominio, sin tipo espacial nativo (modelo-datos-logico §2),
/// con la fórmula de haversine, para evaluar la agrupación por radio (RN-02).
/// </summary>
public readonly record struct Coordenada(decimal Latitud, decimal Longitud)
{
    private const double RadioTierraMetros = 6_371_000d;

    /// <summary>Distancia en metros a otra coordenada (haversine).</summary>
    public double DistanciaMetrosA(Coordenada otra)
    {
        var lat1 = GradosARadianes((double)Latitud);
        var lat2 = GradosARadianes((double)otra.Latitud);
        var dLat = GradosARadianes((double)(otra.Latitud - Latitud));
        var dLon = GradosARadianes((double)(otra.Longitud - Longitud));

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return RadioTierraMetros * c;
    }

    private static double GradosARadianes(double grados) => grados * Math.PI / 180d;
}
