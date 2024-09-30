using FluentChangeDetector.Interfaces;

namespace FluentChangeDetector.Comparers;

public class PropertyComparer<T>(IEqualityComparer<T>? comparer) : IPropertyComparer
{
    private readonly IEqualityComparer<T> _comparer = comparer ?? EqualityComparer<T>.Default;

    public new bool Equals(object? x, object? y)
    {
        var xValue = x is T xT ? xT : default;
        var yValue = y is T yT ? yT : default;
        return _comparer.Equals(xValue, yValue);
    }
}