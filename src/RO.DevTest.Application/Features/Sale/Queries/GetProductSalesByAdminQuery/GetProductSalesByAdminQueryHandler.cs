using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Extensions;

namespace RO.DevTest.Application.Features.Sale.Queries.GetProductSalesByAdminQuery;

public class GetProductSalesByAdminQueryHandler(
    IAdminSalesSummaryRepository adminSalesSummaryRepository,
    ICurrentUserService currentUserService,
    IValidator<GetProductSalesByAdminQuery> validator,
    ILogger<GetProductSalesByAdminQueryHandler> logger)
    : IRequestHandler<GetProductSalesByAdminQuery, Result<List<GetProductSalesByAdminResponse>>>
{
    public async Task<Result<List<GetProductSalesByAdminResponse>>> Handle(GetProductSalesByAdminQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<List<GetProductSalesByAdminResponse>>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            if (!currentUserService.IsAdmin())
                return Result<List<GetProductSalesByAdminResponse>>.Failure(
                    messages: "Only admins have access to this information.");

            request.StartDate ??= request.EndDate.HasValue 
                ? DateTime.UtcNow.GetFirstDay(request.EndDate.Value.Year, request.EndDate.Value.Month)
                : DateTime.UtcNow.GetFirstDay();
            
            request.EndDate ??= DateTime.UtcNow.GetLastDay();

            var sales = await adminSalesSummaryRepository
                .GetQueryable(p =>
                    p.AdminId == Guid.Parse(currentUserService.GetCurrentUserId())
                    && p.TransactionDate >= request.StartDate
                    && p.TransactionDate <= request.EndDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new GetProductSalesByAdminResponse(s))
                .ToListAsync(cancellationToken);

            return Result<List<GetProductSalesByAdminResponse>>.Success(sales, messages: "Sales Found");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<List<GetProductSalesByAdminResponse>>.Failure(StatusCodes.Status500InternalServerError,
                ex.Message);
        }
    }
}