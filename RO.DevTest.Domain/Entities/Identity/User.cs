using Microsoft.AspNetCore.Identity;

namespace RO.DevTest.Domain.Entities.Identity;

/// <summary>
/// Represents a <see cref="IdentityUser"/> int the API
/// </summary>
public class User : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;
}
