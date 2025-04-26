using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Extensions;

namespace RO.DevTest.Application.Features.Sale.Queries.GetTotalRevenueQuery;

public class GetTotalRevenueQueryHandler(
    IAdminSalesSummaryRepository adminSalesSummaryRepository,
    ICurrentUserService currentUserService,
    IValidator<GetTotalRevenueQuery> validator,
    ILogger<GetTotalRevenueQueryHandler> logger)
    : IRequestHandler<GetTotalRevenueQuery, Result<GetTotalRevenueResponse>>
{
    public async Task<Result<GetTotalRevenueResponse>> Handle(GetTotalRevenueQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<GetTotalRevenueResponse>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            if (!currentUserService.IsAdmin())
                return Result<GetTotalRevenueResponse>.Failure(
                    messages: "Somente administradores têm acesso a essa informação.");

            request.StartDate ??= request.EndDate.HasValue
                ? DateTime.UtcNow.GetFirstDay(request.EndDate.Value.Year, request.EndDate.Value.Month)
                : DateTime.UtcNow.GetFirstDay();

            request.EndDate ??= DateTime.UtcNow.GetLastDay();

            var totalSummary = await adminSalesSummaryRepository
                .GetQueryable(p =>
                    p.AdminId == Guid.Parse(currentUserService.GetCurrentUserId())
                    && p.TransactionDate >= request.StartDate
                    && p.TransactionDate <= request.EndDate)
                .GroupBy(_ => 1)
                .Select(g => new 
                {
                    g.First().AdminUsername,
                    TotalValue = g.Sum(s => s.TotalValue),
                    TransactionCount = g.Sum(s => s.TransactionCount),
                    TotalItemsSold = g.Sum(s => s.TotalItemsSold)
                })
                .FirstOrDefaultAsync(cancellationToken);
            
            if (totalSummary is null)
                return Result<GetTotalRevenueResponse>.Failure(StatusCodes.Status404NotFound, 
                    "Não foram encontradas vendas para neste período especificado");

            var topProducts = await adminSalesSummaryRepository
                .GetQueryable(p =>
                    p.AdminId == Guid.Parse(currentUserService.GetCurrentUserId())
                    && p.TransactionDate >= request.StartDate
                    && p.TransactionDate <= request.EndDate)
                .GroupBy(s => new { s.ProductId, s.ProductName })
                .Select(g => new ProductSummary
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    Revenue = g.Sum(s => s.TotalValue),
                    ItemsSold = g.Sum(s => s.TotalItemsSold)
                })
                .OrderByDescending(p => p.Revenue)
                .Take(5)
                .ToListAsync(cancellationToken);
            
            return Result<GetTotalRevenueResponse>.Success(new GetTotalRevenueResponse
            {
                AdminUsername = totalSummary.AdminUsername,
                TotalValue = totalSummary?.TotalValue ?? 0,
                TransactionCount = totalSummary?.TransactionCount ?? 0,
                TotalItemsSold = totalSummary?.TotalItemsSold ?? 0,
                TopProducts = topProducts,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            }, messages: "Faturamento total de todos os produtos durante esse período");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<GetTotalRevenueResponse>.Failure(StatusCodes.Status500InternalServerError,
                "Ocorreu um erro inesperado, consulte o arquivo de hoje na pasta Logs");
        }
    }
}