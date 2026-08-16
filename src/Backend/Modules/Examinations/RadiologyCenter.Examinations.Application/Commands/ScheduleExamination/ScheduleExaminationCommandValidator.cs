using FluentValidation;
using RadiologyCenter.Examinations.Application.Localization;

namespace RadiologyCenter.Examinations.Application.Commands.ScheduleExamination;

public class ScheduleExaminationCommandValidator : AbstractValidator<ScheduleExaminationCommand>
{
    public ScheduleExaminationCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty();
        RuleFor(x => x.ScheduledAt).NotEmpty();
        RuleFor(x => x.ScheduledAt)
            .Must(s => s >= DateTime.UtcNow.AddMinutes(-1))
            .WithErrorCode(ErrorCodes.ScheduledTimePast);
    }
}
