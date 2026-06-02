namespace GeoVial.Domain;

/// <summary>
/// Finalidad declarada al acceder a datos personales (RN-08, Ley 25.326). El tratamiento se limita a
/// los fines del relevamiento y su evaluación; cualquier otra finalidad se bloquea (FINALIDAD_NO_PERMITIDA).
/// </summary>
public enum FinalidadTratamiento
{
    Relevamiento = 1,
    Evaluacion = 2,
}

public static class Finalidades
{
    /// <summary>Indica si una finalidad declarada (texto) corresponde a un fin permitido por RN-08.</summary>
    public static bool EsPermitida(string? finalidad) =>
        Enum.TryParse<FinalidadTratamiento>(finalidad, ignoreCase: true, out _);
}
