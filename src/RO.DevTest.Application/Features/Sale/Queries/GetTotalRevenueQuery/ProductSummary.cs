namespace RO.DevTest.Application.Features.Sale.Queries.GetTotalRevenueQuery;

public record ProductSummary
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal Revenue { get; init; }
    public int ItemsSold { get; init; }
}