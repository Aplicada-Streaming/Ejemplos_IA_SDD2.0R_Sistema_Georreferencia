namespace GeoVial.Domain;

/// <summary>
/// Estado del relevamiento (modelo-datos-logico §1.3, RC-06). 1=recolección, 2=revisión, 3=cerrado.
/// Transiciones válidas (RN-05): recolección → revisión → cierre y, explícitamente, cerrado → recolección.
/// </summary>
public enum EstadoRelevamiento
{
    Recoleccion = 1,
    Revision = 2,
    Cerrado = 3,
}
