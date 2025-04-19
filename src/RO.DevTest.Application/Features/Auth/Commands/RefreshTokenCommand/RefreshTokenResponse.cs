namespace RO.DevTest.Application.Features.Auth.Commands.RefreshTokenCommand;

public record RefreshTokenResponse
{
    public string AccessToken { get; set; }
    public string RefreshTokem { get; set; }
    public DateTime ExpiresAt { get; set; }
    
    public RefreshTokenResponse(string accessToken, string refreshTokem, DateTime expiresAt)
    {
        AccessToken = accessToken;
        RefreshTokem = refreshTokem;
        ExpiresAt = expiresAt;
    }
}