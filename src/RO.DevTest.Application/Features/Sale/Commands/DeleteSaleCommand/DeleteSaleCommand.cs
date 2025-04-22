using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Sale.Commands.DeleteSaleCommand;

public record DeleteSaleCommand (Guid SaleId) : IRequest<Result<DeleteSaleResponse>>;