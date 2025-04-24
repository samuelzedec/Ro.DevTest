namespace RO.DevTest.Application.Features.Sale.Queries.GetProductSalesByAdminQuery;

public record GetProductSalesByAdminResponse
{
    public string AdminUsername { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; }
    public DateTime TransactionDate { get; init; }
    public decimal TotalValue { get; init; }
    public int TransactionCount { get; init; }
    public int TotalItemsSold { get; init; }

    public GetProductSalesByAdminResponse(Domain.ReadModels.AdminSalesSummary adminSalesSummary)
    {
        AdminUsername = adminSalesSummary.AdminUsername;
        ProductId = adminSalesSummary.ProductId;
        ProductName = adminSalesSummary.ProductName;
        TransactionDate = adminSalesSummary.TransactionDate;
        TotalValue = adminSalesSummary.TotalValue;
        TransactionCount = adminSalesSummary.TransactionCount;
        TotalItemsSold = adminSalesSummary.TotalItemsSold;
    }
}