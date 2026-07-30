using System.Linq.Expressions;
using System.Reflection;
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
        var property = GetProperty<T>(filter.Field);
        if (property is null) return null;

        var left = Expression.Property(param, property);
        var leftType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        if (filter.Operator is FilterOperator.In or FilterOperator.NotIn)
            return BuildInExpression<T>(left, filter);

        object? convertedValue = ConvertValue(filter.Value, leftType);

        if (filter.Operator is FilterOperator.Range)
        {
            var secondValue = ConvertValue(filter.SecondValue, leftType);
            if (convertedValue is null || secondValue is null) return null;

            var greater = Expression.GreaterThan(
                left, Expression.Constant(convertedValue, property.PropertyType));
            var less = Expression.LessThan(
                left, Expression.Constant(secondValue, property.PropertyType));
            return Expression.AndAlso(greater, less);
        }

        return filter.Operator switch
        {
            FilterOperator.Equals => BuildEquality(left, convertedValue, property.PropertyType),
            FilterOperator.NotEquals => Expression.Not(BuildEquality(left, convertedValue, property.PropertyType)),
            FilterOperator.Contains => BuildStringMethod(left, convertedValue, nameof(string.Contains)),
            FilterOperator.StartsWith => BuildStringMethod(left, convertedValue, nameof(string.StartsWith)),
            FilterOperator.EndsWith => BuildStringMethod(left, convertedValue, nameof(string.EndsWith)),
            FilterOperator.GreaterThan => Expression.GreaterThan(left, Expression.Constant(convertedValue, property.PropertyType)),
            FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(left, Expression.Constant(convertedValue, property.PropertyType)),
            FilterOperator.LessThan => Expression.LessThan(left, Expression.Constant(convertedValue, property.PropertyType)),
            FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(left, Expression.Constant(convertedValue, property.PropertyType)),
            _ => null,
        };
    }

    private static Expression? BuildInExpression<T>(Expression left, FilterCriteria filter)
    {
        if (filter.Value is not IEnumerable<object> values) return null;
        if (!values.Any()) return null;

        var propertyType = left.Type;
        var convertedValues = values.Select(v => ConvertValue(v, propertyType));
        var containsMethod = typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name is nameof(Enumerable.Contains) && m.GetParameters().Length is 2)
            .MakeGenericMethod(propertyType);

        var constant = Expression.Constant(
            Activator.CreateInstance(typeof(List<>).MakeGenericType(propertyType), convertedValues),
            typeof(IEnumerable<>).MakeGenericType(propertyType));

        Expression contains = Expression.Call(containsMethod, constant, left);

        return filter.Operator is FilterOperator.NotIn
            ? Expression.Not(contains)
            : contains;
    }

    private static PropertyInfo? GetProperty<T>(string fieldName)
    {
        var type = typeof(T);
        foreach (var part in fieldName.Split('.'))
        {
            var prop = type.GetProperty(part, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop is null) return null;
            type = prop.PropertyType;
        }
        return type.GetProperty(fieldName.Split('.').Last(),
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
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

        if (underlying is { IsEnum: true })
            return Enum.Parse(underlying, value.ToString()!, ignoreCase: true);

        return Convert.ChangeType(value, underlying);
    }
}
