namespace RO.DevTest.Application.Features.User.Queries.GetUserByIdQuery;

public record GetUserByIdResponse
{
    public Guid Id { get; init; }
    public string UserName { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public string Role { get; init; }
    
    public GetUserByIdResponse(Domain.Entities.Identity.User user) { 
        Id = user.Id;
        UserName = user.UserName!;
        Email = user.Email!;
        Name = user.Name;
        Role = user.Roles.FirstOrDefault()!;
    }
}