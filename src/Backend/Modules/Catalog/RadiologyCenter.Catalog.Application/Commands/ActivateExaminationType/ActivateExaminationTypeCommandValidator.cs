using FluentValidation;
using RadiologyCenter.Catalog.Application.Localization;

namespace RadiologyCenter.Catalog.Application.Commands.ActivateExaminationType;

public class ActivateExaminationTypeCommandValidator : AbstractValidator<ActivateExaminationTypeCommand>
{
    public ActivateExaminationTypeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationTypeIdRequired);
    }
}
