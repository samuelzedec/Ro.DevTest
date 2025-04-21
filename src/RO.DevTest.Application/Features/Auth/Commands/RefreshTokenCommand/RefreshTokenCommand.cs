using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Auth.Commands.RefreshTokenCommand;

public class RefreshTokenCommand : IRequest<Result<RefreshTokenResponse>>
{
    public Guid UserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}