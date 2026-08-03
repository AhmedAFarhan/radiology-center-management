using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.RemovePayslip;

public class RemovePayslipCommandValidator : AbstractValidator<RemovePayslipCommand>
{
    public RemovePayslipCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty();
        RuleFor(x => x.StaffId).NotEmpty();
    }
}