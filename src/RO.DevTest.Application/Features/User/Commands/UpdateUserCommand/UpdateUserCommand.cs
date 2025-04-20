using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.User.Commands.UpdateUserCommand;

public record UpdateUserCommand(
    string UserName,
    string Name,
    string Email
) : IRequest<Result<UpdateUserResponse>>;