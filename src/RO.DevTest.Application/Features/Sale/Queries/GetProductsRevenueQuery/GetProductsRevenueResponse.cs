namespace RO.DevTest.Application.Features.Sale.Queries.GetProductsRevenueQuery;

public record GetProductsRevenueResponse
{
    public string AdminUsername { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public decimal TotalValue { get; init; }
    public int TransactionCount { get; init; }
    public int TotalItemsSold { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}