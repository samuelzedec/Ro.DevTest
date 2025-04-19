namespace RO.DevTest.Application.Features.User.Commands.UpdateUserCommand;

public record UpdateUserResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    
    public UpdateUserResponse(Domain.Entities.Identity.User user) { 
        Id = user.Id;
        UserName = user.UserName!;
        Email = user.Email!;
        Name = user.Name;
    }
}