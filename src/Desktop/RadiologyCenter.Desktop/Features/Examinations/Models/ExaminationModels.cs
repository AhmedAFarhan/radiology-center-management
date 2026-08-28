namespace RadiologyCenter.Desktop.Features.Examinations.Models;

public sealed record ExaminationDto(
    string Id,
    string PatientId,
    string ExaminationTypeId,
    string ExaminationTypeName,
    string? ReferralDoctorId,
    string? RadiologistId,
    string? TechnicianId,
    string? EquipmentId,
    string ClinicalIndication,
    string Priority,
    string PriorityKey,
    string Status,
    string StatusKey,
    DateTime? ScheduledAt,
    DateTime? ScheduledEnd,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    string? PerformedByUserId,
    string? Notes,
    string? CancellationReason,
    decimal Price,
    decimal Discount,
    bool IsDiscountPercentage,
    decimal Paid,
    decimal Remaining,
    string? StudyInstanceUID,
    string? AccessionNumber,
    DateTime? ImagesReceivedAt,
    IReadOnlyList<ExaminationItemDto> Items);

public sealed record ExaminationItemDto(
    string Id,
    string ItemId,
    int Quantity,
    bool IsContrast,
    bool IsRequired,
    string? Notes);

public sealed record ExaminationTypeDto(
    string Id,
    string Code,
    string Name,
    string Modality,
    string BodyPart,
    int StandardDurationMinutes,
    decimal Price,
    bool RequiresPreparation,
    bool RequiresConsent,
    bool RequiresContrast,
    bool IsActive,
    IReadOnlyList<ExaminationTypeItemDto> Items,
    DateTime CreatedAt,
    string ModalityKey = "");

public sealed record ExaminationTypeItemDto(
    string Id,
    string ItemId,
    int Quantity,
    bool IsContrast,
    bool IsRequired,
    string? Notes);

public sealed class ExaminationTypeItemInput
{
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsContrast { get; set; }
    public bool IsRequired { get; set; }
    public string? Notes { get; set; }
}

public sealed class ExaminationTypeInput
{
    public string Name { get; set; } = string.Empty;
    public string Modality { get; set; } = string.Empty;
    public string BodyPart { get; set; } = string.Empty;
    public int StandardDurationMinutes { get; set; }
    public decimal Price { get; set; }
    public bool RequiresPreparation { get; set; }
    public bool RequiresConsent { get; set; }
    public List<ExaminationTypeItemInput> Items { get; set; } = new();
}

public sealed class ExaminationInput
{
    public string PatientId { get; set; } = string.Empty;
    public string ExaminationTypeId { get; set; } = string.Empty;
    public string? RadiologistId { get; set; }
    public string? TechnicianId { get; set; }
    public string? EquipmentId { get; set; }
    public string? ReferralDoctorId { get; set; }
    public string ClinicalIndication { get; set; } = string.Empty;
    public string Priority { get; set; } = "Routine";
    public decimal Discount { get; set; }
    public bool IsDiscountPercentage { get; set; }
    public decimal Paid { get; set; }
    public string? Notes { get; set; }
    public string? Status { get; set; }
    public string? ScheduledAt { get; set; }
}

public sealed class BookExamInput
{
    public string PatientId { get; set; } = string.Empty;
    public string ExaminationTypeId { get; set; } = string.Empty;
    public string ScheduledAt { get; set; } = string.Empty;
    public string? EquipmentId { get; set; }
    public string? RadiologistId { get; set; }
    public string? TechnicianId { get; set; }
    public string? ReferralDoctorId { get; set; }
    public string? ClinicalIndication { get; set; }
    public string Priority { get; set; } = "Routine";
    public string? Notes { get; set; }
}

public sealed class ExaminationUpdateInput
{
    public string? PatientId { get; set; }
    public string? ExaminationTypeId { get; set; }
    public string? RadiologistId { get; set; }
    public string? TechnicianId { get; set; }
    public string? EquipmentId { get; set; }
    public string? ReferralDoctorId { get; set; }
    public string ClinicalIndication { get; set; } = string.Empty;
    public string Priority { get; set; } = "Routine";
    public string? Notes { get; set; }
    public decimal? Discount { get; set; }
    public bool? IsDiscountPercentage { get; set; }
    public decimal? Paid { get; set; }
    public string? Status { get; set; }
    public string? ScheduledAt { get; set; }
    public List<ExaminationItemInput>? Items { get; set; }
}

public sealed class ExaminationItemInput
{
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsContrast { get; set; }
    public bool IsRequired { get; set; }
    public string? Notes { get; set; }
}

public sealed record CalendarSlotDto(
    string Id,
    string? EquipmentId,
    string? EquipmentName,
    string? RadiologistId,
    string? RadiologistName,
    string PatientName,
    string ExaminationTypeName,
    string Modality,
    DateTime ScheduledAt,
    DateTime? ScheduledEnd,
    string Status,
    string Priority);

public sealed record AvailableSlotDto(
    DateTime StartTime,
    DateTime EndTime,
    bool IsAvailable,
    string? ExaminationId,
    string? PatientName);

public sealed record ExamCheckedInNotificationDto(
    string ExaminationId,
    string PatientId,
    string PatientName,
    string PatientCode,
    string ExamName,
    string ExaminationTypeId,
    string StatusKey,
    DateTime? ScheduledAt,
    string Priority,
    string PriorityKey,
    string? Indication,
    string? RadiologistId,
    string? TechnicianId);