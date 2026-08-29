using Wolverine;
using RadiologyCenter.BuildingBlocks.Application;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Messaging;

public sealed class WolverineDiscoveryExtension : IWolverineExtension
{
    public void Configure(WolverineOptions options)
    {
        foreach (var assembly in ApplicationAssemblyRegistry.GetAll())
            options.Discovery.IncludeAssembly(assembly);
    }
}
