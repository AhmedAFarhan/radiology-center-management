using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;
using RadiologyCenter.ResourceManagement.Domain.Errors;

namespace RadiologyCenter.ResourceManagement.Domain.Entities;

public sealed class Leave : SoftDeletableAggregateRoot<Guid>
{
    public Guid StaffId { get; private set; }
    public LeaveType LeaveType { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string? Reason { get; private set; }

    private Leave()
    {
        LeaveType = null!;
    }

    public static Leave Create(
        Guid staffId,
        LeaveType leaveType,
        DateTime startDate,
        DateTime endDate,
        string? reason = null)
    {
        Guard.AgainstEmpty(staffId, nameof(staffId));
        Guard.AgainstNull(leaveType, nameof(leaveType));
        Guard.AgainstDefault(startDate, nameof(startDate));
        Guard.AgainstDefault(endDate, nameof(endDate));
        Guard.Against(endDate, d => d < startDate, DomainErrors.EndDateBeforeStartDate, "End date cannot be before start date.");

        var leave = new Leave
        {
            Id = Guid.NewGuid(),
            StaffId = staffId,
            LeaveType = leaveType,
            StartDate = startDate,
            EndDate = endDate,
            Reason = reason?.Trim()
        };

        return leave;
    }

    public void Update(
        Guid staffId,
        LeaveType leaveType,
        DateTime startDate,
        DateTime endDate,
        string? reason = null)
    {
        Guard.AgainstEmpty(staffId, nameof(staffId));
        Guard.AgainstNull(leaveType, nameof(leaveType));
        Guard.AgainstDefault(startDate, nameof(startDate));
        Guard.AgainstDefault(endDate, nameof(endDate));
        Guard.Against(endDate, d => d < startDate, DomainErrors.EndDateBeforeStartDate, "End date cannot be before start date.");

        StaffId = staffId;
        LeaveType = leaveType;
        StartDate = startDate;
        EndDate = endDate;
        Reason = reason?.Trim();
    }
}
