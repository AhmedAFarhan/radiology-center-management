using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Examinations.Application.Commands.AddExaminationTypeItem;

public class AddExaminationTypeItemCommandValidator : AbstractValidator<AddExaminationTypeItemCommand>
{
    public AddExaminationTypeItemCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.ItemId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Quantity).GreaterThan(0).WithErrorCode(SharedCodes.Shared.ValueMustBePositive);
        RuleFor(x => x.Notes).MaximumLength(500).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}