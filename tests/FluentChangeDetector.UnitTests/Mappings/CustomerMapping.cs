using FluentChangeDetector.Configurations;
using FluentChangeDetector.Interfaces;
using FluentChangeDetector.UnitTests.Models;

namespace FluentChangeDetector.UnitTests.Mappings;

public class CustomerMapping : IChangeDetectorMapping
{
    public void Configure(ChangeDetectorConfig config)
    {
        config.CreateMapping<CustomerDto, CustomerEntity>()
            .Map(dto => dto.CustomerId, entity => entity.Id)
            .Map(dto => dto.Name, entity => entity.FullName);
    }
}