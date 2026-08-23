using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Examinations.Application.Commands.AddExaminationItem;

public class AddExaminationItemCommandValidator : AbstractValidator<AddExaminationItemCommand>
{
    public AddExaminationItemCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.ItemId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Quantity).GreaterThan(0).WithErrorCode(SharedCodes.Shared.ValueMustBePositive);
        RuleFor(x => x.Notes).MaximumLength(500).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
