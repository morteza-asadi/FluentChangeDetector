using FluentChangeDetector.Models;

namespace FluentChangeDetector.Interfaces;

public interface IChangeDetector
{
    ChangeResult DetectChanges<TSource, TTarget>(TSource source, TTarget target);
}
