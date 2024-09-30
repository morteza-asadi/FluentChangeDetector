namespace FluentChangeDetector.Configurations;

public class MappingConfig<TSource, TTarget> : MappingConfig
{
    private readonly ChangeDetectorConfig _changeDetectorConfig;
    public MappingConfig(ChangeDetectorConfig changeDetectorConfig)
    {
        SourceType = typeof(TSource);
        TargetType = typeof(TTarget);
        _changeDetectorConfig = changeDetectorConfig;
    }
    
}