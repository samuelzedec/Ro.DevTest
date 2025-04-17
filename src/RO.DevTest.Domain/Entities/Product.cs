using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
    public Guid SallerId { get; set; }
    public User? Saller { get; set; }
}