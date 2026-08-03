using FluentValidation;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteSalary;

public class DeleteSalaryCommandValidator : AbstractValidator<DeleteSalaryCommand>
{
    public DeleteSalaryCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}