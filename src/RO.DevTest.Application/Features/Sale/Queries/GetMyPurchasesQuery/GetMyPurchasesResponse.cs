using RO.DevTest.Domain.Extensions;

namespace RO.DevTest.Application.Features.Sale.Queries.GetMyPurchasesQuery;

public record GetMyPurchasesResponse
{
    public Guid SaleId { get; init; }
    public string ProductName { get; set; }
    public DateTime TransactionDate { get; init; }
    public string PaymentMethod { get; init; }
    public decimal TotalPrice { get; init; }

    public GetMyPurchasesResponse(Domain.Entities.Sale sale)
    {
        SaleId = sale.Id;
        ProductName = sale.Product.Name;
        TransactionDate = sale.TransactionDate;
        PaymentMethod = sale.EPaymentMethod.GetDescription();
        TotalPrice = sale.TotalPrice;
    }
}