using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Product.Commands.UpdateProductCommand;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal UnitPrice,
    int AvailableQuantity
) : IRequest<Result<UpdateProductResponse>>;