using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateSalaryComponent;

public class DeactivateSalaryComponentCommandValidator : AbstractValidator<DeactivateSalaryComponentCommand>
{
    public DeactivateSalaryComponentCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}