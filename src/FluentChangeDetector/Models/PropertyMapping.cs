using System.Reflection;
using FluentChangeDetector.Interfaces;

namespace FluentChangeDetector.Models;

public class PropertyMapping
{
    public required PropertyInfo SourceProperty { get; set; }
    public required PropertyInfo TargetProperty { get; set; }
    public required Func<object, object?> SourceGetter { get; set; }
    public required Func<object, object?> TargetGetter { get; set; }
    public IPropertyComparer? Comparer { get; set; }
}