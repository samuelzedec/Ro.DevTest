using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Extensions;

namespace RO.DevTest.Application.Features.Sale.Queries.GetSalesByPeriodQuery;

public class GetSalesByPeriodQueryHandler(
    ISaleRepository saleRepository,
    ICurrentUserService currentUserService,
    IValidator<GetSalesByPeriodQuery> validator,
    ILogger<GetSalesByPeriodQueryHandler> logger)
    : IRequestHandler<GetSalesByPeriodQuery, Result<List<GetSalesByPeriodResponse>>>
{
    public async Task<Result<List<GetSalesByPeriodResponse>>> Handle(GetSalesByPeriodQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<List<GetSalesByPeriodResponse>>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            if (!currentUserService.IsAdmin())
                return Result<List<GetSalesByPeriodResponse>>.Failure(messages: "Only admins have access");

            request.StartDate ??= request.EndDate.HasValue
                ? DateTime.UtcNow.GetFirstDay(request.EndDate.Value.Year, request.EndDate.Value.Month)
                : DateTime.UtcNow.GetFirstDay();

            request.EndDate ??= DateTime.UtcNow.GetLastDay();

            var sales = await saleRepository
                .GetQueryable(s
                        => s.AdminId == Guid.Parse(currentUserService.GetCurrentUserId())
                           && s.DeletedAt == null
                           && s.TransactionDate >= request.StartDate
                           && s.TransactionDate <= request.EndDate,
                    sp => sp.Product,
                    sa => sa.Admin,
                    sc => sc.Customer)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new GetSalesByPeriodResponse(s))
                .OrderBy(s => s.TransactionDate)
                .ToListAsync(cancellationToken);

            return Result<List<GetSalesByPeriodResponse>>.Success(sales, messages: "Purchases found");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<List<GetSalesByPeriodResponse>>.Failure(StatusCodes.Status500InternalServerError,
                ex.Message);
        }
    }
}