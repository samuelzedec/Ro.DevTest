using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductQuery;

public record GetProductQuery(Guid ProductId) : IRequest<Result<GetProductResponse>>;