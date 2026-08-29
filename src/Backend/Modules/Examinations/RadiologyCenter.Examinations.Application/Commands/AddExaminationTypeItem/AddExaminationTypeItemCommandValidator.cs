using FluentValidation;
using RadiologyCenter.Examinations.Application.Localization;

namespace RadiologyCenter.Examinations.Application.Commands.AddExaminationTypeItem;

public class AddExaminationTypeItemCommandValidator : AbstractValidator<AddExaminationTypeItemCommand>
{
    public AddExaminationTypeItemCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationTypeIdRequired);
        RuleFor(x => x.ItemId).NotEmpty().WithErrorCode(ErrorCodes.ItemIdRequired);
        RuleFor(x => x.Quantity).GreaterThan(0).WithErrorCode(ErrorCodes.QuantityMustBePositive);
        RuleFor(x => x.Notes).MaximumLength(500).WithErrorCode(ErrorCodes.NotesTooLong).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
