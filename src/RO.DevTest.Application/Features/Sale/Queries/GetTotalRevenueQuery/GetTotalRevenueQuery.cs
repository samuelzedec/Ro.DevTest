using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Sale.Queries.GetTotalRevenueQuery;

public class GetTotalRevenueQuery : IRequest<Result<GetTotalRevenueResponse>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}