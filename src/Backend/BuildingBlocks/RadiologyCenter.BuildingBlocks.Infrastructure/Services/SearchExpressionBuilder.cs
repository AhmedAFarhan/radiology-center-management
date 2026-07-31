using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Services;

public static class SearchExpressionBuilder
{
    private static readonly MethodInfo LikeMethod = typeof(DbFunctionsExtensions)
        .GetMethod(nameof(DbFunctionsExtensions.Like), [typeof(DbFunctions), typeof(string), typeof(string), typeof(string)])!;

    private static readonly Expression FunctionsExpression = Expression.Property(
        null, typeof(EF).GetProperty(nameof(EF.Functions))!);

    public static Expression<Func<T, bool>>? Build<T>(string? searchTerm, IEnumerable<string>? fields)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return null;

        var targetFields = (fields ?? [])
            .Where(f => !string.IsNullOrWhiteSpace(f))
            .ToArray();

        if (targetFields.Length is 0)
            targetFields = GetDefaultSearchFields<T>();

        if (targetFields.Length is 0)
            return null;

        var pattern = $"%{EscapeLike(searchTerm.Trim())}%";
        var param = Expression.Parameter(typeof(T), "e");
        Expression? combined = null;

        foreach (var field in targetFields)
        {
            var property = typeof(T).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property is null || property.PropertyType != typeof(string))
                continue;

            var condition = BuildLike(Expression.Property(param, property), pattern);
            combined = combined is null ? condition : Expression.OrElse(combined, condition);
        }

        return combined is null ? null : Expression.Lambda<Func<T, bool>>(combined, param);
    }

    private static string[] GetDefaultSearchFields<T>() =>
        typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(string) && p.SetMethod is not null)
            .Select(p => p.Name)
            .ToArray();

    private static Expression BuildLike(Expression member, string pattern) =>
        Expression.Call(LikeMethod, FunctionsExpression, member, Expression.Constant(pattern), Expression.Constant(@"\"));

    private static string EscapeLike(string term) =>
        term.Replace(@"\", @"\\")
            .Replace("%", @"\%")
            .Replace("_", @"\_")
            .Replace("[", @"\[");
}
