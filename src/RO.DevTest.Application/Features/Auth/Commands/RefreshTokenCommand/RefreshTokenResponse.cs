namespace RO.DevTest.Application.Features.Auth.Commands.RefreshTokenCommand;

public record RefreshTokenResponse
{
    public string AccessToken { get; init; }
    public string RefreshTokem { get; init; }
    public DateTime ExpiresAt { get; init; }
    
    public RefreshTokenResponse(string accessToken, string refreshTokem, DateTime expiresAt)
    {
        AccessToken = accessToken;
        RefreshTokem = refreshTokem;
        ExpiresAt = expiresAt;
    }
}