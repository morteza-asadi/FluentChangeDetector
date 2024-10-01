using FluentChangeDetector.Interfaces;

namespace FluentChangeDetector.UnitTests.Comparers;

public class IntStringComparer : IPropertyComparer
{
    public new bool Equals(object? x, object? y)
    {
        if (x is int xInt && y is string yString && int.TryParse(yString, out int yInt))
        {
            return xInt == yInt;
        }
        return false;
    }
}