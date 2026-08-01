using RadiologyCenter.Examinations.Application.Commands.AddExaminationToVisit;

namespace RadiologyCenter.Examinations.Application.Commands.CreateVisit;

public record CreateVisitExamination(
    Guid ExaminationTypeId,
    string ReferringDoctor,
    string ClinicalIndication,
    string Priority,
    string? Notes = null,
    IReadOnlyList<AddExaminationToVisitItem>? Items = null);
