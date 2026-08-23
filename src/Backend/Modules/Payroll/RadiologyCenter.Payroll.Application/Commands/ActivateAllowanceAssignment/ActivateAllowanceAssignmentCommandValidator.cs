using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateAllowanceAssignment;

public class ActivateAllowanceAssignmentCommandValidator : AbstractValidator<ActivateAllowanceAssignmentCommand>
{
    public ActivateAllowanceAssignmentCommandValidator() => RuleFor(x => x.Id).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired);
}