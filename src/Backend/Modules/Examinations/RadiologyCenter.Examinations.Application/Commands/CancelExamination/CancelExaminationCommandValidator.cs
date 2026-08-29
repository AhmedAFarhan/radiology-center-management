using FluentValidation;
using RadiologyCenter.Examinations.Application.Localization;

namespace RadiologyCenter.Examinations.Application.Commands.CancelExamination;

public class CancelExaminationCommandValidator : AbstractValidator<CancelExaminationCommand>
{
    public CancelExaminationCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationIdRequired);
        RuleFor(x => x.Reason).MaximumLength(500).WithErrorCode(ErrorCodes.NotesTooLong).When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}
