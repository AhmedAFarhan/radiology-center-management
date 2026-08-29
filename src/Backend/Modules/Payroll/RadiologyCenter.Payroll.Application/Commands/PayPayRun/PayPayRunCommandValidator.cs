using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.PayPayRun;

public class PayPayRunCommandValidator : AbstractValidator<PayPayRunCommand>
{
    public PayPayRunCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty().WithErrorCode(ErrorCodes.PayRunIdRequired);
    }
}