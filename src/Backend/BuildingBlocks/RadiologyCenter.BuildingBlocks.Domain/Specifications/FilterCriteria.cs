namespace RadiologyCenter.BuildingBlocks.Domain.Specifications;

public class FilterCriteria
{
    public string Field { get; init; } = string.Empty;
    public FilterOperator Operator { get; init; }
    public object? Value { get; init; }
    public object? SecondValue { get; init; }
    public FilterLogic Logic { get; init; } = FilterLogic.And;
}
