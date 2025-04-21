using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductsByAdminIdQuery;

public class GetProductsByAdminIdQuery : IRequest<Result<List<GetProductsByAdminIdResponse>>>
{
    public string SearchTerm { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
};