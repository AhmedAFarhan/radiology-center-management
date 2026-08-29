using FluentValidation;
using RadiologyCenter.Examinations.Application.Localization;

namespace RadiologyCenter.Examinations.Application.Commands.RecordExaminationPayment;

public class RecordExaminationPaymentCommandValidator : AbstractValidator<RecordExaminationPaymentCommand>
{
    public RecordExaminationPaymentCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationIdRequired);
        RuleFor(x => x.Amount).GreaterThan(0).WithErrorCode(ErrorCodes.QuantityMustBePositive);
        RuleFor(x => x.Description).MaximumLength(500).WithErrorCode(ErrorCodes.DescriptionTooLong);
    }
}
