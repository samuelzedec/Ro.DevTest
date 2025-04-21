using MediatR;
using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Application.Features.Sale.Commands.CreateSaleCommand;

public record CreateSaleCommand(
    Guid ProductId,
    PaymentMethod PaymentMethod,
    int Quantity
) : IRequest<Result<CreateSaleResponse>>;