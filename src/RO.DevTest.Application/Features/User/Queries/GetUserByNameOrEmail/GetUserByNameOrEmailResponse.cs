namespace RO.DevTest.Application.Features.User.Queries.GetUserByNameOrEmail;

public record GetUserByNameOrEmailResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    
    public GetUserByNameOrEmailResponse(Domain.Entities.Identity.User user) { 
        Id = user.Id;
        UserName = user.UserName!;
        Email = user.Email!;
        Name = user.Name;
        Role = user.Roles.FirstOrDefault()!;
    }
}