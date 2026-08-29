using FluentValidation;
using RadiologyCenter.Examinations.Application.Localization;

namespace RadiologyCenter.Examinations.Application.Commands.CompleteExamination;

public class CompleteExaminationCommandValidator : AbstractValidator<CompleteExaminationCommand>
{
    public CompleteExaminationCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationIdRequired);
    }
}
