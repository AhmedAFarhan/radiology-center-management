using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.AddPayslip;

public class AddPayslipCommandValidator : AbstractValidator<AddPayslipCommand>
{
    public AddPayslipCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty().WithErrorCode(ErrorCodes.PayRunIdRequired);
        RuleFor(x => x.StaffId).NotEmpty().WithErrorCode(ErrorCodes.StaffIdRequired);
    }
}