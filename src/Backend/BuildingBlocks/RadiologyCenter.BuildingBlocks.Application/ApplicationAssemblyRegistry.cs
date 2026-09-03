using System.Collections.Concurrent;
using System.Reflection;

namespace RadiologyCenter.BuildingBlocks.Application;

public static class ApplicationAssemblyRegistry
{
    private static readonly ConcurrentDictionary<Assembly, byte> Assemblies = new();

    public static void Register(Assembly assembly) => Assemblies.TryAdd(assembly, 0);

    public static IReadOnlyCollection<Assembly> GetAll() => Assemblies.Keys.ToList();
}
