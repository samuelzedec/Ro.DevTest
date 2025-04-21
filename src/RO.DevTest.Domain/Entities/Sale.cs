using System.ComponentModel.DataAnnotations.Schema;
using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Entities.Identity;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Domain.Entities;

public class Sale : BaseEntity
{
    public Guid AdminId { get; set; }
    public User Admin { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid CustomerId { get; set; }
    public User Customer { get; set; } = null!;

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public PaymentMethod PaymentMethod { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    [NotMapped] public decimal TotalPrice => UnitPrice * Quantity;
}