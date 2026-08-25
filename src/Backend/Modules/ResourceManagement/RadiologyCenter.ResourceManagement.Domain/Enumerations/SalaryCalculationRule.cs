using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.ResourceManagement.Domain.Enumerations;

public sealed class SalaryCalculationRule : Enumeration
{
    public static readonly SalaryCalculationRule FixedPlusFees = new(1, "FixedPlusFees");
    public static readonly SalaryCalculationRule HigherOfFixedOrFees = new(2, "HigherOfFixedOrFees");

    private SalaryCalculationRule(int value, string name) : base(value, name) { }
}
