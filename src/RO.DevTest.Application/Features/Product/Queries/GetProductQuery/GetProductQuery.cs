using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductQuery;

public class GetProductQuery : IRequest<Result<GetProductResponse>>
{
    public Guid ProductId { get; set; }
}