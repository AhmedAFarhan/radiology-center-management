namespace RadiologyCenter.Examinations.Application.DTOs;

public record ExaminationDto(
    Guid Id,
    Guid PatientId,
    Guid ExaminationTypeId,
    string ExaminationTypeName,
    string ReferringDoctor,
    string ClinicalIndication,
    string Priority,
    string Status,
    DateTime? ScheduledAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    Guid? PerformedByUserId,
    string? Notes,
    string? CancellationReason,
    decimal Price,
    decimal Discount,
    bool IsDiscountPercentage,
    decimal Paid,
    decimal Remaining,
    IReadOnlyList<ExaminationItemDto> Items);
