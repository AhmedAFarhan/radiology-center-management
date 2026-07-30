using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Services;

public class CurrentUserService : ICurrentUser
{
    public string? Id => null;
    public string? Name => null;
    public bool IsAuthenticated => false;
}
