using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Services;

namespace RO.DevTest.Application.Features.Auth.Commands.LoginCommand;

public class LoginCommandHandler(
    IIdentityAbstractor identityAbstractor, 
    ITokenService tokenService,
    IValidator<LoginCommand> validator,
    ILogger<LoginCommandHandler> logger)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<LoginResponse>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }
            
            var user = await identityAbstractor.FindUserByEmailAsync(request.UsernameOrEmail)
                       ?? await identityAbstractor.FindUserByNameAsync(request.UsernameOrEmail);

            if (user is null)
                return Result<LoginResponse>.Failure(StatusCodes.Status404NotFound, messages: "User not found");
            
            var validationPassword = await identityAbstractor.PasswordSignInAsync(user, request.Password);
            if (!validationPassword.Succeeded)
                return Result<LoginResponse>.Failure(messages: "Invalid email/username or password");

            var role = await identityAbstractor.GetUserRolesAsync(user);
            var token = tokenService.GenerateAccessToken(user);
            var refreshToken = await tokenService.CreateRefreshTokenAsync(user);

            var response = new LoginResponse(
                token,
                refreshToken,
                DateTime.UtcNow.AddMinutes(15),
                role.ToList().FirstOrDefault());
            
            return Result<LoginResponse>.Success(response, messages: "Login successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<LoginResponse>.Failure(StatusCodes.Status500InternalServerError, messages: ex.Message);
        }
    }
}