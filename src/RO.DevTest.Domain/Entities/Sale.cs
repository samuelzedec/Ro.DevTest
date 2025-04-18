using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Domain.Entities;

public class Sale : BaseEntity
{
    public Guid AdminId { get; set; }
    public User? Admin { get; set; }
    
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    
    public Guid CustomerId { get; set; }
    public User? Customer { get; set; }
    
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}