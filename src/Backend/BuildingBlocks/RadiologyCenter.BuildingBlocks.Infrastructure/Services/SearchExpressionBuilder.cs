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

    private static readonly HashSet<Type> NumericTypes =
    [
        typeof(int), typeof(long), typeof(short), typeof(byte),
        typeof(decimal), typeof(double), typeof(float),
        typeof(int?), typeof(long?), typeof(short?), typeof(byte?),
        typeof(decimal?), typeof(double?), typeof(float?)
    ];

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

        var term = searchTerm.Trim();
        var param = Expression.Parameter(typeof(T), "e");
        Expression? combined = null;

        foreach (var field in targetFields)
        {
            var property = typeof(T).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property is null)
                continue;

            Expression? condition = BuildCondition(param, property, term);
            if (condition is not null)
                combined = combined is null ? condition : Expression.OrElse(combined, condition);
        }

        return combined is null ? null : Expression.Lambda<Func<T, bool>>(combined, param);
    }

    private static Expression? BuildCondition(ParameterExpression param, PropertyInfo property, string term)
    {
        var propType = property.PropertyType;
        var member = Expression.Property(param, property);

        if (propType == typeof(string))
            return BuildLike(member, $"%{EscapeLike(term)}%");

        if (NumericTypes.Contains(propType))
            return BuildNumericEquals(member, propType, term);

        return null;
    }

    private static string[] GetDefaultSearchFields<T>() =>
        typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.SetMethod is not null)
            .Where(p => p.PropertyType == typeof(string)
                     || NumericTypes.Contains(p.PropertyType))
            .Select(p => p.Name)
            .ToArray();

    private static Expression BuildLike(Expression member, string pattern) =>
        Expression.Call(LikeMethod, FunctionsExpression, member, Expression.Constant(pattern), Expression.Constant(@"\"));

    private static Expression? BuildNumericEquals(Expression member, Type propertyType, string term)
    {
        if (!TryConvertNumeric(term, propertyType, out var value))
            return null;

        return Expression.Equal(member, Expression.Constant(value, propertyType));
    }

    private static bool TryConvertNumeric(string term, Type targetType, out object? value)
    {
        value = null;
        var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

        try
        {
            value = underlying switch
            {
                var t when t == typeof(int) => int.Parse(term),
                var t when t == typeof(long) => long.Parse(term),
                var t when t == typeof(short) => short.Parse(term),
                var t when t == typeof(byte) => byte.Parse(term),
                var t when t == typeof(decimal) => decimal.Parse(term),
                var t when t == typeof(double) => double.Parse(term),
                var t when t == typeof(float) => float.Parse(term),
                _ => null
            };
            return value is not null;
        }
        catch
        {
            return false;
        }
    }

    private static string EscapeLike(string term) =>
        term.Replace(@"\", @"\\")
            .Replace("%", @"\%")
            .Replace("_", @"\_")
            .Replace("[", @"\[");
}
