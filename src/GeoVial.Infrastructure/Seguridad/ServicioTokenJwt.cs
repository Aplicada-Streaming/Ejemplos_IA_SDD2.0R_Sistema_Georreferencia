using System.Security.Claims;
using System.Text;
using GeoVial.Application.Abstracciones;
using GeoVial.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace GeoVial.Infrastructure.Seguridad;

/// <summary>
/// Emisor de access token JWT con claims sub, role y area (ADR-03, README §8). El refresh token es opaco.
/// </summary>
public sealed class ServicioTokenJwt : IServicioToken
{
    private readonly JwtOptions _opciones;
    private readonly IRelojUtc _reloj;

    public ServicioTokenJwt(IOptions<JwtOptions> opciones, IRelojUtc reloj)
    {
        _opciones = opciones.Value;
        _reloj = reloj;
    }

    public string GenerarAccessToken(Usuario usuario, out int expiraEnSegundos)
    {
        var minutos = _opciones.MinutosAcceso;
        expiraEnSegundos = minutos * 60;

        var claims = new List<Claim>
        {
            new("sub", usuario.UsuarioId.ToString()),
            new("role", ((int)usuario.Rol).ToString()),
        };
        if (usuario.AreaId is not null)
        {
            claims.Add(new Claim("area", usuario.AreaId.Value.ToString()));
        }

        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opciones.ClaveSecreta));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);
        var ahora = _reloj.AhoraUtc;

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _opciones.Issuer,
            Audience = _opciones.Audience,
            Subject = new ClaimsIdentity(claims),
            IssuedAt = ahora,
            NotBefore = ahora,
            Expires = ahora.AddMinutes(minutos),
            SigningCredentials = credenciales,
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    public string GenerarRefreshToken() =>
        Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
}
