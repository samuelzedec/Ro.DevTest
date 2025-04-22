using RO.DevTest.Domain.Extensions;

namespace RO.DevTest.Application.Features.Sale.Queries.GetSaleByIdQuery;

public record GetSaleByIdResponse
{
    public Guid SaleId { get; init; }
    public string AdminName { get; init; }
    public string ProductName { get; init; }
    public string CustomerName { get; init; }
    public string CustomerEmail { get; init; }
    public DateTime TransactionDate { get; init; }
    public string PaymentMethod { get; init; }
    public int QuantityPurchased { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice { get; init; }

    public GetSaleByIdResponse(Domain.Entities.Sale sale)
    {
        SaleId = sale.Id;
        AdminName = sale.Admin.Name;
        ProductName = sale.Product.Name;
        CustomerName = sale.Customer.Name;
        CustomerEmail = sale.Customer.Email!;
        TransactionDate = sale.TransactionDate;
        PaymentMethod = sale.PaymentMethod.GetDescription();
        QuantityPurchased = sale.Quantity;
        UnitPrice = sale.UnitPrice;
        TotalPrice = sale.TotalPrice;
    }
}