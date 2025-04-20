using RO.DevTest.Domain.Extensions;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductQuery;

public record GetProductResponse
{
    public Guid Id { get; set; } 
    public string Name { get; set; }
    public string Description { get; set; }
    public Decimal UnitPrice { get; set; }
    public string Category { get; set; }
    public string AdminName { get; set; }
    public string AdminEmail { get; set; }
    
    public GetProductResponse(Domain.Entities.Product product)
    {
        Id = product.Id;
        Name = product.Name;
        Description = product.Description;
        UnitPrice = product.UnitPrice;
        Category = product.ProductCategory.GetDescription();
        AdminName = product.Admin.Name;
        AdminEmail = product.Admin.Email!;
    }
}