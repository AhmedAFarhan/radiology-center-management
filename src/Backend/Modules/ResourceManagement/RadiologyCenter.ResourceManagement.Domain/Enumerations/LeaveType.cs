using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.ResourceManagement.Domain.Enumerations;

public sealed class LeaveType : Enumeration
{
    public static readonly LeaveType Annual = new(1, "Annual");
    public static readonly LeaveType Sick = new(2, "Sick");
    public static readonly LeaveType Unpaid = new(3, "Unpaid");
    public static readonly LeaveType Maternity = new(4, "Maternity");
    public static readonly LeaveType Other = new(5, "Other");

    private LeaveType(int value, string name) : base(value, name) { }
}
