namespace FluentChangeDetector.UnitTests.Models;

public class EntityWithCollection
{
    public int Id { get; set; }
    public List<string> Items { get; set; } = new();
}