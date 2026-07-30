namespace RadiologyCenter.BuildingBlocks.Application.Abstractions;

public interface IClock
{
    DateTime UtcNow { get; }
}
