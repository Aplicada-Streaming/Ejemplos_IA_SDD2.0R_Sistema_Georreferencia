namespace GeoVial.Sync;

/// <summary>Qué debe hacer la app al arrancar / reabrir, según el estado de sesión y el método del teléfono (S55).</summary>
public enum DecisionArranque
{
    /// <summary>Entrar directo a las solapas (sesión restaurada).</summary>
    Entrar,

    /// <summary>Pedir el método de seguridad del teléfono (patrón/huella/PIN) para desbloquear la sesión.</summary>
    PedirBiometrico,

    /// <summary>Pedir usuario y clave (no hay sesión persistida o no hay método del dispositivo).</summary>
    PedirClave,

    /// <summary>La verificación se canceló o falló: pantalla de bloqueo con opción de usar usuario y clave.</summary>
    Bloqueado,
}

/// <summary>
/// Decisión de arranque/relogueo del móvil (S55), como funciones puras para poder cubrirlas en el gate (la
/// pantalla sólo ejecuta lo que esto decide). Modelo según práctica de industria: al reabrir la app con una
/// sesión persistida se pide el método de seguridad del teléfono para volver; "cerrar sesión" es logout total
/// (vuelve a usuario/clave); y la vuelta de la cámara —que recreó el proceso— no re-pide el método.
/// </summary>
public static class PoliticaArranque
{
    /// <summary>Decide el arranque inicial a partir del estado persistido y si se está volviendo de la cámara.</summary>
    public static DecisionArranque Decidir(bool hayTokenPersistido, bool hayMetodoDispositivo, bool volviendoDeCamara)
    {
        if (!hayTokenPersistido)
        {
            return DecisionArranque.PedirClave;
        }

        if (volviendoDeCamara)
        {
            // El SO recreó el proceso por la cámara (acción propia): se restaura sin re-pedir el método.
            return DecisionArranque.Entrar;
        }

        return hayMetodoDispositivo ? DecisionArranque.PedirBiometrico : DecisionArranque.PedirClave;
    }

    /// <summary>Traduce el resultado de la verificación con el método del teléfono a la acción siguiente.</summary>
    public static DecisionArranque TrasBiometrico(ResultadoBiometrico resultado) => resultado switch
    {
        ResultadoBiometrico.Exito => DecisionArranque.Entrar,
        ResultadoBiometrico.NoDisponible => DecisionArranque.PedirClave,
        _ => DecisionArranque.Bloqueado, // Fallo o Cancelado: no se entra; se ofrece usuario y clave
    };
}
