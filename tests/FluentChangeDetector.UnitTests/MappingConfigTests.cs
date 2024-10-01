using FluentChangeDetector.Extensions;
using FluentChangeDetector.UnitTests.Comparers;
using FluentChangeDetector.UnitTests.Models;

namespace FluentChangeDetector.UnitTests;

public class MappingConfigTests
{
    [Fact]
    public void MappingConfig_IgnoreAllAndInclude_SpecificPropertiesCompared()
    {
        // Arrange
        var source = new TestEntity { Id = 1, Name = "Test", IsActive = true };
        var target = new TestEntity { Id = 1, Name = "Test Updated", IsActive = false };

        // Act
        var changes = source.DetectChanges(target)
            .ConfigureMapping(cfg => cfg
                .IgnoreAll()
                .Include(e => e.IsActive))
            .Result;

        // Assert
        Assert.Single(changes.Changes);
        Assert.Contains(changes.Changes, c => c.PropertyName == nameof(TestEntity.IsActive));
        Assert.DoesNotContain(changes.Changes, c => c.PropertyName == nameof(TestEntity.Name));
    }

    [Fact]
    public void MappingConfig_WithCustomComparer_CustomComparerUsed()
    {
        // Arrange
        var source = new TestEntity { Id = 1, Name = "test", IsActive = true };
        var target = new TestEntity { Id = 1, Name = "TEST", IsActive = true };

        // Act
        var changes = source.DetectChanges(target)
            .ConfigureMapping(cfg => cfg
                .WithComparer(e => e.Name, new CaseInsensitiveStringComparer()))
            .Result;

        // Assert
        Assert.Empty(changes.Changes);
    }

    [Fact]
    public void MappingConfig_MapPropertiesWithDifferentNames_PropertiesMappedCorrectly()
    {
        // Arrange
        var source = new TestEntity { Id = 1, Name = "Test", IsActive = true };
        var target = new TargetEntity { Id = 1, FullName = "Test", IsActive = true };

        // Act
        var changes = source.DetectChanges(target)
            .ConfigureMapping(cfg => cfg
                .Map(s => s.Name, t => t.FullName))
            .Result;

        // Assert
        Assert.Empty(changes.Changes);
    }

}