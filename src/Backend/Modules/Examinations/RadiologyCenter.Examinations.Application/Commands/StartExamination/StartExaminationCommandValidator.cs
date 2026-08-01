using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Commands.StartExamination;

public class StartExaminationCommandValidator : AbstractValidator<StartExaminationCommand>
{
    public StartExaminationCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.ExaminationId).NotEmpty();
    }
}
