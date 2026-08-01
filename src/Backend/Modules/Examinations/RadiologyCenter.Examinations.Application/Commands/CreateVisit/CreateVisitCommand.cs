namespace RadiologyCenter.Examinations.Application.Commands.CreateVisit;

public record CreateVisitCommand(
    Guid PatientId,
    IReadOnlyList<CreateVisitExamination> Examinations,
    DateTime? VisitedAt = null,
    Guid? AppointmentId = null,
    string? Notes = null) : ICommand;
