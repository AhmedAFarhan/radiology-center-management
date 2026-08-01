using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Commands.ScheduleExamination;

public class ScheduleExaminationCommandValidator : AbstractValidator<ScheduleExaminationCommand>
{
    public ScheduleExaminationCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.ExaminationId).NotEmpty();
        RuleFor(x => x.ScheduledAt).NotEmpty();
    }
}
