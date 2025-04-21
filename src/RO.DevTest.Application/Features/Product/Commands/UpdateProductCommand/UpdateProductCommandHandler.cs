using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Product.Commands.UpdateProductCommand;

public class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ICurrentUserService currentUserService,
    IValidator<UpdateProductCommand> validator,
    ILogger<UpdateProductCommandHandler> logger)
    : IRequestHandler<UpdateProductCommand, Result<UpdateProductResponse>>
{
    public async Task<Result<UpdateProductResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<UpdateProductResponse>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }
            
            var product = await productRepository.GetAsync(
                cancellationToken,
                p => p.Id == request.Id && p.AdminId == Guid.Parse(currentUserService.GetCurrentUserId()));

            if (product is null)
                return Result<UpdateProductResponse>.Failure(messages: "Product not found or you do not have access");

            product.Name = string.IsNullOrWhiteSpace(request.Name) ? product.Name : request.Name;
            product.Description = string.IsNullOrWhiteSpace(request.Description) ? product.Description : request.Description;
            product.UnitPrice = request.UnitPrice;
            product.AvailableQuantity = request.AvailableQuantity;
            product.ModifiedOn = DateTime.UtcNow;

            await productRepository.UpdateAsync(product, cancellationToken);
            return Result<UpdateProductResponse>.Success(new UpdateProductResponse(product), messages: "Updated product");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<UpdateProductResponse>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}