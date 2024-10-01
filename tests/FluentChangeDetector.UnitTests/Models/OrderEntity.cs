namespace FluentChangeDetector.UnitTests.Models;

public class OrderEntity
{
    public int Id { get; set; }
    public string Customer { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}