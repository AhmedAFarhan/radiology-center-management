using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateSalaryComponent;

public class DeactivateSalaryComponentCommandValidator : AbstractValidator<DeactivateSalaryComponentCommand>
{
    public DeactivateSalaryComponentCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
}