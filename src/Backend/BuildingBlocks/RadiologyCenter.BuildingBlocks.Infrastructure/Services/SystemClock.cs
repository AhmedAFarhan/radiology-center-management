using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Services;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
