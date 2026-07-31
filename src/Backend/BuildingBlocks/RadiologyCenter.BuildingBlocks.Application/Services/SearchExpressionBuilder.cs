using System.Linq.Expressions;
using System.Reflection;

namespace RadiologyCenter.BuildingBlocks.Application.Services;

public static class SearchExpressionBuilder
{
    public static Expression<Func<T, bool>>? Build<T>(string? searchTerm, IEnumerable<string>? fields)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return null;

        var normalized = searchTerm.Trim();
        var param = Expression.Parameter(typeof(T), "e");
        var contains = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
        Expression? combined = null;

        foreach (var field in fields ?? [])
        {
            var property = typeof(T).GetProperty(
                field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property is null || property.PropertyType != typeof(string)) continue;

            var condition = Expression.Call(
                Expression.Property(param, property), contains, Expression.Constant(normalized));

            combined = combined is null ? condition : Expression.OrElse(combined, condition);
        }

        return combined is null ? null : Expression.Lambda<Func<T, bool>>(combined, param);
    }
}
