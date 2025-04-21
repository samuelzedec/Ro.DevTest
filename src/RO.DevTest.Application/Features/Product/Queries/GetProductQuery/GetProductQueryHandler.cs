using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductQuery;

public class GetProductQueryHandler(
    IProductRepository productRepository,
    IValidator<GetProductQuery> validator,
    ILogger<GetProductQueryHandler> logger) 
    : IRequestHandler<GetProductQuery, Result<GetProductResponse>>
{
    public async Task<Result<GetProductResponse>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<GetProductResponse>.Failure(messages: 
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            var product = await productRepository.GetAsync(
                cancellationToken, 
                p => p.Id == request.ProductId && p.DeletedAt == null,
                a => a.Admin);
        
            if(product is null) 
                return Result<GetProductResponse>.Failure(StatusCodes.Status404NotFound, "Product not found");
        
            return Result<GetProductResponse>.Success(new GetProductResponse(product), messages: "Product found");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<GetProductResponse>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}