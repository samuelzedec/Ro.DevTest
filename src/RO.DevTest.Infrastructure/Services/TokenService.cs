using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Settings;
using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Infrastructure.Services;

public class TokenService(IOptions<JwtSettings> jwtSettings, IUserTokenRepository repository) 
    : ITokenService
{
    public string GenerateAccessToken(User user)
    {
        List<Claim> claims = [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!)
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

    public async Task<bool> ValidationRefreshTokenAsync(User user)
    {
        var refreshToken = await repository.GetRefreshTokenByUserIdAsync(user.Id);
        if (refreshToken?.ExpiresAt <= DateTime.UtcNow)
            return false;

        return true;
    }

    public async Task<string> CreateRefreshTokenAsync(User user)
    {
        var refreshTokenExisting = await repository.GetRefreshTokenByUserIdAsync(user.Id);
        var token = Guid.NewGuid().ToString();
        
        if (refreshTokenExisting is not null)
        {
            refreshTokenExisting.ExpiresAt = DateTime.UtcNow.AddDays(7);
            refreshTokenExisting.Value = token;
            await repository.UpdateRefreshTokenAsync(refreshTokenExisting);
        }
        else
        {
            var newRefreshToken = new UserToken
            {
                UserId = user.Id,
                LoginProvider = "ITokenService",
                Name = "RefreshToken",
                Value = token,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
            await repository.CreateRefreshTokenAsync(newRefreshToken);
        }

        return token;
    }
}