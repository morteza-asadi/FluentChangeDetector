namespace FluentChangeDetector.Models;

public class ChangeResult
{
    public bool HasChanges => Changes.Count > 0;
    public List<PropertyChange> Changes { get; } = [];
}