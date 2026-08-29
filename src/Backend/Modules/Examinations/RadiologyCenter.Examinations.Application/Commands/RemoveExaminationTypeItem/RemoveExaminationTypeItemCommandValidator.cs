using FluentValidation;
using RadiologyCenter.Examinations.Application.Localization;

namespace RadiologyCenter.Examinations.Application.Commands.RemoveExaminationTypeItem;

public class RemoveExaminationTypeItemCommandValidator : AbstractValidator<RemoveExaminationTypeItemCommand>
{
    public RemoveExaminationTypeItemCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationTypeIdRequired);
        RuleFor(x => x.ExaminationTypeItemId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationTypeItemIdRequired);
    }
}
