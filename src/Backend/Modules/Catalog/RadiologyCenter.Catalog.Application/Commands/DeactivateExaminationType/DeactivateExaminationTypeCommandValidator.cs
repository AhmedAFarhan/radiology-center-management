using FluentValidation;
using RadiologyCenter.Catalog.Application.Localization;

namespace RadiologyCenter.Catalog.Application.Commands.DeactivateExaminationType;

public class DeactivateExaminationTypeCommandValidator : AbstractValidator<DeactivateExaminationTypeCommand>
{
    public DeactivateExaminationTypeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationTypeIdRequired);
    }
}
