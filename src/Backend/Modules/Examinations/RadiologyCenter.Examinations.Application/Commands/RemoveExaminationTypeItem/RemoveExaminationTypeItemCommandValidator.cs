using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Examinations.Application.Commands.RemoveExaminationTypeItem;

public class RemoveExaminationTypeItemCommandValidator : AbstractValidator<RemoveExaminationTypeItemCommand>
{
    public RemoveExaminationTypeItemCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.ExaminationTypeItemId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}