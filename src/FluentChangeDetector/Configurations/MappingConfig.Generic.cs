using System.Linq.Expressions;
using System.Reflection;
using FluentChangeDetector.Comparers;
using FluentChangeDetector.Interfaces;
using FluentChangeDetector.Models;

namespace FluentChangeDetector.Configurations;

public class MappingConfig<TSource, TTarget> : MappingConfig
{
    private readonly ChangeDetectorConfig _changeDetectorConfig;
    private bool _ignoreAllProperties;
    
    public MappingConfig(ChangeDetectorConfig changeDetectorConfig)
    {
        SourceType = typeof(TSource);
        TargetType = typeof(TTarget);
        _changeDetectorConfig = changeDetectorConfig;
    }
    
    public MappingConfig<TSource, TTarget> Map<TSourceProperty, TTargetProperty>(
        Expression<Func<TSource, TSourceProperty>> sourceExpression,
        Expression<Func<TTarget, TTargetProperty>> targetExpression)
    {
        var sourcePropertyName = GetPropertyName(sourceExpression);
        var targetPropertyName = GetPropertyName(targetExpression);

        CustomMappings[sourcePropertyName] = targetPropertyName;
        return this;
    }
    
    public MappingConfig<TSource, TTarget> Include<TProperty>(Expression<Func<TSource, TProperty>> expression)
    {
        var propertyName = GetPropertyName(expression);
        IncludedProperties.Add(propertyName);
        return this;
    }
    
    public MappingConfig<TSource, TTarget> Ignore<TProperty>(Expression<Func<TSource, TProperty>> expression)
    {
        var propertyName = GetPropertyName(expression);
        ExcludedProperties.Add(propertyName);
        return this;
    }
    
    public MappingConfig<TSource, TTarget> IgnoreAll()
    {
        _ignoreAllProperties = true;
        return this;
    }
    
    public MappingConfig<TSource, TTarget> WithComparer<TProperty>(
        Expression<Func<TSource, TProperty>> sourceExpression,
        IEqualityComparer<TProperty> comparer)
    {
        var sourcePropertyName = GetPropertyName(sourceExpression);
        PropertyComparers[sourcePropertyName] = new PropertyComparer<TProperty>(comparer);
        return this;
    }
    
    public MappingConfig<TSource, TTarget> WithComparer(
        Expression<Func<TSource, object>> sourceExpression,
        IPropertyComparer comparer)
    {
        var sourcePropertyName = GetPropertyName(sourceExpression);
        PropertyComparers[sourcePropertyName] = comparer;
        return this;
    }
    
    public MappingConfig<TSource, TTarget> WithComparerForType<TProperty>(IEqualityComparer<TProperty> comparer)
    {
        _changeDetectorConfig.AddTypeComparer(comparer);
        return this;
    }
    
    private static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> expression)
    {
        return expression.Body switch
        {
            MemberExpression member => member.Member.Name,
            UnaryExpression { Operand: MemberExpression memberOperand } => memberOperand.Member.Name,
            _ => throw new ArgumentException("Invalid expression")
        };
    }
    
    internal override List<PropertyMapping> GetPropertyMappings()
        {
            var sourceProperties = SourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var targetProperties = TargetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var mappings = new List<PropertyMapping>();

            foreach (var sourceProp in sourceProperties)
            {
                if (ExcludedProperties.Contains(sourceProp.Name))
                    continue;

                switch (_ignoreAllProperties)
                {
                    case true when !IncludedProperties.Contains(sourceProp.Name):
                    case false when IncludedProperties.Count != 0 && !IncludedProperties.Contains(sourceProp.Name):
                        continue;
                }

                var targetPropName = CustomMappings.TryGetValue(sourceProp.Name, out var customMapping)
                    ? customMapping
                    : sourceProp.Name;

                var targetProp = targetProperties.FirstOrDefault(p => p.Name == targetPropName);

                if (targetProp == null)
                    continue;

                var sourceGetter = MappingConfig<TSource, TTarget>.CreatePropertyGetter(SourceType, sourceProp);
                var targetGetter = MappingConfig<TSource, TTarget>.CreatePropertyGetter(TargetType, targetProp);

                PropertyComparers.TryGetValue(sourceProp.Name, out var comparer);

                var mapping = new PropertyMapping
                {
                    SourceProperty = sourceProp,
                    TargetProperty = targetProp,
                    SourceGetter = sourceGetter,
                    TargetGetter = targetGetter,
                    Comparer = comparer
                };

                mappings.Add(mapping);
            }

            return mappings;
        }
    
    private static Func<object, object?> CreatePropertyGetter(Type type, PropertyInfo propertyInfo)
    {
        var param = Expression.Parameter(typeof(object), "instance");
        var castInstance = Expression.Convert(param, type);
        var propertyAccess = Expression.Property(castInstance, propertyInfo);
        var castProperty = Expression.Convert(propertyAccess, typeof(object));
        var lambda = Expression.Lambda<Func<object, object>>(castProperty, param);
        return lambda.Compile();
    }
}