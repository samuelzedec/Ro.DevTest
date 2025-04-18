namespace RO.DevTest.Application.Features.User.Commands.CreateUserCommand;

public record CreateUserResult {
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public CreateUserResult () { }

    public CreateUserResult(Domain.Entities.Identity.User user, string role) { 
        Id = user.Id;
        UserName = user.UserName!;
        Email = user.Email!;
        Name = user.Name;
        Role = role;
    }
}
