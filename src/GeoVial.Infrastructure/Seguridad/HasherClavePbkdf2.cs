using System.Security.Cryptography;
using GeoVial.Application.Abstracciones;

namespace GeoVial.Infrastructure.Seguridad;

/// <summary>
/// Hash de claves con PBKDF2 (SHA-256, 100.000 iteraciones). Formato persistido: "iteraciones.saltB64.hashB64".
/// </summary>
public sealed class HasherClavePbkdf2 : IHasherClave
{
    private const int Iteraciones = 100_000;
    private const int TamanioSalt = 16;
    private const int TamanioHash = 32;

    public string Hash(string clave)
    {
        var salt = RandomNumberGenerator.GetBytes(TamanioSalt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(clave, salt, Iteraciones, HashAlgorithmName.SHA256, TamanioHash);
        return $"{Iteraciones}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verificar(string clave, string hashAlmacenado)
    {
        var partes = hashAlmacenado.Split('.', 3);
        if (partes.Length != 3 || !int.TryParse(partes[0], out var iteraciones))
        {
            return false;
        }

        var salt = Convert.FromBase64String(partes[1]);
        var esperado = Convert.FromBase64String(partes[2]);
        var calculado = Rfc2898DeriveBytes.Pbkdf2(clave, salt, iteraciones, HashAlgorithmName.SHA256, esperado.Length);
        return CryptographicOperations.FixedTimeEquals(calculado, esperado);
    }
}
