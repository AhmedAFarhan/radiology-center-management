using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateSalaryComponent;

public class DeactivateSalaryComponentCommandValidator : AbstractValidator<DeactivateSalaryComponentCommand>
{
    public DeactivateSalaryComponentCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.SalaryComponentIdRequired);
}