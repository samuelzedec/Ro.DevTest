using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.User.Commands.UpdateUserCommand;

public class UpdateUserCommandHandler(
    IIdentityAbstractor identityAbstractor, 
    IValidator<UpdateUserCommand> validator,
    ILogger<UpdateUserCommandHandler> logger)
    : IRequestHandler<UpdateUserCommand, Result<UpdateUserResponse>>
{
    public async Task<Result<UpdateUserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<UpdateUserResponse>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            var user = await identityAbstractor.FindUserByIdAsync(request.Id.ToString());
            if (user is null)
                return Result<UpdateUserResponse>.Failure(messages: "User not found");

            if (await ValitationRequestAsync(request))
                return Result<UpdateUserResponse>.Failure(messages: "Existing information");

            user.UserName = string.IsNullOrWhiteSpace(request.UserName) ? user.UserName : request.UserName;
            user.Name = string.IsNullOrWhiteSpace(request.Name) ? user.Name : request.Name;
            user.Email = string.IsNullOrWhiteSpace(request.Email) ? user.Email : request.Email;

            await identityAbstractor.UpdateUserAsync(user);
            return Result<UpdateUserResponse>.Success(new UpdateUserResponse(user),
                messages: "Data updated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<UpdateUserResponse>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    private async Task<bool> ValitationRequestAsync(UpdateUserCommand request)
    {
        var userNameResult = await identityAbstractor.FindUserByNameAsync(request.UserName);
        if (userNameResult is not null) return true;

        var userEmailResult = await identityAbstractor.FindUserByEmailAsync(request.Email);
        if (userEmailResult is not null) return true;

        return false;
    }
} 