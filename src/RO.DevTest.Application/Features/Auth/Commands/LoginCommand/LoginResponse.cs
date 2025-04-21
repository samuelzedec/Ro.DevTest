using System.Text.Json.Serialization;

namespace RO.DevTest.Application.Features.Auth.Commands.LoginCommand;

public record LoginResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AccessToken { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RefreshToken { get; init; }

    public DateTime IssuedAt { get; init; } = DateTime.UtcNow;
    public DateTime ExpirationDate { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Role { get; init; }
    
    public LoginResponse(string? accessToken, string? refreshToken, DateTime expirationDate, string? role)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpirationDate = expirationDate;
        Role = role;
    }
}