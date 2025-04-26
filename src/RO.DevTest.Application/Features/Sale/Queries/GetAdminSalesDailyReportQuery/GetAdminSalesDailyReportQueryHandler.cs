using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Extensions;

namespace RO.DevTest.Application.Features.Sale.Queries.GetAdminSalesDailyReportQuery;

public class GetAdminSalesDailyReportQueryHandler(
    IAdminSalesSummaryRepository adminSalesSummaryRepository,
    ICurrentUserService currentUserService,
    IValidator<GetAdminSalesDailyReportQuery> validator,
    ILogger<GetAdminSalesDailyReportQueryHandler> logger)
    : IRequestHandler<GetAdminSalesDailyReportQuery, Result<List<GetAdminSalesDailyReportResponse>>>
{
    public async Task<Result<List<GetAdminSalesDailyReportResponse>>> Handle(GetAdminSalesDailyReportQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<List<GetAdminSalesDailyReportResponse>>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            if (!currentUserService.IsAdmin())
                return Result<List<GetAdminSalesDailyReportResponse>>.Failure(
                    messages: "Somente administradores têm acesso a essa informação.");

            request.StartDate ??= request.EndDate.HasValue 
                ? DateTime.UtcNow.GetFirstDay(request.EndDate.Value.Year, request.EndDate.Value.Month)
                : DateTime.UtcNow.GetFirstDay();
            
            request.EndDate ??= DateTime.UtcNow.GetLastDay();

            var sales = await adminSalesSummaryRepository
                .GetQueryable(p =>
                    p.AdminId == Guid.Parse(currentUserService.GetCurrentUserId())
                    && p.TransactionDate >= request.StartDate
                    && p.TransactionDate <= request.EndDate)
                .OrderBy(s => s.TransactionDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new GetAdminSalesDailyReportResponse(s))
                .ToListAsync(cancellationToken);

            return Result<List<GetAdminSalesDailyReportResponse>>.Success(sales, messages: "Vendas encontradas");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<List<GetAdminSalesDailyReportResponse>>.Failure(StatusCodes.Status500InternalServerError,
                "Ocorreu um erro inesperado, consulte o arquivo de hoje na pasta Logs");
        }
    }
}