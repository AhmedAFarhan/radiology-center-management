using FluentValidation;
using RadiologyCenter.Payroll.Application.Localization;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteSalaryComponent;

public class DeleteSalaryComponentCommandValidator : AbstractValidator<DeleteSalaryComponentCommand>
{
    public DeleteSalaryComponentCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.SalaryComponentIdRequired);
}