namespace RadiologyCenter.Examinations.Application.Commands.BookExamination;

public record BookExaminationCommand(
    Guid PatientId,
    Guid ExaminationTypeId,
    DateTime ScheduledAt,
    Guid? EquipmentId = null,
    Guid? RadiologistId = null,
    Guid? TechnicianId = null,
    Guid? ReferralDoctorId = null,
    string? ClinicalIndication = null,
    string Priority = "Routine",
    string? Notes = null) : ICommand;
