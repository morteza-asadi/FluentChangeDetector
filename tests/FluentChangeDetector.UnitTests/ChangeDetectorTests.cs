using FluentChangeDetector.Extensions;
using FluentChangeDetector.UnitTests.Models;

namespace FluentChangeDetector.UnitTests;

public class ChangeDetectorTests
{
    [Fact]
    public void DetectChanges_SimilarObjects_NoChangesDetected()
    {
        // Arrange
        var source = new TestEntity { Id = 1, Name = "Test", IsActive = true };
        var target = new TestEntity { Id = 1, Name = "Test", IsActive = true };

        // Act
        var changes = source.DetectChanges(target).Result;

        // Assert
        Assert.Empty(changes.Changes);
    }
    
    [Fact]
    public void DetectChanges_DifferentObjects_ChangesDetected()
    {
        // Arrange
        var source = new TestEntity { Id = 1, Name = "Test", IsActive = true };
        var target = new TestEntity { Id = 1, Name = "Test Updated", IsActive = false };

        // Act
        var changes = source.DetectChanges(target).Result;

        // Assert
        Assert.Equal(2, changes.Changes.Count);
        Assert.Contains(changes.Changes, c => c.PropertyName == nameof(TestEntity.Name));
        Assert.Contains(changes.Changes, c => c.PropertyName == nameof(TestEntity.IsActive));
    }
    
    [Fact]
    public void DetectChanges_WithIgnoredProperties_IgnoresSpecifiedProperties()
    {
        // Arrange
        var source = new TestEntity { Id = 1, Name = "Test", IsActive = true };
        var target = new TestEntity { Id = 1, Name = "Test Updated", IsActive = false };

        // Act
        var changes = source.DetectChanges(target)
            .ConfigureMapping(cfg => cfg.Ignore(e => e.Name))
            .Result;

        // Assert
        Assert.Single(changes.Changes);
        Assert.Contains(changes.Changes, c => c.PropertyName == nameof(TestEntity.IsActive));
        Assert.DoesNotContain(changes.Changes, c => c.PropertyName == nameof(TestEntity.Name));
    }
    
    [Fact]
    public void DetectChanges_WithIncludedProperties_IncludesOnlySpecifiedProperties()
    {
        // Arrange
        var source = new TestEntity { Id = 1, Name = "Test", IsActive = true };
        var target = new TestEntity { Id = 1, Name = "Test Updated", IsActive = false };

        // Act
        var changes = source.DetectChanges(target)
            .ConfigureMapping(cfg => cfg
                .IgnoreAll()
                .Include(e => e.Name))
            .Result;

        // Assert
        Assert.Single(changes.Changes);
        Assert.Contains(changes.Changes, c => c.PropertyName == nameof(TestEntity.Name));
        Assert.DoesNotContain(changes.Changes, c => c.PropertyName == nameof(TestEntity.IsActive));
    }
    
    [Fact]
    public void DetectChanges_WithCustomMapping_MapsPropertiesCorrectly()
    {
        // Arrange
        var source = new TestEntity { Id = 1, Name = "Test", IsActive = true };
        var target = new TargetEntity { Id = 1, FullName = "Test Updated", IsActive = false };

        // Act
        var changes = source.DetectChanges(target)
            .ConfigureMapping(cfg => cfg
                .Map(s => s.Name, t => t.FullName))
            .Result;

        // Assert
        Assert.Equal(2, changes.Changes.Count);
        Assert.Contains(changes.Changes, c => c.PropertyName == nameof(TestEntity.Name));
        Assert.Contains(changes.Changes, c => c.PropertyName == nameof(TestEntity.IsActive));
    }
    
    [Fact]
    public void DetectChanges_NullSource_ThrowsArgumentNullException()
    {
        // Arrange
        TestEntity? source = null;
        var target = new TestEntity { Id = 1, Name = "Test", IsActive = true };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => source!.DetectChanges(target).Result);
    }
    
    [Fact]
    public void DetectChanges_NullTarget_ThrowsArgumentNullException()
    {
        // Arrange
        var source = new TestEntity { Id = 1, Name = "Test", IsActive = true };
        TestEntity? target = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => source.DetectChanges(target!).Result);
    }
    
    [Fact]
    public void DetectChanges_NullPropertyValues_ComparedCorrectly()
    {
        // Arrange
        var source = new EntityWithNullableProperty { Id = 1, Description = null };
        var target = new EntityWithNullableProperty { Id = 1, Description = "Test" };

        // Act
        var changes = source.DetectChanges(target).Result;

        // Assert
        Assert.Single(changes.Changes);
        Assert.Contains(changes.Changes, c => c.PropertyName == nameof(EntityWithNullableProperty.Description));
    }
    
    [Fact]
    public void DetectChanges_EmptyObjects_NoChangesDetected()
    {
        // Arrange
        var source = new TestEntity();
        var target = new TestEntity();

        // Act
        var changes = source.DetectChanges(target).Result;

        // Assert
        Assert.Empty(changes.Changes);
    }
    
    [Fact]
    public void DetectChanges_Collections_ComparedCorrectly()
    {
        // Arrange
        var source = new EntityWithCollection { Id = 1, Items = ["A", "B"] };
        var target = new EntityWithCollection { Id = 1, Items = ["A", "C"] };

        // Act
        var changes = source.DetectChanges(target).Result;

        // Assert
        Assert.Single(changes.Changes);
        Assert.Contains(changes.Changes, c => c.PropertyName == nameof(EntityWithCollection.Items));
    }
    
    [Fact]
    public void DefaultEquals_BothValuesNull_ReturnsTrue()
    {
        // Arrange
        var source = new EntityWithNullableProperty { Id = 1, Description = null };
        var target = new EntityWithNullableProperty { Id = 1, Description = null };

        var changeDetector = new ChangeDetector();

        // Act
        var result = changeDetector.DetectChanges(source, target);

        // Assert
        Assert.False(result.HasChanges);
    }

    [Fact]
    public void DefaultEquals_OneValueNull_ReturnsFalse()
    {
        // Arrange
        var source = new EntityWithNullableProperty { Id = 1, Description = "Test" };
        var target = new EntityWithNullableProperty { Id = 1, Description = null };

        var changeDetector = new ChangeDetector();

        // Act
        var result = changeDetector.DetectChanges(source, target);

        // Assert
        Assert.True(result.HasChanges);
        Assert.Single(result.Changes);
        var change = result.Changes[0];
        Assert.Equal(nameof(EntityWithNullableProperty.Description), change.PropertyName);
        Assert.Equal("Test", change.SourceValue);
        Assert.Null(change.TargetValue);
    }
}