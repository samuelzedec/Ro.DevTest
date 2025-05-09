using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Auth.Commands.RefreshTokenCommand;

public class RefreshTokenCommandHandler(
    IIdentityAbstractor identityAbstractor,
    ITokenService tokenService,
    IValidator<RefreshTokenCommand> validator,
    ILogger<RefreshTokenCommandHandler> logger)
    : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    public async Task<Result<RefreshTokenResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<RefreshTokenResponse>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            var user = await identityAbstractor.FindUserByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                return Result<RefreshTokenResponse>.Failure(StatusCodes.Status404NotFound,
                    messages: "Usuário não encontrado.");
            }

            if (await tokenService.ValidationRefreshTokenAsync(user, request.RefreshToken))
            {
                var token = tokenService.GenerateAccessToken(user);
                var refreshToken = await tokenService.CreateRefreshTokenAsync(user);
                return Result<RefreshTokenResponse>.Success(new RefreshTokenResponse(
                    token,
                    refreshToken,
                    DateTime.UtcNow.AddMinutes(15)), 
                    messages: "Novos tokens de autenticação.");
            }

            return Result<RefreshTokenResponse>.Failure(messages: "Informações inválidas.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<RefreshTokenResponse>.Failure(StatusCodes.Status500InternalServerError, 
                messages: "Ocorreu um erro inesperado, consulte o arquivo de hoje na pasta Logs");
        }
    }
}
