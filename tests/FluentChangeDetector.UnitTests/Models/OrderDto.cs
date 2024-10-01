namespace FluentChangeDetector.UnitTests.Models;

public class OrderDto
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}