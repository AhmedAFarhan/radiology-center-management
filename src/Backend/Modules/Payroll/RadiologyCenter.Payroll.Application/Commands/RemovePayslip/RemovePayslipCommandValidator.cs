using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.RemovePayslip;

public class RemovePayslipCommandValidator : AbstractValidator<RemovePayslipCommand>
{
    public RemovePayslipCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty().WithErrorCode(ErrorCodes.PayRunIdRequired);
        RuleFor(x => x.StaffId).NotEmpty().WithErrorCode(ErrorCodes.StaffIdRequired);
    }
}