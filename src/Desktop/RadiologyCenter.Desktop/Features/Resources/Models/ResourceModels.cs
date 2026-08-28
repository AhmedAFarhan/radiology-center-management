namespace RadiologyCenter.Desktop.Features.Resources.Models;

public sealed record EquipmentDto(
    string Id,
    string Name,
    string? SerialNumber,
    string Modality,
    string Status,
    DateTime? PurchaseDate,
    bool IsActive,
    DateTime CreatedAt,
    string ModalityKey = "",
    string StatusKey = "");

public sealed class EquipmentInput
{
    public string Name { get; set; } = string.Empty;
    public string Modality { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public DateTime? PurchaseDate { get; set; }
}

public sealed record StaffDto(
    string Id,
    string UserId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    string PhoneNumber,
    string Position,
    string? Department,
    string? Specialization,
    string? LicenseNumber,
    DateTime HireDate,
    bool IsActive,
    DateTime CreatedAt,
    string PositionKey = "",
    string SalaryCalculationRule = "FixedPlusFees");

public sealed class StaffInput
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public string? Department { get; set; }
    public string? Specialization { get; set; }
    public string? LicenseNumber { get; set; }
    public string? SalaryCalculationRule { get; set; }
}

public sealed record WorkShiftDto(
    string Id,
    string StaffId,
    string? EquipmentId,
    DateTime Date,
    TimeSpan StartTime,
    TimeSpan EndTime,
    string? Notes,
    DateTime CreatedAt);

public sealed class WorkShiftInput
{
    public string StaffId { get; set; } = string.Empty;
    public string? EquipmentId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Notes { get; set; }
}

public sealed record LeaveDto(
    string Id,
    string StaffId,
    string LeaveType,
    DateTime StartDate,
    DateTime EndDate,
    string? Reason,
    DateTime CreatedAt,
    string LeaveTypeKey = "");

public sealed class LeaveInput
{
    public string StaffId { get; set; } = string.Empty;
    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
}

public sealed record ReferralDoctorDto(
    string Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    string Phone,
    string? Email,
    string? Specialization,
    string? Hospital,
    bool IsActive,
    DateTime CreatedAt);

public sealed class ReferralDoctorInput
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Specialization { get; set; }
    public string? Hospital { get; set; }
}