using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Catalog.Application.Commands.DeactivateExaminationType;

public class DeactivateExaminationTypeCommandValidator : AbstractValidator<DeactivateExaminationTypeCommand>
{
    public DeactivateExaminationTypeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}
