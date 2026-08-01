namespace RadiologyCenter.Examinations.Application.Commands.AddExaminationToVisit;

public record AddExaminationToVisitCommand(
    Guid VisitId,
    Guid ExaminationTypeId,
    string ReferringDoctor,
    string ClinicalIndication,
    string Priority,
    string? Notes = null) : ICommand;
