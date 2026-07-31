using System.Reflection;

namespace RadiologyCenter.BuildingBlocks.Application;

public static class ApplicationAssemblyRegistry
{
    private static readonly HashSet<Assembly> Assemblies = [];

    public static void Register(Assembly assembly) => Assemblies.Add(assembly);

    public static IReadOnlyCollection<Assembly> GetAll() => Assemblies.ToList();
}
