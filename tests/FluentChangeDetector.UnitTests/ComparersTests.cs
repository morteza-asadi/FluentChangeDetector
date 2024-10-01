using FluentChangeDetector.Comparers;
using FluentChangeDetector.Extensions;
using FluentChangeDetector.UnitTests.Models;

namespace FluentChangeDetector.UnitTests;

public class ComparersTests
{
    [Fact]
    public void EnumStringPropertyComparer_ComparesEnumWithString_CaseInsensitive()
    {
        // Arrange
        var source = new EntityWithEnum { Status = Status.Active };
        var target = new EntityWithStringStatus { Status = "active" };

        // Act
        var changes = source.DetectChanges(target)
            .ConfigureMapping(cfg => cfg
                .Map(s => s.Status, t => t.Status)
                .WithComparer(s => s.Status, new EnumStringPropertyComparer<Status>()))
            .Result;

        // Assert
        Assert.Empty(changes.Changes);
    }

    [Fact]
    public void EnumStringPropertyComparer_DetectsDifference()
    {
        // Arrange
        var source = new EntityWithEnum { Status = Status.Active };
        var target = new EntityWithStringStatus { Status = "inactive" };

        // Act
        var changes = source.DetectChanges(target)
            .ConfigureMapping(cfg => cfg
                .Map(s => s.Status, t => t.Status)
                .WithComparer(s => s.Status, new EnumStringPropertyComparer<Status>()))
            .Result;

        // Assert
        Assert.Single(changes.Changes);
        Assert.Contains(changes.Changes, c => c.PropertyName == nameof(EntityWithEnum.Status));
    }
}