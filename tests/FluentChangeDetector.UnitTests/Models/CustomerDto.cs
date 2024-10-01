namespace FluentChangeDetector.UnitTests.Models;

public class CustomerDto
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class EntityWithEnum
{
    public Status Status { get; set; }
}

public enum Status
{
    Active,
    Inactive
}