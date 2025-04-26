using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Sale.Queries.GetProductsRevenueQuery;

public record GetProductsRevenueQuery : IRequest<Result<List<GetProductsRevenueResponse>>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}