using FluentChangeDetector.Configurations;

namespace FluentChangeDetector.Interfaces;

public interface IChangeDetectorMapping
{
    void Configure(ChangeDetectorConfig config);
}