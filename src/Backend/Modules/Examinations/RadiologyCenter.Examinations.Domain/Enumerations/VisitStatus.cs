using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Examinations.Domain.Enumerations;

public sealed class VisitStatus : Enumeration
{
    public static readonly VisitStatus CheckedIn = new(1, "CheckedIn");
    public static readonly VisitStatus Completed = new(2, "Completed");
    public static readonly VisitStatus Cancelled = new(3, "Cancelled");

    private VisitStatus(int value, string name) : base(value, name) { }
}
