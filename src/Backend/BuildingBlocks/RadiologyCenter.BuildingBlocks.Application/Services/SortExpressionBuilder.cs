using System.Linq.Expressions;
using System.Reflection;

namespace RadiologyCenter.BuildingBlocks.Application.Services;

public static class SortExpressionBuilder
{
    public static bool TryBuildSelector<T>(string? sortBy, out Expression<Func<T, object>> selector)
    {
        selector = null!;
        if (string.IsNullOrWhiteSpace(sortBy)) return false;

        var property = typeof(T).GetProperty(
            sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (property is null) return false;

        var param = Expression.Parameter(typeof(T), "e");
        selector = Expression.Lambda<Func<T, object>>(
            Expression.Convert(Expression.Property(param, property), typeof(object)), param);
        return true;
    }
}
