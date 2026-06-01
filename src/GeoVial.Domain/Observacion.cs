namespace GeoVial.Domain;

/// <summary>
/// Observación: registro de un punto del relevamiento (modelo-datos-logico §1.5). Si tiene coordenada
/// queda asociada a un marcador; si no, va a la bandeja sin georreferenciar del relevamiento (RC-02, RN-03).
/// </summary>
public sealed class Observacion
{
    public Guid ObservacionId { get; private set; }
    public Guid RelevamientoId { get; private set; }
    public Guid? MarcadorId { get; private set; }
    public Guid AgenteUsuarioId { get; private set; }
    public DateTime MomentoCaptura { get; private set; }
    public bool SinGeorreferenciar { get; private set; }

    // ctor para materialización del ORM
    private Observacion()
    {
    }

    private Observacion(Guid relevamientoId, Guid agenteUsuarioId, DateTime momentoCaptura, Guid? marcadorId, bool sinGeorreferenciar)
    {
        ObservacionId = Guid.NewGuid();
        RelevamientoId = relevamientoId;
        AgenteUsuarioId = agenteUsuarioId;
        MomentoCaptura = momentoCaptura;
        MarcadorId = marcadorId;
        SinGeorreferenciar = sinGeorreferenciar;
    }

    public static Observacion Georreferenciada(Guid relevamientoId, Guid agenteUsuarioId, DateTime momentoCaptura, Guid marcadorId) =>
        new(relevamientoId, agenteUsuarioId, momentoCaptura, marcadorId, sinGeorreferenciar: false);

    public static Observacion EnBandejaSinGeorreferenciar(Guid relevamientoId, Guid agenteUsuarioId, DateTime momentoCaptura) =>
        new(relevamientoId, agenteUsuarioId, momentoCaptura, marcadorId: null, sinGeorreferenciar: true);

    /// <summary>Asocia la observación a un marcador y la saca de la bandeja sin georreferenciar (RC-02).</summary>
    public void AsociarMarcador(Guid marcadorId)
    {
        MarcadorId = marcadorId;
        SinGeorreferenciar = false;
    }
}
