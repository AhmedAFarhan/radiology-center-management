using FluentValidation;
using RadiologyCenter.Catalog.Application.Commands.Common;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Catalog.Application.Commands.UpdateExaminationType;

public class UpdateExaminationTypeCommandValidator
    : ExaminationTypeValidatorBase<UpdateExaminationTypeCommand>
{
    public UpdateExaminationTypeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}