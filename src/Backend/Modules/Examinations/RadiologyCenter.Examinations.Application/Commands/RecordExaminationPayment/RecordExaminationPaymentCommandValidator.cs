using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Examinations.Application.Commands.RecordExaminationPayment;

public class RecordExaminationPaymentCommandValidator : AbstractValidator<RecordExaminationPaymentCommand>
{
    public RecordExaminationPaymentCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Amount).GreaterThan(0).WithErrorCode(SharedCodes.Shared.ValueMustBePositive);
        RuleFor(x => x.Description).MaximumLength(500).WithErrorCode(SharedCodes.Shared.TextTooLong);
    }
}