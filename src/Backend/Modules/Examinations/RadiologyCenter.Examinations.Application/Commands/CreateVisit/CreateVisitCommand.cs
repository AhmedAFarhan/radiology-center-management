namespace RadiologyCenter.Examinations.Application.Commands.CreateVisit;

public record CreateVisitCommand(
    Guid PatientId,
    DateTime? VisitedAt = null,
    Guid? AppointmentId = null,
    string? Notes = null) : ICommand;
