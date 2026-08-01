using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Examinations.Domain.Enumerations;

public sealed class ExaminationStatus : Enumeration
{
    public static readonly ExaminationStatus Requested = new(1, "Requested");
    public static readonly ExaminationStatus Scheduled = new(2, "Scheduled");
    public static readonly ExaminationStatus CheckedIn = new(3, "CheckedIn");
    public static readonly ExaminationStatus InProgress = new(4, "InProgress");
    public static readonly ExaminationStatus Completed = new(5, "Completed");
    public static readonly ExaminationStatus Cancelled = new(6, "Cancelled");

    private ExaminationStatus(int value, string name) : base(value, name) { }
}
