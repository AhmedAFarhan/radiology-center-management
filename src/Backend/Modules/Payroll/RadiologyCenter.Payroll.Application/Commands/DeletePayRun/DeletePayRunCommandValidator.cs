using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.DeletePayRun;

public class DeletePayRunCommandValidator : AbstractValidator<DeletePayRunCommand>
{
    public DeletePayRunCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty().WithErrorCode(ErrorCodes.PayRunIdRequired);
    }
}