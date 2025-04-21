using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductsByCategoryQuery;

public class GetProductsByCategoryQueryHandler(
    IProductRepository productRepository,
    IValidator<GetProductsByCategoryQuery> validator,
    ILogger<GetProductsByCategoryQueryHandler> logger)
    : IRequestHandler<GetProductsByCategoryQuery, Result<List<GetProductsByCategoryResponse>>>
{
    public async Task<Result<List<GetProductsByCategoryResponse>>> Handle(GetProductsByCategoryQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<List<GetProductsByCategoryResponse>>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            var query = productRepository.GetQueryable(p =>
                p.ProductCategory == request.Category,
                p => p.Admin);

            var paginatedProducts = await query
                .OrderBy(p => p.ProductCategory)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var paginatedProductsResponse = paginatedProducts
                .Select(p => new GetProductsByCategoryResponse(p))
                .ToList();

            return Result<List<GetProductsByCategoryResponse>>.Success(paginatedProductsResponse,
                messages: "Products found");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<List<GetProductsByCategoryResponse>>.Failure(StatusCodes.Status500InternalServerError,
                ex.Message);
        }
    }
}