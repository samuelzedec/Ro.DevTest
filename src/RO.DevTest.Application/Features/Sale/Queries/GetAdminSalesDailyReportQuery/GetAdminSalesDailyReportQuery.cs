using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Sale.Queries.GetAdminSalesDailyReportQuery;

public record GetAdminSalesDailyReportQuery : IRequest<Result<List<GetAdminSalesDailyReportResponse>>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 25;
};