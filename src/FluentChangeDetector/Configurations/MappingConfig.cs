using System.Linq.Expressions;
using System.Reflection;
using FluentChangeDetector.Interfaces;
using FluentChangeDetector.Models;

namespace FluentChangeDetector.Configurations;

 public class MappingConfig
    {
        protected readonly Dictionary<string, string> CustomMappings = new();
        protected readonly HashSet<string> IncludedProperties = [];
        protected readonly HashSet<string> ExcludedProperties = [];
        protected readonly Dictionary<string, IPropertyComparer> PropertyComparers = new();

        internal Type SourceType { get; set; } = null!;
        internal Type TargetType { get; set; } = null!;

        internal virtual List<PropertyMapping> GetPropertyMappings()
        {
            var sourceProperties = SourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var targetProperties = TargetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var mappings = new List<PropertyMapping>();

            foreach (var sourceProp in sourceProperties)
            {
                if (ExcludedProperties.Contains(sourceProp.Name))
                    continue;

                if (IncludedProperties.Count != 0 && !IncludedProperties.Contains(sourceProp.Name))
                    continue;

                var targetPropName = CustomMappings.TryGetValue(sourceProp.Name, out var customMapping)
                    ? customMapping
                    : sourceProp.Name;

                var targetProp = targetProperties.FirstOrDefault(p => p.Name == targetPropName);

                if (targetProp == null)
                    continue;

                var sourceGetter = CreatePropertyGetter(SourceType, sourceProp);
                var targetGetter = CreatePropertyGetter(TargetType, targetProp);

                var mapping = new PropertyMapping
                {
                    SourceProperty = sourceProp,
                    TargetProperty = targetProp,
                    SourceGetter = sourceGetter,
                    TargetGetter = targetGetter,
                    Comparer = PropertyComparers.GetValueOrDefault(sourceProp.Name)
                };

                mappings.Add(mapping);
            }

            return mappings;
        }

        private static Func<object, object> CreatePropertyGetter(Type type, PropertyInfo propertyInfo)
        {
            var param = Expression.Parameter(typeof(object), "instance");
            var castInstance = Expression.Convert(param, type);
            var propertyAccess = Expression.Property(castInstance, propertyInfo);
            var castProperty = Expression.Convert(propertyAccess, typeof(object));
            var lambda = Expression.Lambda<Func<object, object>>(castProperty, param);
            return lambda.Compile();
        }
    }