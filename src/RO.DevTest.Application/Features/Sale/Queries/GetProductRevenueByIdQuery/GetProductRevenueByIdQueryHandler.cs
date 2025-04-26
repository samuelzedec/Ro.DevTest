using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Extensions;

namespace RO.DevTest.Application.Features.Sale.Queries.GetProductRevenueByIdQuery;

public class GetProductRevenueByIdQueryHandler(
    IAdminSalesSummaryRepository adminSalesSummaryRepository,
    ICurrentUserService currentUserService,
    IValidator<GetProductRevenueByIdQuery> validator,
    ILogger<GetProductRevenueByIdQueryHandler> logger)
    : IRequestHandler<GetProductRevenueByIdQuery, Result<GetProductRevenueByIdResponse>>
{
    public async Task<Result<GetProductRevenueByIdResponse>> Handle(GetProductRevenueByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<GetProductRevenueByIdResponse>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            if (!currentUserService.IsAdmin())
                return Result<GetProductRevenueByIdResponse>.Failure(
                    messages: "Somente administradores têm acesso a essa informação.");
            
            request.StartDate ??= request.EndDate.HasValue 
                ? DateTime.UtcNow.GetFirstDay(request.EndDate.Value.Year, request.EndDate.Value.Month)
                : DateTime.UtcNow.GetFirstDay();
            
            request.EndDate ??= DateTime.UtcNow.GetLastDay();

            var sale = await adminSalesSummaryRepository
                .GetQueryable(p =>
                    p.AdminId == Guid.Parse(currentUserService.GetCurrentUserId())
                    && p.ProductId == request.ProductId
                    && p.TransactionDate >= request.StartDate
                    && p.TransactionDate <= request.EndDate)
                .GroupBy(s => new { s.ProductId, s.ProductName, s.AdminUsername })
                .Select(g => new GetProductRevenueByIdResponse
                {
                    AdminUsername = g.Key.AdminUsername,
                    ProductName = g.Key.ProductName,
                    TotalValue = g.Sum(s => s.TotalValue),
                    TotalItemsSold = g.Sum(s => s.TotalItemsSold),
                    TransactionCount = g.Count(),
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                }).FirstOrDefaultAsync(cancellationToken);

            if (sale is null)
                return Result<GetProductRevenueByIdResponse>.Failure(StatusCodes.Status404NotFound, 
                    "Não foram encontradas vendas para este produto no período especificado ou o produto não existe");
            
            return Result<GetProductRevenueByIdResponse>.Success(sale, messages: "Relatório da venda desse produto");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<GetProductRevenueByIdResponse>.Failure(StatusCodes.Status500InternalServerError,
                "Ocorreu um erro inesperado, consulte o arquivo de hoje na pasta Logs");
        }
    }
}