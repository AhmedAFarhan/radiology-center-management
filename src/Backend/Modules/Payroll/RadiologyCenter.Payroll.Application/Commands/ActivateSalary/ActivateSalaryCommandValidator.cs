using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateSalary;

public class ActivateSalaryCommandValidator : AbstractValidator<ActivateSalaryCommand>
{
    public ActivateSalaryCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
}