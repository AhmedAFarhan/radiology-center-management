using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Payroll.Domain.Enumerations;

public sealed class SalaryType : Enumeration
{
    public static readonly SalaryType Monthly = new(1, "Monthly");
    public static readonly SalaryType Hourly = new(2, "Hourly");

    private SalaryType(int value, string name) : base(value, name) { }
}
