using RO.DevTest.Domain.Extensions;

namespace RO.DevTest.Application.Features.Sale.Commands.DeleteSaleCommand;

public record DeleteSaleResponse
{
    public Guid SaleId { get; init; }
    public DateTime TransactionDate { get; init; }
    public string PaymentMethod { get; init; }
    public decimal TotalPrice { get; init; }

    public DeleteSaleResponse(Domain.Entities.Sale sale)
    {
        SaleId = sale.Id;
        TransactionDate = sale.TransactionDate;
        PaymentMethod = sale.EPaymentMethod.GetDescription();
        TotalPrice = sale.TotalPrice;
    }
}