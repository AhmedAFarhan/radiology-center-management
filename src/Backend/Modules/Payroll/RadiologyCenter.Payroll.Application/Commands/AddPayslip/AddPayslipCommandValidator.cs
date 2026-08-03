using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.AddPayslip;

public class AddPayslipCommandValidator : AbstractValidator<AddPayslipCommand>
{
    public AddPayslipCommandValidator()
    {
        RuleFor(x => x.PayRunId).NotEmpty();
        RuleFor(x => x.StaffId).NotEmpty();
    }
}