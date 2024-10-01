namespace FluentChangeDetector.UnitTests.Comparers;

public class TrimStringComparer : IEqualityComparer<string>
{
    public bool Equals(string? x, string? y)
    {
        return string.Equals(x?.Trim(), y?.Trim(), StringComparison.Ordinal);
    }

    public int GetHashCode(string obj)
    {
        return obj.Trim().GetHashCode();
    }
}