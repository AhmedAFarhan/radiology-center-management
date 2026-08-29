using FluentValidation;
using RadiologyCenter.Examinations.Application.Localization;

namespace RadiologyCenter.Examinations.Application.Commands.AddExaminationItem;

public class AddExaminationItemCommandValidator : AbstractValidator<AddExaminationItemCommand>
{
    public AddExaminationItemCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationIdRequired);
        RuleFor(x => x.ItemId).NotEmpty().WithErrorCode(ErrorCodes.ItemIdRequired);
        RuleFor(x => x.Quantity).GreaterThan(0).WithErrorCode(ErrorCodes.QuantityMustBePositive);
        RuleFor(x => x.Notes).MaximumLength(500).WithErrorCode(ErrorCodes.NotesTooLong).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
