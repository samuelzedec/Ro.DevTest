using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.User.Queries.GetUserById;

public class GetUserByIdQueryHandler(
    IIdentityAbstractor identityAbstractor, 
    IValidator<GetUserByIdQuery> validator,
    ILogger<GetUserByIdQueryHandler> logger)
    : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdResponse>>
{
    public async Task<Result<GetUserByIdResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<GetUserByIdResponse>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }
            
            var user = await identityAbstractor.FindUserByIdAsync(request.Id.ToString());
            if (user is null)
                return Result<GetUserByIdResponse>.Failure(StatusCodes.Status404NotFound, "User not found");

            var role = await identityAbstractor.GetUserRolesAsync(user);
            user.Roles = role.ToList();

            return Result<GetUserByIdResponse>.Success(new GetUserByIdResponse(user), messages: "User found");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<GetUserByIdResponse>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}