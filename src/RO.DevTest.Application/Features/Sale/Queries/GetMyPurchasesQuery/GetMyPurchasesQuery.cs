using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Sale.Queries.GetMyPurchasesQuery;

public record GetMyPurchasesQuery(
    int PageNumber = 1,
    int PageSize = 25
) : IRequest<Result<List<GetMyPurchasesResponse>>>;