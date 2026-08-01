using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Examinations.Domain.Enumerations;

public sealed class ExaminationPriority : Enumeration
{
    public static readonly ExaminationPriority Routine = new(1, "Routine");
    public static readonly ExaminationPriority Urgent = new(2, "Urgent");
    public static readonly ExaminationPriority Stat = new(3, "Stat");

    private ExaminationPriority(int value, string name) : base(value, name) { }
}
