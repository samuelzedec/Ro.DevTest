using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Product.Queries.GetAllProductsQuery;

public class GetAllProductsQueryHandler(
    IProductRepository productRepository,
    IValidator<GetAllProductsQuery> validator,
    ILogger<GetAllProductsQueryHandler> logger)
    : IRequestHandler<GetAllProductsQuery, Result<List<GetAllProductsResponse>>>
{
    public async Task<Result<List<GetAllProductsResponse>>> Handle(GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<List<GetAllProductsResponse>>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            var query = request.AdminId.HasValue
                ? productRepository.GetQueryable(p => p.AdminId == request.AdminId.Value, p => p.Admin)
                : productRepository.GetQueryable(p => true, p => p.Admin);

            if (request.Category.HasValue)
                query = query.Where(p => p.ProductCategory == request.Category.Value);

            var paginatedProducts = await query
                .OrderBy(p => p.ProductCategory)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return Result<List<GetAllProductsResponse>>.Success(paginatedProducts.Select(p 
                => new GetAllProductsResponse(p)).ToList(), messages: "Products found");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<List<GetAllProductsResponse>>.Failure(StatusCodes.Status500InternalServerError,
                ex.Message);
        }
    }
}