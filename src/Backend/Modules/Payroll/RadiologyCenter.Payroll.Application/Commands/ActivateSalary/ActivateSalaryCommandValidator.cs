using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateSalary;

public class ActivateSalaryCommandValidator : AbstractValidator<ActivateSalaryCommand>
{
    public ActivateSalaryCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.SalaryIdRequired);
}