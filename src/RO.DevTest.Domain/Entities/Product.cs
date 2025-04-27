using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Entities.Identity;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int AvailableQuantity { get; set; }
    public EProductCategory EProductCategory { get; set; }
    public Guid AdminId { get; set; }
    public User Admin { get; set; } = null!;
}
