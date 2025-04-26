using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.User.Queries.GetUserByNameOrEmailQuery;

public class GetUserByNameOrEmailQueryHandler(
    IIdentityAbstractor identityAbstractor, 
    IValidator<GetUserByNameOrEmailQuery> validator,
    ILogger<GetUserByNameOrEmailQueryHandler> logger)
    : IRequestHandler<GetUserByNameOrEmailQuery, Result<GetUserByNameOrEmailResponse>>
{
    public async Task<Result<GetUserByNameOrEmailResponse>> Handle(GetUserByNameOrEmailQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<GetUserByNameOrEmailResponse>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            var user = await identityAbstractor.FindUserByNameAsync(request.NameOrEmail)
                       ?? await identityAbstractor.FindUserByEmailAsync(request.NameOrEmail);
            
            if (user is null)
                return Result<GetUserByNameOrEmailResponse>.Failure(StatusCodes.Status404NotFound, "Usuário não encontrado");

            var role = await identityAbstractor.GetUserRolesAsync(user);
            user.Roles = role.ToList();

            return Result<GetUserByNameOrEmailResponse>.Success(new GetUserByNameOrEmailResponse(user), messages: "Usuário encontrado");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<GetUserByNameOrEmailResponse>.Failure(StatusCodes.Status500InternalServerError, 
                "Ocorreu um erro inesperado, consulte o arquivo de hoje na pasta Logs");
        }
    }
}