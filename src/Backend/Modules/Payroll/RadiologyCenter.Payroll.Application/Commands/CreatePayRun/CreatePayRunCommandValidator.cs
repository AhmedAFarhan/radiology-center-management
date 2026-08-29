using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.CreatePayRun;

public class CreatePayRunCommandValidator : AbstractValidator<CreatePayRunCommand>
{
    public CreatePayRunCommandValidator()
    {
        RuleFor(x => x.RunFrom).NotEqual(default(DateTime)).WithErrorCode(ErrorCodes.RunFromRequired);
        RuleFor(x => x.RunTo).NotEqual(default(DateTime)).WithErrorCode(ErrorCodes.RunToRequired);
        RuleFor(x => x.RunTo).GreaterThanOrEqualTo(x => x.RunFrom).WithErrorCode(ErrorCodes.PayRunEndOnOrAfterStart).When(x => x.RunFrom != default(DateTime));
    }
}