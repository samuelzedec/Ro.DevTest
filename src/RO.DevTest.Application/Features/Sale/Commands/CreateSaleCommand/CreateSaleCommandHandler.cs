using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Sale.Commands.CreateSaleCommand;

public class CreateSaleCommandHandler(
    ISaleRepository saleRepository,
    IProductRepository productRepository,
    IValidator<CreateSaleCommand> validator,
    ICurrentUserService currentUserService,
    ILogger<CreateSaleCommandHandler> logger)
    : IRequestHandler<CreateSaleCommand, Result<CreateSaleResponse>>
{
    public async Task<Result<CreateSaleResponse>> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<CreateSaleResponse>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            if (currentUserService.IsAdmin())
                return Result<CreateSaleResponse>.Failure(messages: "Somente clientes podem realizar compras.");

            var product = await productRepository.GetAsync(
                cancellationToken,
                p => p.Id == request.ProductId && p.DeletedAt == null);

            if (product is not { AvailableQuantity: > 0 })
                return Result<CreateSaleResponse>.Failure(messages: "Produto fora de estoque");

            if (product.AvailableQuantity < request.Quantity)
                return Result<CreateSaleResponse>.Failure(messages: "Quantidade insuficiente disponível");

            var sale = new Domain.Entities.Sale
            {
                AdminId = product.AdminId,
                ProductId = product.Id,
                CustomerId = Guid.Parse(currentUserService.GetCurrentUserId()),
                PaymentMethod = request.PaymentMethod,
                Quantity = request.Quantity,
                UnitPrice = product.UnitPrice
            };
            await saleRepository.CreateAsync(sale, cancellationToken);
            product.AvailableQuantity -= request.Quantity;
            await productRepository.UpdateAsync(product, cancellationToken);
            var completeSale = await saleRepository.GetAsync(
                cancellationToken,
                s => s.Id == sale.Id,
                s => s.Admin,
                s => s.Customer,
                s => s.Product);

            return Result<CreateSaleResponse>.Success(
                new CreateSaleResponse(completeSale!),
                StatusCodes.Status201Created,
                messages: "Compra realizada com sucesso");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<CreateSaleResponse>.Failure(StatusCodes.Status500InternalServerError, 
                "Ocorreu um erro inesperado, consulte o arquivo de hoje na pasta Logs");
        }
    }
}
