namespace RO.DevTest.Application.Features.Sale.Queries.GetTotalRevenueQuery;

public class GetTotalRevenueResponse
{
    public string AdminUsername { get; init; } = string.Empty;
    public decimal TotalValue { get; init; }
    public int TransactionCount { get; init; }
    public int TotalItemsSold { get; init; }
    public List<ProductSummary> TopProducts { get; init; } = [];
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}