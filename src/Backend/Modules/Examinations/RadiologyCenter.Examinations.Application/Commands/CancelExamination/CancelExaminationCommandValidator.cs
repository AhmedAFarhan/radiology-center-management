using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Commands.CancelExamination;

public class CancelExaminationCommandValidator : AbstractValidator<CancelExaminationCommand>
{
    public CancelExaminationCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty();
        RuleFor(x => x.Reason).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}
