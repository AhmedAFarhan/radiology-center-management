using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Commands.CompleteExamination;

public class CompleteExaminationCommandValidator : AbstractValidator<CompleteExaminationCommand>
{
    public CompleteExaminationCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.ExaminationId).NotEmpty();
    }
}
