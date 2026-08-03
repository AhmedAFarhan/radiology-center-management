using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Payroll.Domain.Enumerations;

public sealed class ExamFeeRole : Enumeration
{
    public static readonly ExamFeeRole Radiologist = new(1, "Radiologist");
    public static readonly ExamFeeRole Technician = new(2, "Technician");

    private ExamFeeRole(int value, string name) : base(value, name) { }
}