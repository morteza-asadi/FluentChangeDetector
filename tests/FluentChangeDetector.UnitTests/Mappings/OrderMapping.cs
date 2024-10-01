using FluentChangeDetector.Configurations;
using FluentChangeDetector.Interfaces;
using FluentChangeDetector.UnitTests.Models;

namespace FluentChangeDetector.UnitTests.Mappings;

public class OrderMapping : IChangeDetectorMapping
{
    public void Configure(ChangeDetectorConfig config)
    {
        config.CreateMapping<OrderDto, OrderEntity>()
            .Map(dto => dto.OrderId, entity => entity.Id)
            .Map(dto => dto.CustomerName, entity => entity.Customer)
            .Map(dto => dto.TotalAmount, entity => entity.Amount);
    }
}