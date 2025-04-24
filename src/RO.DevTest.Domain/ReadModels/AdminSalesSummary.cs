namespace RO.DevTest.Domain.ReadModels;

public class AdminSalesSummary
{
    public Guid AdminId { get; set; }
    public string AdminUsername { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public decimal TotalValue { get; set; }
    public int TransactionCount { get; set; }
    public int TotalItemsSold { get; set; }
}