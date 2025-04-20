using System.Text.Json.Serialization;
using MediatR;
using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Application.Features.User.Commands.CreateUserCommand;

public record CreateUserCommand(
    string UserName,
    string Name,
    string Email,
    string Password,
    string PasswordConfirmation
) : IRequest<Result<CreateUserResponse>>
{
    [JsonIgnore] public UserRoles Role { get; set; }

    public Domain.Entities.Identity.User AssignTo() => new()
    {
        UserName = UserName,
        Email = Email,
        Name = Name,
    };
}