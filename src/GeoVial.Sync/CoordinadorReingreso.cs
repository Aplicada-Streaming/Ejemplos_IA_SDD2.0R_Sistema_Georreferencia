namespace GeoVial.Sync;

/// <summary>Resultado de pedirle al teléfono que verifique la identidad con su método nativo (RN-06, S53).</summary>
public enum ResultadoBiometrico
{
    /// <summary>El usuario se verificó con huella/rostro/PIN del teléfono.</summary>
    Exito,

    /// <summary>El método nativo rechazó la verificación (no coincide).</summary>
    Fallo,

    /// <summary>El usuario canceló el pedido de verificación.</summary>
    Cancelado,

    /// <summary>El teléfono no tiene un método de seguridad disponible (sin huella/PIN o hardware).</summary>
    NoDisponible,
}

/// <summary>
/// Verificación de identidad con el método de seguridad nativo del teléfono (huella/rostro/PIN). La
/// implementación real es de plataforma (Android <c>BiometricPrompt</c>); esta abstracción permite que el
/// coordinador de reingreso (en el gate) decida sin depender del SO.
/// </summary>
public interface IAutenticadorBiometrico
{
    /// <summary>Indica si el teléfono tiene un método de seguridad configurado y utilizable.</summary>
    Task<bool> HayMetodoDisponibleAsync();

    /// <summary>Pide al teléfono que verifique al usuario mostrando el motivo; devuelve el resultado.</summary>
    Task<ResultadoBiometrico> AutenticarAsync(string motivo, CancellationToken ct = default);
}

/// <summary>
/// Coordina el reingreso en terreno con verificación biométrica nativa (RN-06, S53): antes de re-autenticar
/// sin clave, exige que el teléfono verifique la identidad con su método de seguridad. Sólo si la verificación
/// tiene éxito se hace el reingreso contra el backend (que el método está presente lo informa esta verificación,
/// no un marcador blando). Núcleo testeable: el biométrico se inyecta como <see cref="IAutenticadorBiometrico"/>.
/// </summary>
public sealed class CoordinadorReingreso
{
    private readonly ServicioSesion _sesion;
    private readonly IAutenticadorBiometrico _biometrico;

    public CoordinadorReingreso(ServicioSesion sesion, IAutenticadorBiometrico biometrico)
    {
        _sesion = sesion;
        _biometrico = biometrico;
    }

    /// <summary>Indica si el teléfono tiene método de seguridad (para ofrecer/condicionar el reingreso y el offline).</summary>
    public Task<bool> HayMetodoDisponibleAsync() => _biometrico.HayMetodoDisponibleAsync();

    /// <summary>
    /// Reingresa en terreno tras verificar la identidad con el método nativo del teléfono. No lanza:
    /// devuelve un <see cref="ResultadoSesion"/> con un mensaje apto para la UI si la verificación falla,
    /// se cancela, no hay método, o no hay usuario recordado.
    /// </summary>
    public async Task<ResultadoSesion> ReingresarAsync(string? usuario, CancellationToken ct = default)
    {
        var u = usuario?.Trim() ?? "";
        if (u.Length == 0)
        {
            return ResultadoSesion.Fallo("No hay un usuario recordado para el reingreso.");
        }

        var verificacion = await _biometrico.AutenticarAsync($"Reingreso en terreno de {u}", ct);
        if (verificacion != ResultadoBiometrico.Exito)
        {
            return ResultadoSesion.Fallo(MensajePara(verificacion));
        }

        // La verificación nativa exitosa ES el método de seguridad presente (RN-06): se reingresa sin clave.
        return await _sesion.ReingresarAsync(u, metodoPresente: true);
    }

    private static string MensajePara(ResultadoBiometrico resultado) => resultado switch
    {
        ResultadoBiometrico.NoDisponible => "Este teléfono no tiene huella, rostro o PIN configurado; reingresá con usuario y clave.",
        ResultadoBiometrico.Cancelado => "Reingreso cancelado.",
        _ => "No se pudo verificar tu identidad. Probá de nuevo o ingresá con usuario y clave.",
    };
}
