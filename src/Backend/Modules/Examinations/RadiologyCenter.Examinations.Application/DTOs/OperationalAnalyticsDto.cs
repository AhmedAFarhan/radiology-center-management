using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.DTOs;

public record OperationalAnalyticsDto(
    int TotalExams,
    int CompletedExams,
    int CancelledExams,
    decimal CompletionRate,
    double AvgDurationMinutes,
    double AvgTimeToStartMinutes,
    IReadOnlyList<StatusCountDto> Funnel,
    IReadOnlyList<MonthlyVolumeDto> VolumeByMonth,
    IReadOnlyList<ModalityVolumeDto> VolumeByModality,
    IReadOnlyList<PriorityVolumeDto> VolumeByPriority);

public record StatusCountDto(
    string Status,
    int Count);

public record MonthlyVolumeDto(
    string Month,
    int Total,
    int Completed);

public record ModalityVolumeDto(
    string Modality,
    int Total,
    int Completed);

public record PriorityVolumeDto(
    string Priority,
    int Count);

/// <summary>
/// Lightweight projection of the operational fields of an examination, used by the analytics read side.
/// </summary>
public record OperationalExamProjection(
    Guid ExaminationTypeId,
    ExaminationStatus Status,
    ExaminationPriority Priority,
    DateTime? ScheduledAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    DateTime CreatedAt);
