using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateSalaryComponent;

public class ActivateSalaryComponentCommandValidator : AbstractValidator<ActivateSalaryComponentCommand>
{
    public ActivateSalaryComponentCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.SalaryComponentIdRequired);
}