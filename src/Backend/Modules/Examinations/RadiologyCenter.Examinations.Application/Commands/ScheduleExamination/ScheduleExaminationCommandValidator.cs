using FluentValidation;
using RadiologyCenter.Examinations.Application.Localization;

namespace RadiologyCenter.Examinations.Application.Commands.ScheduleExamination;

public class ScheduleExaminationCommandValidator : AbstractValidator<ScheduleExaminationCommand>
{
    public ScheduleExaminationCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationIdRequired);
        RuleFor(x => x.ScheduledAt).NotEmpty().WithErrorCode(ErrorCodes.ScheduledAtRequired);
    }
}
