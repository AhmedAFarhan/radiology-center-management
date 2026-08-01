namespace RadiologyCenter.Examinations.Application.Commands.UpdateExamination;

public record UpdateExaminationCommand(
    Guid VisitId,
    Guid ExaminationId,
    string ReferringDoctor,
    string ClinicalIndication,
    string Priority,
    string? Notes = null,
    IReadOnlyList<UpdateExaminationItemRequest>? Items = null) : ICommand;
