using RO.DevTest.Domain.Extensions;

namespace RO.DevTest.Application.Features.Product.Commands.CreateProductCommand;

public record CreateProductResponse
{
    public Guid Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; }

    public CreateProductResponse(Domain.Entities.Product product)
    {
        Id = product.Id;
        Name = product.Name;
        Description = product.Description;
        Category = product.ProductCategory.GetDescription();
    }
}