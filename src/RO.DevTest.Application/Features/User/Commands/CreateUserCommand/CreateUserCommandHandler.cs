using MediatR;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Entities.Identity;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Application.Features.User.Commands.CreateUserCommand;

/// <summary>
/// Command handler for the creation of <see cref="User"/>
/// </summary>
public class CreateUserCommandHandler(IIdentityAbstractor identityAbstractor, ILogger<CreateUserCommandHandler> logger)
    : IRequestHandler<CreateUserCommand, Result<CreateUserResult>>
{
    public async Task<Result<CreateUserResult>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validator = new CreateUserCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<CreateUserResult>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            Domain.Entities.Identity.User newUser = request.AssignTo();
            var userCreationResult = await identityAbstractor.CreateUserAsync(newUser, request.Password);
            if (!userCreationResult.Succeeded)
            {
                return Result<CreateUserResult>.Failure(messages:
                    userCreationResult.Errors.Select(e => e.Description).ToArray());
            }

            var userRoleResult = await identityAbstractor.AddToRoleAsync(newUser, request.Role);
            if (!userRoleResult.Succeeded)
            {
                return Result<CreateUserResult>.Failure(messages:
                    userRoleResult.Errors.Select(e => e.Description).ToArray());
            }

            var role = await identityAbstractor.GetUserRolesAsync(newUser);
            return Result<CreateUserResult>.Success(
                new CreateUserResult(newUser, role.ToList().FirstOrDefault()!), 201);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<CreateUserResult>.Failure(messages: ex.Message);
        }
    }
}