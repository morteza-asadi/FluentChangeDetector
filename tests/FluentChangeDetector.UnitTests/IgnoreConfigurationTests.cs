using FluentChangeDetector.Extensions;
using FluentChangeDetector.UnitTests.Models;

namespace FluentChangeDetector.UnitTests;

public class IgnoreConfigurationTests
{
    [Fact]
    public void Ignore_AllProperties_NoChangesDetected()
    {
        // Arrange
        var source = new TestEntity { Id = 1, Name = "Source", IsActive = true };
        var target = new TestEntity { Id = 2, Name = "Target", IsActive = false };

        // Act
        var changes = source.DetectChanges(target)
            .ConfigureMapping(cfg => cfg.IgnoreAll())
            .Result;

        // Assert
        Assert.Empty(changes.Changes);
    }

    [Fact]
    public void Ignore_SpecificProperties_ChangesDetectedForOthers()
    {
        // Arrange
        var source = new TestEntity { Id = 1, Name = "Source", IsActive = true };
        var target = new TestEntity { Id = 2, Name = "Target", IsActive = false };

        // Act
        var changes = source.DetectChanges(target)
            .ConfigureMapping(cfg => cfg
                .Ignore(e => e.Id)
                .Ignore(e => e.Name))
            .Result;

        // Assert
        Assert.Single(changes.Changes);
        Assert.Contains(changes.Changes, c => c.PropertyName == nameof(TestEntity.IsActive));
    }

    [Fact]
    public void Include_OverridesIgnoreAll_SpecifiedPropertiesCompared()
    {
        // Arrange
        var source = new TestEntity { Id = 1, Name = "Source", IsActive = true };
        var target = new TestEntity { Id = 2, Name = "Target", IsActive = false };

        // Act
        var changes = source.DetectChanges(target)
            .ConfigureMapping(cfg => cfg
                .IgnoreAll()
                .Include(e => e.Name))
            .Result;

        // Assert
        Assert.Single(changes.Changes);
        Assert.Contains(changes.Changes, c => c.PropertyName == nameof(TestEntity.Name));
    }
}