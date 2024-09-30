using System.Reflection;
using FluentChangeDetector.Comparers;
using FluentChangeDetector.Interfaces;

namespace FluentChangeDetector.Configurations;

public class ChangeDetectorConfig
{
    private readonly Dictionary<(Type, Type), MappingConfig> _mappings = new();
    private readonly Dictionary<Type, IPropertyComparer> _typeComparers = new();

    public void AddMapping(MappingConfig mappingConfig)
    {
        var key = (mappingConfig.SourceType, mappingConfig.TargetType);
        _mappings[key] = mappingConfig;
    }

    public MappingConfig<TSource, TTarget> CreateMapping<TSource, TTarget>()
    {
        var mappingConfig = new MappingConfig<TSource, TTarget>(this);
        AddMapping(mappingConfig);
        return mappingConfig;
    }

    public MappingConfig? GetMapping(Type sourceType, Type targetType)
    {
        var key = (sourceType, targetType);
        _mappings.TryGetValue(key, out var mapping);
        return mapping;
    }

    public void AddTypeComparer<T>(IEqualityComparer<T> comparer)
    {
        _typeComparers[typeof(T)] = new PropertyComparer<T>(comparer);
    }

    internal IPropertyComparer? GetTypeComparer(Type type)
    {
        _typeComparers.TryGetValue(type, out var comparer);
        return comparer;
    }

    public void AddMappingsFromAssembly(Assembly assembly)
    {
        var mappingTypes = assembly.GetTypes();

        foreach (var type in mappingTypes)
        {
            if (!typeof(IChangeDetectorMapping).IsAssignableFrom(type) || type.IsInterface || type.IsAbstract)
                continue;

            if (Activator.CreateInstance(type) is IChangeDetectorMapping mappingInstance)
            {
                mappingInstance.Configure(this);
            }
            else
            {
                throw new InvalidOperationException($"Unable to create an instance of type {type.FullName}");
            }
        }
    }

}