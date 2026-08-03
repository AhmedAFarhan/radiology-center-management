namespace RadiologyCenter.Examinations.Application.Commands.CreateExamination;

public record CreateExaminationCommand(
    Guid PatientId,
    Guid ExaminationTypeId,
    Guid RadiologistId,
    Guid TechnicianId,
    Guid? ReferralDoctorId = null,
    string ClinicalIndication = null!,
    string Priority = "",
    decimal Discount = 0,
    bool IsDiscountPercentage = false,
    decimal Paid = 0,
    string? Notes = null) : ICommand;
