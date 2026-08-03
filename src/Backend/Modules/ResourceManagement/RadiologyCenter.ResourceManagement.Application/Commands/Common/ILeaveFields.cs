namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public interface ILeaveFields
{
    Guid StaffId { get; }
    string LeaveType { get; }
    DateTime StartDate { get; }
    DateTime EndDate { get; }
    string? Reason { get; }
}
