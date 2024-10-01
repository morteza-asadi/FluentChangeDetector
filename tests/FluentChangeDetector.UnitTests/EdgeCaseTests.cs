using FluentChangeDetector.Configurations;
using FluentChangeDetector.Extensions;
using FluentChangeDetector.UnitTests.Comparers;
using FluentChangeDetector.UnitTests.Models;

namespace FluentChangeDetector.UnitTests;

public class EdgeCaseTests
{
    [Fact]
    public void ChangeDetectorConfig_WithTypeComparer_TypeComparerUsed()
    {
        // Arrange
        var source = new TestEntity { Id = 1, Name = "Test  ", IsActive = true };
        var target = new TestEntity { Id = 1, Name = "Test", IsActive = true };

        var config = new ChangeDetectorConfig();
        config.AddTypeComparer<string>(new TrimStringComparer());
        var changeDetector = new ChangeDetector(config);

        // Act
        var changes = changeDetector.DetectChanges(source, target);

        // Assert
        Assert.Empty(changes.Changes);
    }

    [Fact]
    public void DetectChanges_DifferentPropertyTypes_UsesDefaultComparer()
    {
        // Arrange
        var source = new EntityWithInt { Value = 10 };
        var target = new EntityWithStringValue { Value = "10" };

        // Act
        var changes = source.DetectChanges(target)
            .ConfigureMapping(cfg => cfg.Map(s => s.Value, t => t.Value))
            .Result;

        // Assert
        Assert.Single(changes.Changes);
        Assert.Contains(changes.Changes, c => c.PropertyName == nameof(EntityWithInt.Value));
    }

    [Fact]
    public void DetectChanges_DifferentPropertyTypesWithComparer_CustomComparerUsed()
    {
        // Arrange
        var source = new EntityWithInt { Value = 10 };
        var target = new EntityWithStringValue { Value = "10" };

        // Act
        var changes = source.DetectChanges(target)
            .ConfigureMapping(cfg => cfg
                .Map(s => s.Value, t => t.Value)
                .WithComparer(s => s.Value, new IntStringComparer()))
            .Result;

        // Assert
        Assert.Empty(changes.Changes);
    }
}
