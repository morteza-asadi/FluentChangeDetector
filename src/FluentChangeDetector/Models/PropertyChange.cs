namespace FluentChangeDetector.Models;

public class PropertyChange
{
    public required string PropertyName { get; set; }
    public object? SourceValue { get; set; }
    public  object? TargetValue { get; set; }
}