using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateSalary;

public class DeactivateSalaryCommandValidator : AbstractValidator<DeactivateSalaryCommand>
{
    public DeactivateSalaryCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
}