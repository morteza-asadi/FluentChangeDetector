using System.Reflection;
using FluentChangeDetector.Configurations;
using FluentChangeDetector.UnitTests.Models;

namespace FluentChangeDetector.UnitTests;

public class AddMappingsFromAssemblyTests
{
    [Fact]
    public void DetectChanges_WithMappingsFromAssembly_MappingsAppliedCorrectly()
    {
        // Arrange
        var source = new OrderDto { OrderId = 1, CustomerName = "Alice Johnson", TotalAmount = 300.00m };
        var target = new OrderEntity { Id = 1, Customer = "Alice Johnson", Amount = 350.00m };

        var config = new ChangeDetectorConfig();

        // Load mappings from the assembly containing mapping classes
        config.AddMappingsFromAssembly(Assembly.GetExecutingAssembly());

        var changeDetector = new ChangeDetector(config);

        // Act
        var changes = changeDetector.DetectChanges(source, target);

        // Assert
        Assert.Single(changes.Changes);
        var change = changes.Changes[0];
        Assert.Equal(nameof(OrderDto.TotalAmount), change.PropertyName);
        Assert.Equal(300.00m, change.SourceValue);
        Assert.Equal(350.00m, change.TargetValue);
    }

    [Fact]
    public void DetectChanges_WithoutMappingsFromAssembly_NoMappingsApplied()
    {
        // Arrange
        var source = new OrderDto { OrderId = 2, CustomerName = "Bob Smith", TotalAmount = 400.00m };
        var target = new OrderEntity { Id = 2, Customer = "Bob Smith", Amount = 400.00m };

        var config = new ChangeDetectorConfig();
        // Not loading mappings from assembly

        var changeDetector = new ChangeDetector(config);

        // Act
        var changes = changeDetector.DetectChanges(source, target);

        // Assert
        // Since mappings are not loaded, no changes should be detected
        Assert.Empty(changes.Changes);
    }
    
    [Fact]
    public void DetectChanges_MultipleMappingsFromAssembly_AllMappingsApplied()
    {
        // Arrange
        var orderDto = new OrderDto { OrderId = 1, CustomerName = "Charlie Brown", TotalAmount = 500.00m };
        var orderEntity = new OrderEntity { Id = 1, Customer = "Charlie Brown", Amount = 600.00m };

        var customerDto = new CustomerDto { CustomerId = 10, Name = "Charlie Brown" };
        var customerEntity = new CustomerEntity { Id = 10, FullName = "Charles Brown" };

        var config = new ChangeDetectorConfig();

        // Load mappings from the assembly containing mapping classes
        config.AddMappingsFromAssembly(Assembly.GetExecutingAssembly());

        var changeDetector = new ChangeDetector(config);

        // Act
        var orderChanges = changeDetector.DetectChanges(orderDto, orderEntity);
        var customerChanges = changeDetector.DetectChanges(customerDto, customerEntity);

        // Assert
        // Order Changes
        Assert.Single(orderChanges.Changes);
        var orderChange = orderChanges.Changes[0];
        Assert.Equal(nameof(OrderDto.TotalAmount), orderChange.PropertyName);
        Assert.Equal(500.00m, orderChange.SourceValue);
        Assert.Equal(600.00m, orderChange.TargetValue);

        // Customer Changes
        Assert.Single(customerChanges.Changes);
        var customerChange = customerChanges.Changes[0];
        Assert.Equal(nameof(CustomerDto.Name), customerChange.PropertyName);
        Assert.Equal("Charlie Brown", customerChange.SourceValue);
        Assert.Equal("Charles Brown", customerChange.TargetValue);
    }
}