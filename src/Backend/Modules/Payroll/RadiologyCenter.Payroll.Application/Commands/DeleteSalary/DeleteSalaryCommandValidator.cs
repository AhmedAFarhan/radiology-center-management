using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteSalary;

public class DeleteSalaryCommandValidator : AbstractValidator<DeleteSalaryCommand>
{
    public DeleteSalaryCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.SalaryIdRequired);
}