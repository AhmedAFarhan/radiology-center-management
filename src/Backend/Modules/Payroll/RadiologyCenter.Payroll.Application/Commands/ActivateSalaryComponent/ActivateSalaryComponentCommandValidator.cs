using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateSalaryComponent;

public class ActivateSalaryComponentCommandValidator : AbstractValidator<ActivateSalaryComponentCommand>
{
    public ActivateSalaryComponentCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
}