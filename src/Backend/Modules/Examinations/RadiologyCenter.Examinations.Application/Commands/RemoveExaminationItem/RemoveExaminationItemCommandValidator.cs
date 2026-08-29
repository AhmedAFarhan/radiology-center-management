using FluentValidation;
using RadiologyCenter.Examinations.Application.Localization;

namespace RadiologyCenter.Examinations.Application.Commands.RemoveExaminationItem;

public class RemoveExaminationItemCommandValidator : AbstractValidator<RemoveExaminationItemCommand>
{
    public RemoveExaminationItemCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationIdRequired);
        RuleFor(x => x.ExaminationItemId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationItemIdRequired);
    }
}
