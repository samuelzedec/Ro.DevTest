namespace RO.DevTest.Application.Features.User.Commands.UpdateUserCommand;

public record UpdateUserResponse
{
    public Guid Id { get; init; }
    public string UserName { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    
    public UpdateUserResponse(Domain.Entities.Identity.User user) { 
        Id = user.Id;
        UserName = user.UserName!;
        Email = user.Email!;
        Name = user.Name;
    }
}