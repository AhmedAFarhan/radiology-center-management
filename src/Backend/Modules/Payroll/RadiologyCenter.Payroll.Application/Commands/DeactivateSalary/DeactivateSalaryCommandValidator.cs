using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateSalary;

public class DeactivateSalaryCommandValidator : AbstractValidator<DeactivateSalaryCommand>
{
    public DeactivateSalaryCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.SalaryIdRequired);
}