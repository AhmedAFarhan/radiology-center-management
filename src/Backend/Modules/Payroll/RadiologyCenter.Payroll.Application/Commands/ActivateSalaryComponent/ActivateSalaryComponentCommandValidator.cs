using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateSalaryComponent;

public class ActivateSalaryComponentCommandValidator : AbstractValidator<ActivateSalaryComponentCommand>
{
    public ActivateSalaryComponentCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}