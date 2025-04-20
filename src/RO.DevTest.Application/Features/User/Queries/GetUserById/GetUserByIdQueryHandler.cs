using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.User.Queries.GetUserById;

public class GetUserByIdQueryHandler(
    IIdentityAbstractor identityAbstractor,
    ICurrentUserService currentUserService,
    ILogger<GetUserByIdQueryHandler> logger)
    : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdResponse>>
{
    public async Task<Result<GetUserByIdResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await identityAbstractor.FindUserByIdAsync(currentUserService.GetCurrentUserId());
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