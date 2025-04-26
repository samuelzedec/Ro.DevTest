using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Application.Features.User.Commands.CreateUserCommand;

/// <summary>
/// Command handler for the creation of <see cref="User"/>
/// </summary>
public class CreateUserCommandHandler(
    IIdentityAbstractor identityAbstractor, 
    IValidator<CreateUserCommand> validator,
    ILogger<CreateUserCommandHandler> logger)
    : IRequestHandler<CreateUserCommand, Result<CreateUserResponse>>
{
    public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<CreateUserResponse>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            Domain.Entities.Identity.User newUser = request.AssignTo();
            var userCreationResult = await identityAbstractor.CreateUserAsync(newUser, request.Password);
            if (!userCreationResult.Succeeded)
            {
                return Result<CreateUserResponse>.Failure(messages:
                    userCreationResult.Errors.Select(e => e.Description).ToArray());
            }

            var userRoleResult = await identityAbstractor.AddToRoleAsync(newUser, request.Role);
            if (!userRoleResult.Succeeded)
            {
                return Result<CreateUserResponse>.Failure(messages:
                    userRoleResult.Errors.Select(e => e.Description).ToArray());
            }

            var role = await identityAbstractor.GetUserRolesAsync(newUser);
            return Result<CreateUserResponse>.Success(
                new CreateUserResponse(newUser, role.ToList().FirstOrDefault()!), StatusCodes.Status201Created, "Usuário criado com sucesso");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<CreateUserResponse>.Failure(StatusCodes.Status500InternalServerError,
                "Ocorreu um erro inesperado, consulte o arquivo de hoje na pasta Logs");
        }
    }
}
