using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Sale.Queries.GetSalesByPeriodQuery;

public record GetSalesByPeriodQuery : IRequest<Result<List<GetSalesByPeriodResponse>>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 25;
}