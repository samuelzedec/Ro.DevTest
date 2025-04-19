using System.ComponentModel.DataAnnotations.Schema;
using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Entities.Identity;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Domain.Entities;

public class Sale : BaseEntity
{
    public Guid AdminId { get; set; }
    public User? Admin { get; set; }

    public Guid ProductId { get; set; }
    public Product? Product { get; set; }

    public Guid CustomerId { get; set; }
    public User? Customer { get; set; }

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    [NotMapped] public decimal TotalPrice => UnitPrice * Quantity;
}