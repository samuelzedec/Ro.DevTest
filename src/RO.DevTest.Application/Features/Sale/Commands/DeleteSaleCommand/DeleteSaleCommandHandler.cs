using System.Transactions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Sale.Commands.DeleteSaleCommand;

public class DeleteSaleCommandHandler(
    ISaleRepository saleRepository,
    IProductRepository productRepository,
    IValidator<DeleteSaleCommand> validator,
    ICurrentUserService currentUserService,
    ILogger<DeleteSaleCommandHandler> logger)
    : IRequestHandler<DeleteSaleCommand, Result<DeleteSaleResponse>>
{
    public async Task<Result<DeleteSaleResponse>> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<DeleteSaleResponse>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }
            
            var sale = await saleRepository.GetAsync(
                cancellationToken,
                p => p.Id == request.SaleId
                     && p.DeletedAt == null
                     && p.CustomerId == Guid.Parse(currentUserService.GetCurrentUserId()));
            
            if (sale is null)
                return Result<DeleteSaleResponse>.Failure(StatusCodes.Status404NotFound, messages: "Compra não encontrada.");

            var product = await productRepository.GetAsync(
                cancellationToken, 
                p => p.Id == sale.ProductId
                && p.DeletedAt == null);

            if (product is null)
                return Result<DeleteSaleResponse>.Failure(StatusCodes.Status404NotFound, messages: "Você não pode excluir a compra porque o produto não está mais disponível.");
 
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                product.AvailableQuantity += sale.Quantity;
                sale.Quantity = 0;
                sale.DeletedAt = DateTime.Now;
    
                await saleRepository.UpdateAsync(sale, cancellationToken);
                await productRepository.UpdateAsync(product, cancellationToken);
    
                transaction.Complete();
            }
            
            return Result<DeleteSaleResponse>.Success(new DeleteSaleResponse(sale), messages: "Compra excluída com sucesso.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<DeleteSaleResponse>.Failure(StatusCodes.Status500InternalServerError, 
                "Ocorreu um erro inesperado, consulte o arquivo de hoje na pasta Logs");
        }
    }
}
