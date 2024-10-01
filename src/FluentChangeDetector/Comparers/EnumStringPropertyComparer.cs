using FluentChangeDetector.Interfaces;

namespace FluentChangeDetector.Comparers;

public class EnumStringPropertyComparer<TEnum> : IPropertyComparer where TEnum : struct, Enum
{
    public new bool Equals(object? x, object? y)
    {
        var xString = x?.ToString();
        var yString = y?.ToString();

        return string.Equals(xString, yString, StringComparison.OrdinalIgnoreCase);
    }
}