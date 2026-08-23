using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Catalog.Application.Commands.DeleteExaminationType;

public class DeleteExaminationTypeCommandValidator : AbstractValidator<DeleteExaminationTypeCommand>
{
    public DeleteExaminationTypeCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}
