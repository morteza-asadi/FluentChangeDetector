using FluentChangeDetector.Interfaces;
using FluentChangeDetector.Models;

namespace FluentChangeDetector;

public class ChangeDetector : IChangeDetector
{
    public ChangeResult DetectChanges<TSource, TTarget>(TSource source, TTarget target)
    {
        throw new NotImplementedException();
    }
}