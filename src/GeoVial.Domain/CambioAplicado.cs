namespace GeoVial.Domain;

/// <summary>
/// Registro de un cambio de sincronización ya aplicado (RC-03, CU-07). Garantiza la idempotencia: un
/// reintento con el mismo <see cref="CambioId"/> no vuelve a aplicar el cambio, lo que permite reanudar
/// una sincronización interrumpida sin duplicar.
/// </summary>
public sealed class CambioAplicado
{
    public Guid CambioId { get; private set; }
    public Guid RelevamientoId { get; private set; }
    public DateTime AplicadoUtc { get; private set; }

    // ctor para materialización del ORM
    private CambioAplicado()
    {
    }

    public CambioAplicado(Guid cambioId, Guid relevamientoId, DateTime aplicadoUtc)
    {
        CambioId = cambioId;
        RelevamientoId = relevamientoId;
        AplicadoUtc = aplicadoUtc;
    }
}
