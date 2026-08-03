using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Payroll.Domain.Enumerations;

public sealed class PayRunStatus : Enumeration
{
    public static readonly PayRunStatus Draft = new(1, "Draft");
    public static readonly PayRunStatus Computed = new(2, "Computed");
    public static readonly PayRunStatus Approved = new(3, "Approved");
    public static readonly PayRunStatus Paid = new(4, "Paid");
    public static readonly PayRunStatus Rejected = new(5, "Rejected");

    private PayRunStatus(int value, string name) : base(value, name) { }
}
