namespace GeoVial.Domain;

/// <summary>
/// Agrupación de observaciones por radio (RN-02). Una coordenada cae en un marcador existente si la
/// distancia es menor que el radio de agrupación del relevamiento; nunca se unifican ni descartan
/// marcadores de forma automática.
/// </summary>
public static class AgrupacionMarcador
{
    /// <summary>Devuelve el primer marcador existente dentro del radio, o null si la coordenada inicia un punto nuevo.</summary>
    public static Marcador? MarcadorEnRadio(IEnumerable<Marcador> existentes, Coordenada coordenada, decimal radioMetros) =>
        existentes.FirstOrDefault(m => m.Coordenada.DistanciaMetrosA(coordenada) < (double)radioMetros);
}
