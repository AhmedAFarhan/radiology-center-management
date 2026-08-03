using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Payroll.Domain.Enumerations;

public sealed class Frequency : Enumeration
{
    public static readonly Frequency OneTime = new(1, "OneTime");
    public static readonly Frequency Monthly = new(2, "Monthly");
    public static readonly Frequency Quarterly = new(3, "Quarterly");
    public static readonly Frequency Annual = new(4, "Annual");

    private Frequency(int value, string name) : base(value, name) { }
}