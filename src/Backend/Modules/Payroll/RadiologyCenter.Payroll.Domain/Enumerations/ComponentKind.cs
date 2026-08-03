using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Payroll.Domain.Enumerations;

public sealed class ComponentKind : Enumeration
{
    public static readonly ComponentKind Earning = new(1, "Earning");
    public static readonly ComponentKind Deduction = new(2, "Deduction");

    private ComponentKind(int value, string name) : base(value, name) { }
}
