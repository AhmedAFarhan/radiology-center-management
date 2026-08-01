namespace RadiologyCenter.Examinations.Application.Commands.ScheduleExamination;

public record ScheduleExaminationCommand(
    Guid VisitId,
    Guid ExaminationId,
    DateTime ScheduledAt) : ICommand;
