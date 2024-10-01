using FluentChangeDetector.Extensions;
using FluentChangeDetector.UnitTests.Models;

namespace FluentChangeDetector.UnitTests;

public class MappingConfigurationTests
{
    [Fact]
    public void Map_DifferentPropertyNames_PropertiesMappedAndCompared()
    {
        // Arrange
        var source = new TestEntity { Id = 1, Name = "Source Name", IsActive = true };
        var target = new TargetEntity { Id = 1, FullName = "Source Name", IsActive = true };

        // Act
        var changes = source.DetectChanges(target)
            .ConfigureMapping(cfg => cfg.Map(s => s.Name, t => t.FullName))
            .Result;

        // Assert
        Assert.Empty(changes.Changes);
    }

    [Fact]
    public void Map_WithoutConfiguration_PropertiesNotMapped()
    {
        // Arrange
        var source = new TestEntity { Id = 1, Name = "Source Name", IsActive = true };
        var target = new TargetEntity { Id = 1, FullName = "Target Name", IsActive = true };

        // Act
        var changes = source.DetectChanges(target).Result;

        // Assert
        // Since 'Name' and 'FullName' are not mapped, they won't be compared
        Assert.Empty(changes.Changes);
    }

    [Fact]
    public void Map_MultipleProperties_PropertiesMappedAndCompared()
    {
        // Arrange
        var source = new TestEntity { Id = 1, Name = "Source Name", IsActive = true };
        var target = new TargetEntity { Id = 2, FullName = "Source Name", IsActive = false };

        // Act
        var changes = source.DetectChanges(target)
            .ConfigureMapping(cfg => cfg
                .Map(s => s.Name, t => t.FullName)
                .Map(s => s.Id, t => t.Id))
            .Result;

        // Assert
        Assert.Equal(2, changes.Changes.Count);
        Assert.Contains(changes.Changes, c => c.PropertyName == nameof(TestEntity.Id));
        Assert.Contains(changes.Changes, c => c.PropertyName == nameof(TestEntity.IsActive));
    }
}