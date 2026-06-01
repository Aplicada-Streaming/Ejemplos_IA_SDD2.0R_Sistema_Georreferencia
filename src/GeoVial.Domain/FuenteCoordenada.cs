namespace GeoVial.Domain;

/// <summary>
/// Origen de la coordenada de una foto (modelo-datos-logico §1.7, RN-03). 1=metadatos de la foto
/// (fuente primaria), 2=ubicación manual sobre el mapa (respaldo).
/// </summary>
public enum FuenteCoordenada
{
    Metadatos = 1,
    Manual = 2,
}
