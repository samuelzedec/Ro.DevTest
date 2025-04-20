using RO.DevTest.Domain.Extensions;

namespace RO.DevTest.Application.Features.Product.Commands.UpdateProductCommand;

public record UpdateProductResponse
{
    public Guid Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Decimal UnitPrice { get; set; }
    public string Category { get; set; }

    public UpdateProductResponse(Domain.Entities.Product product)
    {
        Id = product.Id;
        Name = product.Name;
        Description = product.Description;
        UnitPrice = product.UnitPrice;
        Category = product.ProductCategory.GetDescription();
    }
}