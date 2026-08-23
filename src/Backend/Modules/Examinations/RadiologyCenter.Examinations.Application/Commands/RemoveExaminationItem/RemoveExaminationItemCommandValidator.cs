using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Examinations.Application.Commands.RemoveExaminationItem;

public class RemoveExaminationItemCommandValidator : AbstractValidator<RemoveExaminationItemCommand>
{
    public RemoveExaminationItemCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.ExaminationItemId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}
