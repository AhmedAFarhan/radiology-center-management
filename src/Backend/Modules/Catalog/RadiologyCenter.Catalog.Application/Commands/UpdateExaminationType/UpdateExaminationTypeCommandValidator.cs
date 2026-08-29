using FluentValidation;
using RadiologyCenter.Catalog.Application.Commands.Common;
using RadiologyCenter.Catalog.Application.Localization;

namespace RadiologyCenter.Catalog.Application.Commands.UpdateExaminationType;

public class UpdateExaminationTypeCommandValidator
    : ExaminationTypeValidatorBase<UpdateExaminationTypeCommand>
{
    public UpdateExaminationTypeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationTypeIdRequired);
    }
}
