namespace GeoVial.Sync;

/// <summary>
/// Marca persistente de "captura en curso" (S55): se setea antes de abrir la cámara y se limpia al volver. Si
/// el SO mata el proceso mientras la cámara está en primer plano, esta marca —que sobrevive a la muerte del
/// proceso— permite distinguir "volví de la cámara" (no re-pedir el método) de un arranque en frío. La
/// implementación de plataforma usa <c>Preferences</c> (flag no sensible).
/// </summary>
public interface IMarcadorCaptura
{
    Task MarcarEnCursoAsync();
    Task<bool> EstaEnCursoAsync();
    Task LimpiarAsync();
}

/// <summary>
/// Orquesta la decisión de arranque/relogueo del móvil (S55) usando la política pura
/// <see cref="PoliticaArranque"/> sobre las fuentes reales (sesión persistida, método del dispositivo y marca
/// de cámara), y ejecuta la restauración de sesión cuando corresponde. Núcleo testeable; la pantalla sólo
/// reacciona a la <see cref="DecisionArranque"/> devuelta.
/// </summary>
public sealed class CoordinadorArranque
{
    private readonly ServicioSesion _sesion;
    private readonly IAutenticadorBiometrico _biometrico;
    private readonly IMarcadorCaptura _marcadorCaptura;

    public CoordinadorArranque(ServicioSesion sesion, IAutenticadorBiometrico biometrico, IMarcadorCaptura marcadorCaptura)
    {
        _sesion = sesion;
        _biometrico = biometrico;
        _marcadorCaptura = marcadorCaptura;
    }

    /// <summary>
    /// Decide el arranque inicial. Si se vuelve de la cámara con sesión persistida, restaura sin pedir el método
    /// y limpia la marca. Si hay sesión + método, devuelve <see cref="DecisionArranque.PedirBiometrico"/> (la UI
    /// llamará luego a <see cref="DesbloquearConBiometricoAsync"/>). Si no, pide usuario y clave.
    /// </summary>
    public async Task<DecisionArranque> DecidirInicialAsync()
    {
        var hayToken = await _sesion.HayTokenPersistidoAsync();
        var volviendoDeCamara = await _marcadorCaptura.EstaEnCursoAsync();
        var hayMetodo = await _biometrico.HayMetodoDisponibleAsync();

        var decision = PoliticaArranque.Decidir(hayToken, hayMetodo, volviendoDeCamara);
        if (decision == DecisionArranque.Entrar)
        {
            await _sesion.RestaurarAsync();
            await _marcadorCaptura.LimpiarAsync();
        }

        return decision;
    }

    /// <summary>
    /// Pide el método de seguridad del teléfono y, si verifica, restaura la sesión persistida (offline, sin red).
    /// Devuelve la acción resultante (<see cref="DecisionArranque.Entrar"/> / <c>Bloqueado</c> / <c>PedirClave</c>).
    /// </summary>
    public async Task<DecisionArranque> DesbloquearConBiometricoAsync()
    {
        var verificacion = await _biometrico.AutenticarAsync("Desbloqueá GeoVial para volver a tu sesión");
        var decision = PoliticaArranque.TrasBiometrico(verificacion);
        if (decision == DecisionArranque.Entrar)
        {
            await _sesion.RestaurarAsync();
        }

        return decision;
    }
}
