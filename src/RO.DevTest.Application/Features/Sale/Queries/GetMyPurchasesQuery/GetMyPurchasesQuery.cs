using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Sale.Queries.GetMyPurchasesQuery;

public record GetMyPurchasesQuery : IRequest<Result<List<GetMyPurchasesResponse>>>;