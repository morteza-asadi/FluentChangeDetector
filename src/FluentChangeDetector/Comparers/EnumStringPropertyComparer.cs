using FluentChangeDetector.Interfaces;

namespace FluentChangeDetector.Comparers;

public class EnumStringPropertyComparer<TEnum> : IPropertyComparer where TEnum : struct, Enum
{
    public new bool Equals(object? x, object? y)
    {
        if (x is not TEnum xEnum || y is not TEnum yEnum) return false;
        
        var xString = xEnum.ToString();
        var yString = yEnum.ToString();
            
        return string.Equals(xString, yString, StringComparison.OrdinalIgnoreCase);
    } 
}