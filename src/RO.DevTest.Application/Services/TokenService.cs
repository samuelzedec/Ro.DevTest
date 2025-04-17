using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Settings;
using RO.DevTest.Domain.Entities.Identity;
using RO.DevTest.Domain.Services;

namespace RO.DevTest.Application.Services;

public class TokenService(IOptions<JwtSettings> jwtSettings) 
    : ITokenService
{
    public string GenerateToken(User user)
    {
        List<Claim> claims = [
            new(ClaimTypes.NameIdentifier, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.Name)
        ];
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            Issuer = jwtSettings.Value.Issuer,
            Audience = jwtSettings.Value.Audience,
            SigningCredentials = new SigningCredentials(
                new X509SecurityKey(jwtSettings.Value.GenerateCertificate()),
                SecurityAlgorithms.RsaSha256Signature)
        };

        return tokenHandler.WriteToken(
            tokenHandler.CreateToken(tokenDescriptor));
    }

    public string GenerateRefreshToken()
        => Guid.NewGuid().ToString();
}