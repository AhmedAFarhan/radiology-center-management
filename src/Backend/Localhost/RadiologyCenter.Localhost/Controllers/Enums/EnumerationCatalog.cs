using System.Reflection;
using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Localhost.Controllers.Enums;

/// <summary>
/// Resolves <see cref="Enumeration"/> types by their simple type name (e.g. "Modality"),
/// so the enums endpoint works for any registered enum without a hardcoded whitelist.
/// </summary>
internal static class EnumerationCatalog
{
    private static readonly Lazy<IReadOnlyDictionary<string, Type>> TypesLoader = new(Build);

    public static IReadOnlyDictionary<string, Type> Types => TypesLoader.Value;

    private static Dictionary<string, Type> Build()
    {
        var result = new Dictionary<string, Type>(StringComparer.Ordinal);

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t is not null).Cast<Type>().ToArray();
            }
            catch
            {
                continue;
            }

            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsGenericTypeDefinition || type == typeof(Enumeration))
                    continue;
                if (!typeof(Enumeration).IsAssignableFrom(type))
                    continue;

                result.TryAdd(type.Name, type);
            }
        }

        return result;
    }
}