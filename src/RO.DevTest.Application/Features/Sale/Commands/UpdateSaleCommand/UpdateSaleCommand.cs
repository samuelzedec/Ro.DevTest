using MediatR;
using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Application.Features.Sale.Commands.UpdateSaleCommand;

public record UpdateSaleCommand(
    Guid SaleId,
    PaymentMethod PaymentMethod
) : IRequest<Result<UpdateSaleResponse>>;