using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Product.Commands.CreateProductCommand;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICurrentUserService currentUserService,
    IValidator<CreateProductCommand> validator,
    ILogger<CreateProductCommandHandler> logger)
    : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<CreateProductResponse>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            if (!currentUserService.IsAdmin())
                return Result<CreateProductResponse>.Failure(StatusCodes.Status401Unauthorized, messages: "Apenas administradores podem criar produtos");

            var product = new Domain.Entities.Product
            {
                AdminId = Guid.Parse(currentUserService.GetCurrentUserId()),
                Name = request.Name,
                Description = request.Description,
                UnitPrice = request.UnitPrice,
                AvailableQuantity = request.AvailableQuantity,
                ProductCategory = request.ProductCategory
            };

            await productRepository.CreateAsync(product, cancellationToken);
            return Result<CreateProductResponse>.Success(
                new CreateProductResponse(product),
                StatusCodes.Status201Created,
                "Produto criado com sucesso");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<CreateProductResponse>.Failure(StatusCodes.Status500InternalServerError,
                "Ocorreu um erro inesperado, consulte o arquivo de hoje na pasta Logs");
        }
    }
}