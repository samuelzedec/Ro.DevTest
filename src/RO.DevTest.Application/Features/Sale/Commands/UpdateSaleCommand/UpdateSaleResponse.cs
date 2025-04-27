using RO.DevTest.Domain.Extensions;

namespace RO.DevTest.Application.Features.Sale.Commands.UpdateSaleCommand;

public class UpdateSaleResponse
{
    public Guid SaleId { get; init; }
    public DateTime TransactionDate { get; init; }
    public string PaymentMethod { get; init; }
    public decimal TotalPrice { get; init; }

    public UpdateSaleResponse(Domain.Entities.Sale sale)
    {
        SaleId = sale.Id;
        TransactionDate = sale.TransactionDate;
        PaymentMethod = sale.EPaymentMethod.GetDescription();
        TotalPrice = sale.TotalPrice;
    }
}