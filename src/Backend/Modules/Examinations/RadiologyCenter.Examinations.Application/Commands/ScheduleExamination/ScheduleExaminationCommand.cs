namespace RadiologyCenter.Examinations.Application.Commands.ScheduleExamination;

public record ScheduleExaminationCommand(
    Guid ExaminationId,
    DateTime ScheduledAt) : ICommand;
