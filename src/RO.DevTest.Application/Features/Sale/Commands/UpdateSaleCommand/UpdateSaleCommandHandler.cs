using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Sale.Commands.UpdateSaleCommand;

public class UpdateSaleCommandHandler(
    ISaleRepository saleRepository,
    IValidator<UpdateSaleCommand> validator,
    ICurrentUserService currentUserService,
    ILogger<UpdateSaleCommandHandler> logger)
    : IRequestHandler<UpdateSaleCommand, Result<UpdateSaleResponse>>
{
    public async Task<Result<UpdateSaleResponse>> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<UpdateSaleResponse>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            var sale = await saleRepository.GetAsync(
                cancellationToken,
                p => p.Id == request.SaleId
                     && p.DeletedAt == null
                     && p.CustomerId == Guid.Parse(currentUserService.GetCurrentUserId()));

            if (sale is null)
                return Result<UpdateSaleResponse>.Failure(messages: "Compra não encontrada");

            sale.PaymentMethod = request.PaymentMethod;
            sale.ModifiedOn = DateTime.Now;
            await saleRepository.UpdateAsync(sale, cancellationToken);
            return Result<UpdateSaleResponse>.Success(new UpdateSaleResponse(sale), messages: "Compra atualizada");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<UpdateSaleResponse>.Failure(StatusCodes.Status500InternalServerError, 
                "Ocorreu um erro inesperado, consulte o arquivo de hoje na pasta Logs");
        }
    }
}