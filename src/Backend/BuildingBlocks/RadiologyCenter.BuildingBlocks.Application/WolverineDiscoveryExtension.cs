using Wolverine;

namespace RadiologyCenter.BuildingBlocks.Application;

public sealed class WolverineDiscoveryExtension : IWolverineExtension
{
    public void Configure(WolverineOptions options)
    {
        foreach (var assembly in ApplicationAssemblyRegistry.GetAll())
            options.Discovery.IncludeAssembly(assembly);
    }
}
