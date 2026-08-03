namespace RadiologyCenter.ResourceManagement.Application.DTOs;

public record LeaveDto(
    Guid Id,
    Guid StaffId,
    string LeaveType,
    DateTime StartDate,
    DateTime EndDate,
    string? Reason,
    DateTime CreatedAt);
