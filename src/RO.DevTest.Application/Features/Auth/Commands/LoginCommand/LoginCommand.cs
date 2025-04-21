using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Auth.Commands.LoginCommand;

public record LoginCommand(string UsernameOrEmail, string Password) : IRequest<Result<LoginResponse>>;