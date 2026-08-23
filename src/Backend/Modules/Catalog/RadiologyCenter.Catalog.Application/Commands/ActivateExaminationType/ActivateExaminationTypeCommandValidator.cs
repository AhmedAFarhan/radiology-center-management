using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Catalog.Application.Commands.ActivateExaminationType;

public class ActivateExaminationTypeCommandValidator : AbstractValidator<ActivateExaminationTypeCommand>
{
    public ActivateExaminationTypeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}
