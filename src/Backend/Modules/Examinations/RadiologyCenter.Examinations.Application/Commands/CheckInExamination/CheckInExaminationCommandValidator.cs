using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Commands.CheckInExamination;

public class CheckInExaminationCommandValidator : AbstractValidator<CheckInExaminationCommand>
{
    public CheckInExaminationCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.ExaminationId).NotEmpty();
    }
}
