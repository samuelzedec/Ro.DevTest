using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductsByAdminIdQuery;

public class GetProductsByAdminIdQueryHandler(
    IProductRepository productRepository,
    ICurrentUserService currentUserService,
    IValidator<GetProductsByAdminIdQuery> validator,
    ILogger<GetProductsByAdminIdQueryHandler> logger)
    : IRequestHandler<GetProductsByAdminIdQuery, Result<List<GetProductsByAdminIdResponse>>>
{
    public async Task<Result<List<GetProductsByAdminIdResponse>>> Handle(GetProductsByAdminIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<List<GetProductsByAdminIdResponse>>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }
            
            if (!currentUserService.IsAdmin())
                return Result<List<GetProductsByAdminIdResponse>>.Failure(StatusCodes.Status403Forbidden,
                    "You are not an administrator to have products");

            var query = productRepository.GetQueryable(p =>
                p.AdminId == Guid.Parse(currentUserService.GetCurrentUserId())
                && p.Name.ToLower().Contains(request.SearchTerm.ToLower())); 
            // Usando ToLower() para busca case-insensitive pois não temos acesso ao EF.Functions.ILike do Npgsql
            // Nota: Esta abordagem funciona mas não é ideal para performance, pois impede o uso de índices no banco de dados

            var paginatedProducts = await query
                .OrderBy(p => p.ProductCategory)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var paginatedProductsResponse = new List<GetProductsByAdminIdResponse>(
                paginatedProducts.Select(p => new GetProductsByAdminIdResponse(p)).ToList());

            return Result<List<GetProductsByAdminIdResponse>>.Success(paginatedProductsResponse,
                messages: "Products found");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<List<GetProductsByAdminIdResponse>>.Failure(StatusCodes.Status500InternalServerError,
                ex.Message);
        }
    }
}