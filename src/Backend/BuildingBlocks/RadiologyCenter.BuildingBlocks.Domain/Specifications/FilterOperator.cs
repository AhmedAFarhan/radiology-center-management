namespace RadiologyCenter.BuildingBlocks.Domain.Specifications;

public enum FilterOperator
{
    Equals,
    NotEquals,
    Contains,
    StartsWith,
    EndsWith,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Range,
    In,
    NotIn,
}
