using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Commands.RecordExaminationPayment;

public class RecordExaminationPaymentCommandValidator : AbstractValidator<RecordExaminationPaymentCommand>
{
    public RecordExaminationPaymentCommandValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}