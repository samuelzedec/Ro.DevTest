using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Entities.Identity;

namespace RO.DevTest.Domain.Entities;

public class Sale : BaseEntity
{
    public Guid SallerId { get; set; }
    public User? Saller { get; set; }
    
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    
    public Guid BuyerId { get; set; }
    public User? Buyer { get; set; }
    
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}