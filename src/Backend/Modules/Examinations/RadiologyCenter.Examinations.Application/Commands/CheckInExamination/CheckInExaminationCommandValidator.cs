using FluentValidation;
using RadiologyCenter.Examinations.Application.Localization;

namespace RadiologyCenter.Examinations.Application.Commands.CheckInExamination;

public class CheckInExaminationCommandValidator : AbstractValidator<CheckInExaminationCommand>
{
    public CheckInExaminationCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationIdRequired);
    }
}
