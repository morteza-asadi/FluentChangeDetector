using FluentChangeDetector.Configurations;
using FluentChangeDetector.Interfaces;
using FluentChangeDetector.Models;

namespace FluentChangeDetector;

public class ChangeDetector : IChangeDetector
{
    private readonly ChangeDetectorConfig _config;

    public ChangeDetector()
    {
        _config = new ChangeDetectorConfig();
    }

    public ChangeDetector(ChangeDetectorConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }
    
    public ChangeResult DetectChanges<TSource, TTarget>(TSource source, TTarget target)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (target == null) throw new ArgumentNullException(nameof(target));

        var result = new ChangeResult();
        
        var mappingConfig = _config.GetMapping(typeof(TSource), typeof(TTarget));

        if (mappingConfig == null)
        {
            mappingConfig = new MappingConfig<TSource, TTarget>(_config);
            _config.AddMapping(mappingConfig);
        }
        
        var mappings = mappingConfig.GetPropertyMappings();
        
        foreach (var mapping in mappings)
        {
            var sourceValue = mapping.SourceGetter(source);
            var targetValue = mapping.TargetGetter(target);

            var propertyName = mapping.SourceProperty.Name;
            var propertyType = mapping.SourceProperty.PropertyType;

            bool areEqual;

            if (mapping.Comparer != null)
            {
                areEqual = mapping.Comparer.Equals(sourceValue, targetValue);
            }
            else if (_config.GetTypeComparer(propertyType) is { } typeComparer)
            {
                areEqual = typeComparer.Equals(sourceValue, targetValue);
            }
            else
            {
                areEqual = DefaultEquals(sourceValue, targetValue);
            }

            if (!areEqual)
            {
                result.Changes.Add(new PropertyChange
                {
                    PropertyName = propertyName,
                    SourceValue = sourceValue,
                    TargetValue = targetValue
                });
            }
        }

        return result;
    }
    
    private static bool DefaultEquals(object? sourceValue, object? targetValue)
    {
        if (sourceValue == null && targetValue == null)
            return true;

        if (sourceValue == null || targetValue == null)
            return false;

        return sourceValue.Equals(targetValue);
    }
}