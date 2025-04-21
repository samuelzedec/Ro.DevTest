using MediatR;
using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductsByCategoryQuery;

public record GetProductsByCategoryQuery(
    ProductCategory Category,
    int PageNumber = 1,
    int PageSize = 25
) : IRequest<Result<List<GetProductsByCategoryResponse>>>;