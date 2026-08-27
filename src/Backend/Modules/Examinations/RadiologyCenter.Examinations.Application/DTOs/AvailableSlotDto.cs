namespace RadiologyCenter.Examinations.Application.DTOs;

public record AvailableSlotDto(
    DateTime StartTime,
    DateTime EndTime,
    bool IsAvailable,
    string? ExaminationId,
    string? PatientName);
