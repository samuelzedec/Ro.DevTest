using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Sale.Queries.GetSaleByIdQuery;

public class GetSaleByIdQueryHandler(
    ISaleRepository saleRepository,
    IValidator<GetSaleByIdQuery> validator,
    ILogger<GetSaleByIdQueryHandler> logger) 
    : IRequestHandler<GetSaleByIdQuery, Result<GetSaleByIdResponse>>
{
    public async Task<Result<GetSaleByIdResponse>> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<GetSaleByIdResponse>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            var sale = await saleRepository
                .GetQueryable(
                    s => s.Id == request.SaleId && s.DeletedAt == null,
                    s => s.Product,
                    s=> s.Customer,
                    s => s.Admin)
                .FirstOrDefaultAsync(cancellationToken);
            
            if(sale is null)
                return Result<GetSaleByIdResponse>.Failure(StatusCodes.Status404NotFound, messages: "Sale not found");
            
            return Result<GetSaleByIdResponse>.Success(new GetSaleByIdResponse(sale), messages: "Sale found");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<GetSaleByIdResponse>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}