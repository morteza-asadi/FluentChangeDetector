using FluentChangeDetector.Builders;

namespace FluentChangeDetector.Extensions;

public static class ChangeDetectorExtensions
{
    public static ChangeDetectionBuilder<TSource, TTarget> DetectChanges<TSource, TTarget>(this TSource source, TTarget target)
    {
        return new ChangeDetectionBuilder<TSource, TTarget>(source, target);
    }
}