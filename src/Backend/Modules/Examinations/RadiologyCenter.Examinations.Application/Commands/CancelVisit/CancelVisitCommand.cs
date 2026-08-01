namespace RadiologyCenter.Examinations.Application.Commands.CancelVisit;

public record CancelVisitCommand(Guid VisitId, string? Reason = null) : ICommand;
