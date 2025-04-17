using Microsoft.AspNetCore.Identity;

namespace RO.DevTest.Domain.Entities.Identity;

public class UserToken : IdentityUserToken<Guid>
{
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;
}