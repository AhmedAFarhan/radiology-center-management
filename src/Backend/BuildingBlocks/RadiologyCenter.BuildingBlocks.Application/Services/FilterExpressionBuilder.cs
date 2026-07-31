using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;

namespace RadiologyCenter.BuildingBlocks.Application.Services;

public static class FilterExpressionBuilder
{
    public static Expression<Func<T, bool>>? Build<T>(List<FilterCriteria>? filters)
    {
        if (filters is null || filters.Count is 0)
            return null;

        var param = Expression.Parameter(typeof(T), "e");
        Expression? combined = null;

        foreach (var filter in filters)
        {
            var condition = BuildCondition<T>(param, filter);
            if (condition is null) continue;

            combined = combined is null
                ? condition
                : filter.Logic is FilterLogic.Or
                    ? Expression.OrElse(combined, condition)
                    : Expression.AndAlso(combined, condition);
        }

        return combined is null ? null : Expression.Lambda<Func<T, bool>>(combined, param);
    }

    private static Expression? BuildCondition<T>(ParameterExpression param, FilterCriteria filter)
    {
        var segments = filter.Field.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (segments.Length is 0)
            return null;

        return BuildRecursive(param, typeof(T), segments, 0, filter);
    }

    private static Expression? BuildRecursive(
        Expression instance,
        Type currentType,
        string[] segments,
        int index,
        FilterCriteria filter)
    {
        var segment = segments[index];
        var property = currentType.GetProperty(segment, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (property is null)
            return null;

        var isLast = index == segments.Length - 1;
        var propertyType = property.PropertyType;

        if (!isLast && TryGetElementType(propertyType, out var elementType) && elementType is not null)
        {
            var anyParam = Expression.Parameter(elementType, "c");
            var inner = BuildRecursive(anyParam, elementType, segments, index + 1, filter);
            if (inner is null)
                return null;

            var anyMethod = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(m => m.Name is nameof(Enumerable.Any) && m.GetParameters().Length is 2)
                .MakeGenericMethod(elementType);

            return Expression.Call(
                anyMethod,
                Expression.Property(instance, property),
                Expression.Lambda(inner, anyParam));
        }

        var left = Expression.Property(instance, property);

        if (!isLast)
            return BuildRecursive(left, propertyType, segments, index + 1, filter);

        return ApplyOperator(left, propertyType, filter);
    }

    private static Expression? ApplyOperator(Expression left, Type propertyType, FilterCriteria filter)
    {
        var leftType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        if (filter.Operator is FilterOperator.In or FilterOperator.NotIn)
            return BuildInExpression(left, filter);

        object? convertedValue = ConvertValue(filter.Value, leftType);

        if (filter.Operator is FilterOperator.Range)
        {
            var secondValue = ConvertValue(filter.SecondValue, leftType);
            if (convertedValue is null || secondValue is null) return null;

            var greater = Expression.GreaterThan(
                left, Expression.Constant(convertedValue, propertyType));
            var less = Expression.LessThan(
                left, Expression.Constant(secondValue, propertyType));
            return Expression.AndAlso(greater, less);
        }

        return filter.Operator switch
        {
            FilterOperator.Equals => BuildEquality(left, convertedValue, propertyType),
            FilterOperator.NotEquals => Expression.Not(BuildEquality(left, convertedValue, propertyType)),
            FilterOperator.Contains => BuildStringMethod(left, convertedValue, nameof(string.Contains)),
            FilterOperator.StartsWith => BuildStringMethod(left, convertedValue, nameof(string.StartsWith)),
            FilterOperator.EndsWith => BuildStringMethod(left, convertedValue, nameof(string.EndsWith)),
            FilterOperator.GreaterThan => Expression.GreaterThan(left, Expression.Constant(convertedValue, propertyType)),
            FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(left, Expression.Constant(convertedValue, propertyType)),
            FilterOperator.LessThan => Expression.LessThan(left, Expression.Constant(convertedValue, propertyType)),
            FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(left, Expression.Constant(convertedValue, propertyType)),
            _ => null,
        };
    }

    private static Expression? BuildInExpression(Expression left, FilterCriteria filter)
    {
        IEnumerable<object>? values = filter.Value switch
        {
            IEnumerable<object> list => list,
            JsonElement { ValueKind: JsonValueKind.Array } element => element.EnumerateArray().Select(v => (object)v),
            _ => null,
        };

        if (values is null || !values.Any()) return null;

        var propertyType = left.Type;
        var convertedValues = values.Select(v => ConvertValue(v, propertyType)).ToArray();

        var cast = typeof(Enumerable)
            .GetMethod(nameof(Enumerable.Cast), BindingFlags.Static | BindingFlags.Public)!
            .MakeGenericMethod(propertyType);
        var typedValues = cast.Invoke(null, [convertedValues]);

        var listType = typeof(List<>).MakeGenericType(propertyType);
        var valueList = Activator.CreateInstance(listType, typedValues);

        var containsMethod = typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name is nameof(Enumerable.Contains) && m.GetParameters().Length is 2)
            .MakeGenericMethod(propertyType);

        var constant = Expression.Constant(valueList, listType);

        Expression contains = Expression.Call(containsMethod, constant, left);

        return filter.Operator is FilterOperator.NotIn
            ? Expression.Not(contains)
            : contains;
    }

    private static bool TryGetElementType(Type type, out Type? elementType)
    {
        elementType = null;
        if (type == typeof(string) || type == typeof(byte[]))
            return false;

        var enumerable = type.GetInterfaces()
            .Prepend(type)
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        if (enumerable is null)
            return false;

        elementType = enumerable.GetGenericArguments()[0];
        return true;
    }

    private static Expression BuildEquality(Expression left, object? value, Type propertyType)
    {
        var constant = Expression.Constant(value, propertyType);
        if (propertyType.IsValueType && Nullable.GetUnderlyingType(propertyType) is null)
            return Expression.Equal(left, constant);
        return Expression.Equal(left, Expression.Convert(constant, propertyType));
    }

    private static Expression? BuildStringMethod(Expression left, object? value, string methodName)
    {
        if (value is null) return null;
        var method = typeof(string).GetMethod(methodName, [typeof(string)]);
        if (method is null) return null;
        return Expression.Call(left, method, Expression.Constant(value));
    }

    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value is null) return null;
        if (targetType.IsInstanceOfType(value)) return value;

        var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlying != typeof(string) && typeof(Enumeration).IsAssignableFrom(underlying))
            return ConvertEnumerationValue(value, underlying);

        if (value is JsonElement { ValueKind: not JsonValueKind.Null and not JsonValueKind.Undefined } element)
        {
            if (underlying.IsEnum)
                return Enum.Parse(underlying, element.GetString() ?? string.Empty, ignoreCase: true);
            return element.Deserialize(underlying);
        }

        if (underlying is { IsEnum: true })
            return Enum.Parse(underlying, value.ToString()!, ignoreCase: true);

        return Convert.ChangeType(value, underlying);
    }

    private static object? ConvertEnumerationValue(object? value, Type enumerationType)
    {
        string raw;
        if (value is JsonElement element)
            raw = element.ValueKind is JsonValueKind.Number ? element.GetRawText() : element.GetString() ?? string.Empty;
        else
            raw = value?.ToString() ?? string.Empty;

        if (int.TryParse(raw, out var numeric))
        {
            var fromValue = typeof(Enumeration)
                .GetMethod(nameof(Enumeration.FromValue), BindingFlags.Public | BindingFlags.Static)!
                .MakeGenericMethod(enumerationType);
            return fromValue.Invoke(null, [numeric]);
        }

        var fromName = typeof(Enumeration)
            .GetMethod(nameof(Enumeration.FromName), BindingFlags.Public | BindingFlags.Static)!
            .MakeGenericMethod(enumerationType);
        return fromName.Invoke(null, [raw]);
    }
}
