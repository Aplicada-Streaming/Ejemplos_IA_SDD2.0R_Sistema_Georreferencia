using GeoVial.Sync;

namespace GeoVial.Mobile;

/// <summary>
/// Persiste el token de sesión en el almacén seguro del dispositivo (SecureStorage, respaldado por el Keystore)
/// para que la sesión sobreviva a que el SO mate el proceso —p. ej. al abrir la cámara, que en equipos con poca
/// RAM recrea la app— (S54). Sin esto, al volver la app arrancaba sin sesión y rebotaba al login.
/// </summary>
public sealed class AlmacenTokenSecureStorage : IAlmacenTokenSesion
{
    private const string ClaveToken = "geovial.sesion.token";
    private const string ClaveUsuario = "geovial.sesion.usuario";

    public async Task GuardarAsync(string token, string usuario)
    {
        await SecureStorage.Default.SetAsync(ClaveToken, token);
        await SecureStorage.Default.SetAsync(ClaveUsuario, usuario);
    }

    public async Task<(string Token, string Usuario)?> LeerAsync()
    {
        var token = await SecureStorage.Default.GetAsync(ClaveToken);
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        var usuario = await SecureStorage.Default.GetAsync(ClaveUsuario) ?? "";
        return (token, usuario);
    }

    public Task LimpiarAsync()
    {
        SecureStorage.Default.Remove(ClaveToken);
        SecureStorage.Default.Remove(ClaveUsuario);
        return Task.CompletedTask;
    }
}
