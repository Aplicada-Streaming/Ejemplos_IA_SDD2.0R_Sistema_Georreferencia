namespace GeoVial.Mobile;

/// <summary>
/// Método de seguridad del teléfono (RN-06, CU-02): recuerda de forma segura (SecureStorage, respaldado por
/// el Keystore del dispositivo) el usuario y que el método quedó configurado, para habilitar el reingreso en
/// terreno sin reescribir la clave. Guardar/leer en SecureStorage exige, en la práctica, un dispositivo con
/// seguridad configurada; eso representa "el método de seguridad del teléfono". El reingreso real contra el
/// backend lo hace <see cref="GeoVial.Sync.ServicioSesion.ReingresarAsync"/>.
/// </summary>
public sealed class SeguridadDispositivo
{
    private const string ClaveUsuario = "geovial.reingreso.usuario";
    private const string ClaveMetodo = "geovial.metodo.configurado";

    /// <summary>Marca el método de seguridad como configurado y recuerda el usuario para el reingreso.</summary>
    public async Task ConfigurarAsync(string usuario)
    {
        await SecureStorage.Default.SetAsync(ClaveUsuario, usuario);
        await SecureStorage.Default.SetAsync(ClaveMetodo, "1");
    }

    /// <summary>Usuario recordado para reingresar, o <c>null</c> si no hay método configurado.</summary>
    public Task<string?> UsuarioRecordadoAsync() => SecureStorage.Default.GetAsync(ClaveUsuario);

    /// <summary>
    /// El método de seguridad del teléfono está presente: hay un marcador en el almacén seguro del
    /// dispositivo (que sólo es accesible si el teléfono tiene su seguridad configurada).
    /// </summary>
    public async Task<bool> MetodoPresenteAsync() => await SecureStorage.Default.GetAsync(ClaveMetodo) == "1";

    /// <summary>Olvida el método y el usuario (al cerrar sesión explícitamente).</summary>
    public void Olvidar()
    {
        SecureStorage.Default.Remove(ClaveUsuario);
        SecureStorage.Default.Remove(ClaveMetodo);
    }
}
