using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.ApprovePayRun;

public class ApprovePayRunCommandValidator : AbstractValidator<ApprovePayRunCommand>
{
    public ApprovePayRunCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty().WithErrorCode(ErrorCodes.PayRunIdRequired);
    }
}