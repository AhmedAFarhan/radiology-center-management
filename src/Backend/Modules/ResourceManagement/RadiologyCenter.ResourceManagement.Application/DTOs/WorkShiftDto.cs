namespace RadiologyCenter.ResourceManagement.Application.DTOs;

public record WorkShiftDto(
    Guid Id,
    Guid StaffId,
    Guid? EquipmentId,
    DateTime Date,
    TimeSpan StartTime,
    TimeSpan EndTime,
    string? Notes,
    DateTime CreatedAt);
