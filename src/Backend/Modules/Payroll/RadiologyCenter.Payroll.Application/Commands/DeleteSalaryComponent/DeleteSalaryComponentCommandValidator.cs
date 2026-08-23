using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteSalaryComponent;

public class DeleteSalaryComponentCommandValidator : AbstractValidator<DeleteSalaryComponentCommand>
{
    public DeleteSalaryComponentCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
}