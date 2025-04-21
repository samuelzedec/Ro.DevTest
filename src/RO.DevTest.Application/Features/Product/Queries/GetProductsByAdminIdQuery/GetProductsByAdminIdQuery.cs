using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductsByAdminIdQuery;

public record GetProductsByAdminIdQuery(
    int PageNumber = 1, 
    int PageSize = 25
) : IRequest<Result<List<GetProductsByAdminIdResponse>>>;