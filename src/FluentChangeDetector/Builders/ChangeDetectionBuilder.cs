using FluentChangeDetector.Configurations;
using FluentChangeDetector.Models;

namespace FluentChangeDetector.Builders;

public class ChangeDetectionBuilder<TSource, TTarget>
{
    private readonly TSource _source;
    private readonly TTarget _target;
    private readonly ChangeDetectorConfig _config = new();
    private readonly MappingConfig<TSource, TTarget> _mappingConfig;

    public ChangeDetectionBuilder(TSource source, TTarget target)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
        _target = target ?? throw new ArgumentNullException(nameof(target));
        _mappingConfig = new MappingConfig<TSource, TTarget>(_config);
        _config.AddMapping(_mappingConfig);
    }

    public ChangeDetectionBuilder<TSource, TTarget> ConfigureMapping(Action<MappingConfig<TSource, TTarget>> configAction)
    {
        ArgumentNullException.ThrowIfNull(configAction);
        configAction(_mappingConfig);
        return this;
    }

    public ChangeDetectionBuilder<TSource, TTarget> ConfigureGlobal(Action<ChangeDetectorConfig> configAction)
    {
        ArgumentNullException.ThrowIfNull(configAction);
        configAction(_config);
        return this;
    }

    public ChangeResult Result
    {
        get
        {
            var changeDetector = new ChangeDetector(_config);
            return changeDetector.DetectChanges(_source, _target);
        }
    }

    public async Task<ChangeResult> ResultAsync()
    {
        return await Task.FromResult(Result);
    }
}