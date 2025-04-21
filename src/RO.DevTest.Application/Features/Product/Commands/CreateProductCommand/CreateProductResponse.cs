using RO.DevTest.Domain.Extensions;

namespace RO.DevTest.Application.Features.Product.Commands.CreateProductCommand;

public record CreateProductResponse
{
    public Guid Id { get; init; } 
    public string Name { get; init; }
    public string Description { get; init; }
    public Decimal UnitPrice { get; init; }
    public string Category { get; init; }

    public CreateProductResponse(Domain.Entities.Product product)
    {
        Id = product.Id;
        Name = product.Name;
        Description = product.Description;
        UnitPrice = product.UnitPrice;
        Category = product.ProductCategory.GetDescription();
    }
}