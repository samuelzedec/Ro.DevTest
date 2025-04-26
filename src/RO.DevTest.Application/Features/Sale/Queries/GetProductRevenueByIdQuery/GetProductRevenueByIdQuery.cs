using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Sale.Queries.GetProductRevenueByIdQuery;

public record GetProductRevenueByIdQuery : IRequest<Result<GetProductRevenueByIdResponse>>
{
    public Guid ProductId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}