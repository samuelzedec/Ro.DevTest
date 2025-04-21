namespace RO.DevTest.Application.Features.User.Commands.CreateUserCommand;

public record CreateUserResponse {
    public Guid Id { get; init; }
    public string UserName { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public string Role { get; init; }
    
    public CreateUserResponse(Domain.Entities.Identity.User user, string role) { 
        Id = user.Id;
        UserName = user.UserName!;
        Email = user.Email!;
        Name = user.Name;
        Role = role;
    }
}
