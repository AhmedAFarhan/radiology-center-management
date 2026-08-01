using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Commands.ScheduleExamination;

public class ScheduleExaminationCommandValidator : AbstractValidator<ScheduleExaminationCommand>
{
    public ScheduleExaminationCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty();
        RuleFor(x => x.ScheduledAt).NotEmpty();
        RuleFor(x => x.ScheduledAt)
            .Must(s => s >= DateTime.UtcNow.AddMinutes(-1))
            .WithMessage("Scheduled time cannot be in the past.");
    }
}
