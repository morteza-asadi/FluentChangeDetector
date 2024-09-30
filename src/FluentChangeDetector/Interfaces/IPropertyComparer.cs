namespace FluentChangeDetector.Interfaces;

public interface IPropertyComparer
{
    bool Equals(object? x, object? y);
}