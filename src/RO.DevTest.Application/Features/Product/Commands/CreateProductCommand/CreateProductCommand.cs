using MediatR;
using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Application.Features.Product.Commands.CreateProductCommand;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal UnitPrice,
    int AvailableQuantity,
    ProductCategory ProductCategory
) : IRequest<Result<CreateProductResponse>>;