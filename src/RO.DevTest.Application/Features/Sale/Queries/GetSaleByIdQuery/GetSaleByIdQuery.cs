using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Sale.Queries.GetSaleByIdQuery;

public record GetSaleByIdQuery(Guid SaleId) : IRequest<Result<GetSaleByIdResponse>>;