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
                return Result<DeleteSaleResponse>.Failure(messages: "Purchase not found");
            
            sale.DeletedAt = DateTime.Now;
            await saleRepository.UpdateAsync(sale, cancellationToken);
            return Result<DeleteSaleResponse>.Success(new DeleteSaleResponse(sale), messages: "Deleted purchase");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<DeleteSaleResponse>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}