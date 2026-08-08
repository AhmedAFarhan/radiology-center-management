using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Reports.Domain.Enumerations;

public sealed class FindingSeverity : Enumeration
{
    public static readonly FindingSeverity None = new(1, "None");
    public static readonly FindingSeverity Mild = new(2, "Mild");
    public static readonly FindingSeverity Moderate = new(3, "Moderate");
    public static readonly FindingSeverity Severe = new(4, "Severe");
    public static readonly FindingSeverity Critical = new(5, "Critical");

    private FindingSeverity(int value, string name) : base(value, name) { }
}