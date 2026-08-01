namespace RadiologyCenter.Examinations.Application.Commands.UpdateExamination;

public record UpdateExaminationCommand(
    Guid ExaminationId,
    string ReferringDoctor,
    string ClinicalIndication,
    string Priority,
    string? Notes = null,
    decimal? Discount = null,
    bool? IsDiscountPercentage = null,
    decimal? Paid = null,
    IReadOnlyList<UpdateExaminationItemRequest>? Items = null) : ICommand;
