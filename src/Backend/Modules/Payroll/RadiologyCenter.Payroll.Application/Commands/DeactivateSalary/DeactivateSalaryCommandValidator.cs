using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateSalary;

public class DeactivateSalaryCommandValidator : AbstractValidator<DeactivateSalaryCommand>
{
    public DeactivateSalaryCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}