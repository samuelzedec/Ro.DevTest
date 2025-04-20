using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Auth.Commands.RefreshTokenCommand;

public record RefreshTokenCommand(Guid UserId, string RefreshToken ) : IRequest<Result<RefreshTokenResponse>>;