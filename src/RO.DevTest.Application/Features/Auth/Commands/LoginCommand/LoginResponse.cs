using System.Text.Json.Serialization;

namespace RO.DevTest.Application.Features.Auth.Commands.LoginCommand;

public record LoginResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AccessToken { get; set; } = null;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RefreshToken { get; set; } = null;

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpirationDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Role { get; set; } = null;
    
    public LoginResponse(){}
    
    public LoginResponse(string? accessToken, string? refreshToken, DateTime expirationDate, string? role)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpirationDate = expirationDate;
        Role = role;
    }
}