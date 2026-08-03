namespace RadiologyCenter.Examinations.Application.Commands.UpdateExamination;

public record UpdateExaminationCommand(
    Guid ExaminationId,
    Guid RadiologistId,
    Guid TechnicianId,
    string ClinicalIndication,
    string Priority,
    Guid? ReferralDoctorId = null,
    string? Notes = null,
    decimal? Discount = null,
    bool? IsDiscountPercentage = null,
    decimal? Paid = null,
    IReadOnlyList<UpdateExaminationItemRequest>? Items = null) : ICommand;
