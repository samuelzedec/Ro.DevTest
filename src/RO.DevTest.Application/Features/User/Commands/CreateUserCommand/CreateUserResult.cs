namespace RO.DevTest.Application.Features.User.Commands.CreateUserCommand;

public record CreateUserResult {
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    
    public CreateUserResult(Domain.Entities.Identity.User user, string role) { 
        Id = user.Id;
        UserName = user.UserName!;
        Email = user.Email!;
        Name = user.Name;
        Role = role;
    }
}
