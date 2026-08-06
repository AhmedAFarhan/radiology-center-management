namespace RadiologyCenter.Examinations.Domain.ValueObjects;

public sealed record ItemSnapshot(Guid ItemId, string Name, int CategoryValue, decimal UnitCost);
