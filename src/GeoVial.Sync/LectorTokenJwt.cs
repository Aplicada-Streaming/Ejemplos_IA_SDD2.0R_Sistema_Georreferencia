using System.Text;
using System.Text.Json;

namespace GeoVial.Sync;

/// <summary>
/// Lee el identificador de usuario (claim <c>sub</c>) de un JWT del backend, sin validar la firma
/// (el cliente sólo necesita saber quién es para marcar los relevamientos asignados; la validación
/// la hace el backend). Decodifica el payload base64url y extrae <c>sub</c> como GUID.
/// </summary>
public static class LectorTokenJwt
{
    public static Guid? LeerUsuarioId(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var partes = token.Split('.');
        if (partes.Length < 2)
        {
            return null;
        }

        try
        {
            var json = DecodificarBase64Url(partes[1]);
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("sub", out var sub) && Guid.TryParse(sub.GetString(), out var id))
            {
                return id;
            }
        }
        catch
        {
            // Token mal formado: el cliente sigue sin conocer el usuario (no marca asignados); no rompe.
        }

        return null;
    }

    private static string DecodificarBase64Url(string s)
    {
        s = s.Replace('-', '+').Replace('_', '/');
        s += (s.Length % 4) switch
        {
            2 => "==",
            3 => "=",
            _ => "",
        };
        return Encoding.UTF8.GetString(Convert.FromBase64String(s));
    }
}
